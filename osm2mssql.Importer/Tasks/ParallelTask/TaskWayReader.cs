using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using osm2mssql.Importer.Enums;
using osm2mssql.Importer.OsmReader;

namespace osm2mssql.Importer.Tasks.ParallelTask
{
	public class TaskWayReader : TaskBulkInsertBase
	{
		private int _countOfInsertedWays;
		private double? _timeOffset = null;
		public TaskWayReader(string name) : base(TaskType.ParallelTask, name)
		{
		}

		internal override void DurationRefresh()
		{
			base.DurationRefresh();
			if (_timeOffset.HasValue)
				AdditionalInfos = (int)(_countOfInsertedWays / (Duration.TotalSeconds - _timeOffset)) + " Ways / sec.";
		}

		protected override Task DoTaskWork(string osmFile, AttributeRegistry attributeRegistry)
		{
			var watch = Stopwatch.StartNew();
			ExecuteSqlCmd("TRUNCATE TABLE [dbo].[tWayCreation]");
			ExecuteSqlCmd("TRUNCATE TABLE [dbo].[tWayTag]");

			var dWays = new DataTable();
			dWays.TableName = "tWayCreation";
			dWays.MinimumCapacity = MaxRowCountInMemory;
			dWays.Columns.Add("wayId", typeof(long));
			dWays.Columns.Add("nodeId", typeof(long));
			dWays.Columns.Add("sort");

			var dWayTags = new DataTable();
			dWayTags.TableName = "tWayTag";
			dWayTags.MinimumCapacity = MaxRowCountInMemory;
			dWayTags.Columns.Add("WayId", typeof(long));
			dWayTags.Columns.Add("Typ", typeof(int));
			dWayTags.Columns.Add("Info", typeof(string));

			var insertingTask = Task.Factory.StartNew(() => StartInserting());
			var reader = osmFile.EndsWith(".pbf")
				? (IOsmReader)new PbfOsmReader()
				: (IOsmReader)new XmlOsmReader();

			foreach (var way in reader.ReadWays(osmFile, attributeRegistry))
			{
				if (!_timeOffset.HasValue)
				{
					watch.Stop();
					_timeOffset = watch.Elapsed.TotalSeconds;
				}
				_countOfInsertedWays++;
				var sort = 0;

				foreach (var node in way.NodeRefs)
					dWays = AddToCollection(dWays, way.WayId, node, sort++);

				foreach (var tag in way.Tags)
					dWayTags = AddToCollection(dWayTags, way.WayId, tag.Typ, tag.Value);
			}

			DataTableCollection.Add(dWays);
			DataTableCollection.Add(dWayTags);
			DataTableCollection.CompleteAdding();

			Trace.WriteLine(string.Format("Inserted {0} ways", _countOfInsertedWays));
			return insertingTask.Result;
		}
	}
}