using MySQLBackup.Application.Config;
using MySQLBackup.Application.Logging;
using MySQLBackup.Application.Scheduler;
using System.ServiceProcess;

namespace MySQLBackupService
{
    public partial class MySQLBackupService : ServiceBase
    {
        JobHandler jobHandler;

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
            jobHandler.ScheduleJobs();
            new ConfigLocationCreator().CreateConfigLocations();
            new LogHandler().LogMessage(LogHandler.MessageType.INFO, "The MySQL Backup Service has been started");
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
            new LogHandler().LogMessage(LogHandler.MessageType.INFO, "The MySQL Backup Service has been stopped");
        }
    }
}
