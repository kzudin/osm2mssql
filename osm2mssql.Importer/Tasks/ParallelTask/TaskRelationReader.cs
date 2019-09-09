using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using osm2mssql.Importer.Enums;
using osm2mssql.Importer.OsmReader;

namespace osm2mssql.Importer.Tasks.ParallelTask
{
	public class TaskRelationReader : TaskBulkInsertBase
	{
		private double _countOfInsertedRelations;
		public TaskRelationReader(string name) : base(TaskType.ParallelTask, name)
		{
		}

		internal override void DurationRefresh()
		{
			base.DurationRefresh();
			AdditionalInfos = (int)(_countOfInsertedRelations / Duration.TotalSeconds) + " Relations / sec.";
		}

		protected override Task DoTaskWork(string osmFile, AttributeRegistry attributeRegistry)
		{
			var reader = osmFile.EndsWith(".pbf")
				? (IOsmReader)new PbfOsmReader()
				: (IOsmReader)new XmlOsmReader();

			ExecuteSqlCmd("TRUNCATE TABLE [dbo].[tRelationCreation]");
			ExecuteSqlCmd("TRUNCATE TABLE [dbo].[tRelationTag]");

			var dRelationCreation = new DataTable { MinimumCapacity = MaxRowCountInMemory };
			dRelationCreation.TableName = "tRelationCreation";
			dRelationCreation.Columns.Add("RelationId", typeof(long));
			dRelationCreation.Columns.Add("Ref",        typeof(long));
			dRelationCreation.Columns.Add("Type");
			dRelationCreation.Columns.Add("Role");
			dRelationCreation.Columns.Add("Sort");

			var dRelationTags = new DataTable { MinimumCapacity = MaxRowCountInMemory };
			dRelationTags.TableName = "tRelationTag";
			dRelationTags.Columns.Add("RelationId", typeof(long));
			dRelationTags.Columns.Add("Typ");
			dRelationTags.Columns.Add("Info");

			var insertingTask = Task.Factory.StartNew(() => StartInserting());

			foreach (var relation in reader.ReadRelations(osmFile, attributeRegistry))
			{
				_countOfInsertedRelations++;
				var sort = 0;
				foreach (var member in relation.Members)
				{
					dRelationCreation = AddToCollection(dRelationCreation, relation.RelationId, member.Ref, member.Type, member.Role, sort++);
				}

				foreach (var tag in relation.Tags)
					dRelationTags = AddToCollection(dRelationTags, relation.RelationId, tag.Typ, tag.Value);
			}

			DataTableCollection.Add(dRelationCreation);
			DataTableCollection.Add(dRelationTags);
			DataTableCollection.CompleteAdding();

			Trace.WriteLine(string.Format("Inserted {0} relations", _countOfInsertedRelations));
			return insertingTask.Result;
		}
	}
}