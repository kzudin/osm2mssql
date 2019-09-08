using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using osm2mssql.Importer.OsmReader;
using osm2mssql.Importer.Properties;
using osm2mssql.Importer.Tasks.ParallelFinishTask;

namespace osm2mssql.Importer.Tasks.FinishTasks
{
	class TaskExecuteSqlCommands : TaskBase
	{
		public TaskExecuteSqlCommands(string name) : base(TaskType.FinishTask, name)
		{
		}
		protected override async Task DoTaskWork(string osmFile, AttributeRegistry attributeRegistry)
		{
			await Task.Run(() =>
			{
				foreach (var sqlDir in Settings.Default.SqlDirs)
					ExecuteFolderScript(sqlDir);
			});
		}
		private void ExecuteFolderScript(string folder)
		{
			var changeFiles = Directory.GetFiles(folder, "*.sql");
			Array.Sort(changeFiles);
			foreach (var file in changeFiles)
			{
				AdditionalInfos = " executing batch " + Path.GetFileName(file);
				Trace.TraceInformation("Starting SQL file {0}", file);
				var sw = new Stopwatch();
				sw.Start();
				ExecuteSqlCmd(File.ReadAllText(file).Replace("[OSM]", Connection.InitialCatalog));
				sw.Stop();
				Trace.TraceInformation("SQL file execution finished in {0}: {1}", sw.Elapsed, file);
			}
			AdditionalInfos = string.Empty;
		}
	}
}