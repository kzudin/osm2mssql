﻿using System.IO;
using System.Threading.Tasks;
using osm2mssql.Importer.Enums;
using osm2mssql.Importer.OsmReader;

namespace osm2mssql.Importer.Tasks.FinishTasks
{
	public class TaskClearAndFillRelations : TaskBase
	{
		public TaskClearAndFillRelations(string name)
			: base(TaskType.FinishTask, name)
		{
		}

		protected override async Task DoTaskWork(string osmFile, AttributeRegistry attributeRegistry)
		{
			await Task.Run(() =>
			{
				ExecuteSqlCmd(File
					.ReadAllText($@"SQL\{GetType().Name}.sql")
					.Replace("[OSM]", Connection.InitialCatalog));
			});
		}
	}
}