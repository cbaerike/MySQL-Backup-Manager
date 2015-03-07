using MySQLBackup.Application.Config;
using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace MySQLBackupService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectInstaller"/> class.
        /// </summary>
        public ProjectInstaller()
        {
            InitializeComponent();
            this.AfterInstall += new InstallEventHandler(ServiceInstaller_AfterInstall);
        }

        /// <summary>
        /// Handles the AfterInstall event of the ServiceInstaller control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="InstallEventArgs"/> instance containing the event data.</param>
        void ServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            this.AddMySqlBinToSystemPath();
            ServiceController sc = new ServiceController("MySQL Backup Service");
            sc.Start();
        }

        /// <summary>
        /// Adds MySQL bin folder to system path.
        /// </summary>
        private void AddMySqlBinToSystemPath()
        {
            String binLocation = ConfigurationHandler.RetrieveMySQLInstallationBinPath();
            if (!String.IsNullOrEmpty(binLocation))
            {
                string path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
                if (path == null)
                {
                    Environment.SetEnvironmentVariable("PATH", binLocation, EnvironmentVariableTarget.Machine);
                }
                else if (!path.Contains(binLocation))
                {
                    path += ";" + binLocation;
                    Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Machine);
                }
            }
        }
    }
}
