﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using osm2mssql.Importer.Enums;
using osm2mssql.Importer.Languages;
using osm2mssql.Importer.OsmReader;
using osm2mssql.Importer.Tasks.FinishTasks;
using osm2mssql.Importer.Tasks.InitializeTasks;
using osm2mssql.Importer.Tasks.ParallelFinishTask;
using osm2mssql.Importer.Tasks.ParallelTask;

namespace osm2mssql.Importer.Tasks
{
	public class TaskRunner : IDisposable
	{
		public ObservableCollection<TaskBase> Tasks { get; } = new ObservableCollection<TaskBase>();

		private Timer _timer;

		public TaskRunner()
		{
			_timer = new Timer();
			_timer.Interval = TimeSpan.FromSeconds(1).TotalMilliseconds;
			_timer.Elapsed += (o, e) => RefreshTime();
			_timer.Start();
		}

		private void RefreshTime()
		{
			foreach (var task in Tasks.Where(x => x.Result == TaskResult.InProgress))
			{
				task.DurationRefresh();
			}
		}

		public void FillTaskList()
		{
			Tasks.Clear();
			Tasks.Add(new TaskCreateDatabase        (Language.CurrentLanguage[typeof(TaskCreateDatabase)       .Name]));
			Tasks.Add(new TaskCreateTables          (Language.CurrentLanguage[typeof(TaskCreateTables)         .Name]));
			Tasks.Add(new TaskInstallDbExtension    (Language.CurrentLanguage[typeof(TaskInstallDbExtension)   .Name]));
			Tasks.Add(new TaskCreateIndicesNode     (Language.CurrentLanguage[typeof(TaskCreateIndicesNode)    .Name]));
			Tasks.Add(new TaskCreateIndicesWay      (Language.CurrentLanguage[typeof(TaskCreateIndicesWay)     .Name]));
			Tasks.Add(new TaskCreateIndicesRelation (Language.CurrentLanguage[typeof(TaskCreateIndicesRelation).Name]));
			Tasks.Add(new TaskCreateSpatialIndices  (Language.CurrentLanguage[typeof(TaskCreateSpatialIndices) .Name]));

			Tasks.Add(new TaskNodeReader            (Language.CurrentLanguage[typeof(TaskNodeReader)           .Name]));
			Tasks.Add(new TaskWayReader             (Language.CurrentLanguage[typeof(TaskWayReader)            .Name]));
			Tasks.Add(new TaskRelationReader        (Language.CurrentLanguage[typeof(TaskRelationReader)       .Name]));

			Tasks.Add(new TaskAttributeWriter       (Language.CurrentLanguage[typeof(TaskAttributeWriter)      .Name]));

			Tasks.Add(new TaskClearAndFillWays      (Language.CurrentLanguage[typeof(TaskClearAndFillWays)     .Name]));
			Tasks.Add(new TaskClearAndFillRelations (Language.CurrentLanguage[typeof(TaskClearAndFillRelations).Name]));
			Tasks.Add(new TaskExecuteSqlCommands    (Language.CurrentLanguage[typeof(TaskExecuteSqlCommands)   .Name]));
		}

		public Task RunTasks(SqlConnectionStringBuilder connectionStringBuilder, string fileName)
		{
			return Task.Run(() => RunAllTasksAsync(connectionStringBuilder, fileName));
		}

		private async Task RunAllTasksAsync(SqlConnectionStringBuilder connectionStringBuilder, string fileName)
		{
			foreach (var task in Tasks)
				task.Result = TaskResult.Pending;

			var attributeRegistry = new AttributeRegistry();
			var watch = Stopwatch.StartNew();
			///////////////////////////////////////////////////
			RunTasksSynchron(Tasks.Where(d => d.IsEnabled && d.Type == TaskType.InitializeTask), connectionStringBuilder, fileName, attributeRegistry);
			///////////////////////////////////////////////////
			await RunTasksParallel(Tasks.Where(d => d.IsEnabled && d.Type == TaskType.ParallelTask), connectionStringBuilder, fileName, attributeRegistry);
			await RunTasksParallel(Tasks.Where(d => d.IsEnabled && d.Type == TaskType.ParallelFinishTask), connectionStringBuilder, fileName, attributeRegistry);
			///////////////////////////////////////////////////
			RunTasksSynchron(Tasks.Where(d => d.IsEnabled && d.Type == TaskType.FinishTask), connectionStringBuilder, fileName, attributeRegistry);
			///////////////////////////////////////////////////
			var processSummary = string.Format("Total duration: {0}", watch.Elapsed);
			Trace.WriteLine(processSummary);
		}

		private void RunTasksSynchron(IEnumerable<TaskBase> tasks, SqlConnectionStringBuilder connectionStringBuilder, string fileName, AttributeRegistry attributeRegistry)
		{
			foreach (var task in tasks)
			{
				task.RunTask(connectionStringBuilder, fileName, attributeRegistry).Wait();
			}
		}

		private Task RunTasksParallel(IEnumerable<TaskBase> tasks, SqlConnectionStringBuilder connectionStringBuilder, string fileName, AttributeRegistry attributeRegistry)
		{
			var runningTasks = tasks.Select(task => Task.Run(() => task.RunTask(connectionStringBuilder, fileName, attributeRegistry))).ToList();
			return Task.WhenAll(runningTasks);
		}

		public void Dispose()
		{
			if (_timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}
		}
	}
}