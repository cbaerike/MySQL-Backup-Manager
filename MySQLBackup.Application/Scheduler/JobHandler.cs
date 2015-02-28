using MySQLBackup.Application.Config;
using MySQLBackup.Application.Logging;
using MySQLBackup.Application.Model;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;

namespace MySQLBackup.Application.Scheduler
{
    public class JobHandler : IDisposable
    {
        private IScheduler scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobHandler"/> class.
        /// </summary>
        public JobHandler() { }

        /// <summary>
        /// Schedules configured backups, and a daily cleanup task.
        /// </summary>
        public void ScheduleJobs()
        {
            LogHandler logHandler = new LogHandler();
            List<DatabaseInfo> dbNodes = new DatabasesHandler().GetAllDatabaseNodes();
            if (0 == dbNodes.Count)
            {
                logHandler.LogMessage(LogHandler.MessageType.WARNING, "MySQL Backup Scheduler - No database nodes found. No backups will be scheduled.");
            }
            else
            {
                scheduler = StdSchedulerFactory.GetDefaultScheduler();
                scheduler.Start();
                foreach (DatabaseInfo dbNode in dbNodes)
                {
                    //Schedule backup job
                    IJobDetail backupJobDetail = JobBuilder.Create<CreateBackupJob>()
                        .WithIdentity(dbNode.DatabaseName + "_Job", "Backup")
                        .UsingJobData(CreateBackupJob.Properties.DatabaseName.ToString(), dbNode.DatabaseName)
                        .Build();
                    ITrigger backupJobTrigger = TriggerBuilder.Create()
                        .WithIdentity(dbNode.DatabaseName + "_Trigger", "Backup")
                        .StartAt(DateBuilder.TodayAt(dbNode.StartTime.Hours, dbNode.StartTime.Minutes, 0))
                        .WithSimpleSchedule(x => x.WithIntervalInHours(24).RepeatForever())
                        .Build();
                    scheduler.ScheduleJob(backupJobDetail, backupJobTrigger);
                    logHandler.LogMessage(LogHandler.MessageType.INFO, "Backup job scheduled for database: " + dbNode.DatabaseName);
                }
                //Schedule cleanup job
                IJobDetail cleanupJobDetail = JobBuilder.Create<DeleteOldBackupsJob>()
                        .WithIdentity("Delete_Old_Job", "Cleanup")
                        .Build();
                ITrigger cleanupTrigger = TriggerBuilder.Create()
                        .WithIdentity("Delete_Old_Trigger", "Cleanup")
                        .StartAt(DateBuilder.TodayAt(1, 0, 0))
                        .WithSimpleSchedule(x => x.WithIntervalInHours(24).RepeatForever())
                        .Build();
                scheduler.ScheduleJob(cleanupJobDetail, cleanupTrigger);
                logHandler.LogMessage(LogHandler.MessageType.INFO, "Cleanup job scheduled.");
            }
        }

        /// <summary>
        /// Shuts down the scheduler. Does not wait for jobs to complete.
        /// </summary>
        public void Dispose()
        {
            if (null != scheduler && scheduler.IsStarted)
            {
                scheduler.Shutdown(false);
            }
        }
    }
}
