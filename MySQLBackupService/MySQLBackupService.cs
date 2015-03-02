using MySQLBackup.Application.Config;
using MySQLBackup.Application.Logging;
using MySQLBackup.Application.Scheduler;
using System.IO;
using System.ServiceProcess;

namespace MySQLBackupService
{
    public partial class MySQLBackupService : ServiceBase
    {
        JobHandler jobHandler;
        FileSystemWatcher configFileWatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySQLBackupService"/> class.
        /// </summary>
        public MySQLBackupService()
        {
            InitializeComponent();
            jobHandler = new JobHandler();
        }

        /// <summary>
        /// Schedules configured backups, and a daily cleanup task.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            new ConfigLocationCreator().CreateConfigLocations();
            jobHandler.ScheduleJobs();
            CreateConfigFileWatcher();
            new LogHandler().LogMessage(LogHandler.MessageType.INFO, "The MySQL Backup Service has been started.");
        }

        /// <summary>
        /// Shuts down the scheduler. Does not wait for jobs to complete.
        /// </summary>
        protected override void OnStop()
        {
            if (null != jobHandler)
            {
                jobHandler.Dispose();
            }
            if (null != configFileWatcher)
            {
                configFileWatcher.Dispose();
            }
            new LogHandler().LogMessage(LogHandler.MessageType.INFO, "The MySQL Backup Service has been stopped.");
        }

        /// <summary>
        /// Creates the configuration file watcher. If a change to the file is detected, jobs will be rescheduled.
        /// </summary>
        private void CreateConfigFileWatcher()
        {
            configFileWatcher = new FileSystemWatcher();
            configFileWatcher.Path = ConfigurationHandler.CONFIGURATION_LOCATION;
            configFileWatcher.Filter = ConfigurationHandler.DB_CONFIG_FILE;
            configFileWatcher.IncludeSubdirectories = false;
            configFileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            configFileWatcher.Changed += configFileWatcher_Changed;
            configFileWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Handles the Changed event of the configFileWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void configFileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (null != jobHandler)
            {
                jobHandler.Shutdown(true);
                jobHandler.ScheduleJobs();
                new LogHandler().LogMessage(LogHandler.MessageType.INFO, "MySQL Backup Service: Configuration file change detected. Backups rescheduled.");
            }
        }
    }
}
