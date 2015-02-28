using MySQLBackup.Application.Config;
using MySQLBackup.Application.Logging;
using MySQLBackup.Application.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLBackupManager.Pages.Content
{
    class DatabasesViewModel
    {
        private readonly DatabasesHandler dbHandler = new DatabasesHandler();
        private ObservableCollection<DatabaseInfo> databases = new ObservableCollection<DatabaseInfo>();

        public ObservableCollection<DatabaseInfo> Databases
        {
            get
            {
                foreach (DatabaseInfo dbInfo in dbHandler.GetAllDatabaseNodes())
                {
                    databases.Add(dbInfo);
                }
                return databases;
            }
        }

        /**
         * Add a Database Info object to the collection
         */
        public void addDatabase(DatabaseInfo dbInfo)
        {
            this.databases.Add(dbInfo);
            dbHandler.InsertDatabaseNode(dbInfo);
            new LogHandler().LogMessage(LogHandler.MessageType.INFO, string.Format("The database {0} is now ready for backup", dbInfo.DatabaseName));
        }
    }
}
