using System.Threading.Tasks;
using osm2mssql.Importer.Enums;
using osm2mssql.Importer.OsmReader;

namespace osm2mssql.Importer.Tasks.InitializeTasks
{
	public class TaskCreateTables : TaskBase
	{
		private string _database;
		private string _osmFile;
		public TaskCreateTables(string name) : base(TaskType.InitializeTask, name)
		{
		}
		protected override async Task DoTaskWork(string osmFile, AttributeRegistry attributeRegistry)
		{
			await Task.Run(() =>
			{
				_database = Connection.InitialCatalog;
				_osmFile = osmFile;
				Connection.InitialCatalog = string.Empty;

				CreateTables();

				Connection.InitialCatalog = _database;
			});
		}
		internal void CreateTables()
		{
			var createTables = App.GetResourceFileText("osm2mssql.Importer.SQL.CreateTables.sql");
			StepDescription = "Creating tables...";
			createTables = createTables.Replace("[OSM]", _database);
			ExecuteSqlCmd("ALTER DATABASE " + _database + " SET trustworthy ON");
			ExecuteSqlCmd(createTables);
		}
	}
}