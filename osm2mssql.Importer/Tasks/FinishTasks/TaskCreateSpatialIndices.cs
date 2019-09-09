using System.IO;
using System.Threading.Tasks;
using osm2mssql.Importer.Enums;
using osm2mssql.Importer.OsmReader;

namespace osm2mssql.Importer.Tasks.FinishTasks
{
	public class TaskCreateSpatialIndices : TaskBase
	{
		public TaskCreateSpatialIndices(string name) : base(TaskType.FinishTask, name)
		{
		}

		protected override Task DoTaskWork(string osmFile, AttributeRegistry attributeRegistry)
		{
			var task1 = Task.Run(() => ExecuteSqlCmd(File
				.ReadAllText($@"SQL\{GetType().Name}Way.sql")
				.Replace("[OSM]", Connection.InitialCatalog)));
			var task2 = Task.Run(() => ExecuteSqlCmd(File
				.ReadAllText($@"SQL\{GetType().Name}Node.sql")
				.Replace("[OSM]", Connection.InitialCatalog)));
			return Task.WhenAll(new[] { task1, task2 });
		}
	}
}