using MySQLBackup.Application.Backup;
using MySQLBackup.Application.Logging;
using Quartz;
using System;
using System.Diagnostics;

namespace MySQLBackup.Application.Scheduler
{
    class CreateBackupJob : IJob
    {
        /// <summary>
        /// A list of all available job properties.
        /// </summary>
        public enum Properties
        {
            DatabaseName
        };

        /// <summary>
        /// Creates a backup of the database specified in the job details.
        /// </summary>
        /// <param name="context">The execution context.</param>
        public void Execute(IJobExecutionContext context)
        {
            String databaseName = context.JobDetail.JobDataMap.GetString(CreateBackupJob.Properties.DatabaseName.ToString());
            BackupHandler backupHandler = new BackupHandler();
            try
            {
                backupHandler.CreateBackup(databaseName);
            }
            catch (Exception ex)
            {
                new LogHandler().LogMessage(LogHandler.MessageType.ERROR, "Job: " + context.JobDetail.Key + Environment.NewLine + ex.ToString());
            }
        }
    }
}
