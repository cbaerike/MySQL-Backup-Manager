using MySQLBackup.Application.Util;
using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace MySQLBackupService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            this.AfterInstall += new InstallEventHandler(ServiceInstaller_AfterInstall);
        }

        void ServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            this.CheckSystemPathVariable();
            ServiceController sc = new ServiceController("MySQL Backup Service");
            sc.Start();
        }

        private void CheckSystemPathVariable()
        {
            String binLocation = Utilities.RetrieveMySQLInstallationBinPath();
            if (!String.IsNullOrEmpty(binLocation))
            {
                string path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
                if (path == null)
                {
                    Environment.SetEnvironmentVariable("PATH", Utilities.RetrieveMySQLInstallationBinPath(), EnvironmentVariableTarget.Machine);
                }
                else if (!path.Contains(Utilities.RetrieveMySQLInstallationBinPath()))
                {
                    path += ";" + Utilities.RetrieveMySQLInstallationBinPath();
                    Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Machine);
                }
            }
        }
    }
}
