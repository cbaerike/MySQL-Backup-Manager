using MySQLBackup.Application.Config;
using MySQLBackup.Application.Logging;
using Quartz;
using System;
using System.IO;

namespace MySQLBackup.Application.Scheduler
{
    class DeleteOldBackupsJob : IJob
    {
        LogHandler logHandler;

        /// <summary>
        /// Deletes old backup files. How old a backup file may become is specified by the user in the configurations file.
        /// </summary>
        /// <param name="context">The execution context.</param>
        public void Execute(IJobExecutionContext context)
        {
            DeleteOldBackupFiles();
        }

        /// <summary>
        /// Deletes the old backup files.
        /// </summary>
        private void DeleteOldBackupFiles()
        {
            logHandler = new LogHandler();
            int days = ConfigurationXmlHandler.GetDeleteBackupsOlderThanDays();
            string backupLocation = ConfigurationXmlHandler.GetBackupLocation();

            if (days > 0)
            {
                try
                {
                    ProcessDeleteFiles(backupLocation, days + 1); //Add 1 to number of days, since it has to delete files older than the specified number of days
                    logHandler.LogMessage(LogHandler.MessageType.INFO, string.Format("Cleaned up backup files older than {0} days", days));
                }
                catch (Exception ex)
                {
                    logHandler.LogMessage(LogHandler.MessageType.ERROR, string.Format("Error while cleaning up files older than {0} days: {1}", days, ex.ToString()));
                }
            }
        }

        /// <summary>
        /// Process which handles the deletion of files older than the specified number of days. Moves recursively through all subdirectories.
        /// </summary>
        /// <param name="backupLocation">The backup location.</param>
        /// <param name="days">The days.</param>
        private void ProcessDeleteFiles(string backupLocation, int days)
        {
            string[] files = Directory.GetFiles(backupLocation);
            foreach (string fileName in files)
            {
                DateTime fileCreatedDate = File.GetCreationTime(fileName);
                if (DateTime.Now - fileCreatedDate > TimeSpan.FromDays(days))
                {
                    File.Delete(fileName);
                    logHandler.LogMessage(LogHandler.MessageType.INFO, "File deleted: " + fileName);
                }
            }

            //Get all Subdirectories to the backup path
            string[] subDirs = Directory.GetDirectories(backupLocation);
            foreach (string subDir in subDirs)
            {
                ProcessDeleteFiles(subDir, days);
            }
        }
    }
}
