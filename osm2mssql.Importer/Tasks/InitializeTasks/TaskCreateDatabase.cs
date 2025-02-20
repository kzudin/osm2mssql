﻿using System.Threading.Tasks;
using osm2mssql.Importer.Enums;
using osm2mssql.Importer.OsmReader;

namespace osm2mssql.Importer.Tasks.InitializeTasks
{
	public class TaskCreateDatabase : TaskBase
	{
		private string _database;
		private string _osmFile;
		public TaskCreateDatabase(string name) : base(TaskType.InitializeTask, name)
		{
		}
		protected override async Task DoTaskWork(string osmFile, AttributeRegistry attributeRegistry)
		{
			await Task.Run(() =>
			{
				_database = Connection.InitialCatalog;
				_osmFile = osmFile;
				Connection.InitialCatalog = string.Empty;

				CreateDatabase();

				Connection.InitialCatalog = _database;
			});
		}
		internal void CreateDatabase()
		{
			StepDescription = "Creating database...";
			var createSql = App.GetResourceFileText("osm2mssql.Importer.SQL.CreateDatabase.sql");
			createSql = createSql.Replace("[OSM]", _database);
			ExecuteSqlCmd(createSql);            
		}
	}
}