using System.ServiceProcess;

namespace MySQLBackupService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun = new ServiceBase[] 
            { 
                new MySQLBackupService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
