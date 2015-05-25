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
        private readonly DatabasesXmlHandler dbHandler = new DatabasesXmlHandler();
        private ObservableCollection<DatabaseInfo> databases = new ObservableCollection<DatabaseInfo>();

        /// <summary>
        /// Gets the databases.
        /// </summary>
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

        /// <summary>
        /// Adds a database node to the list.
        /// </summary>
        /// <param name="dbInfo">The database information.</param>
        public void addDatabase(DatabaseInfo dbInfo)
        {
            this.databases.Add(dbInfo);
            dbHandler.InsertDatabaseNode(dbInfo);
            new LogHandler().LogMessage(LogHandler.MessageType.INFO, string.Format("The database {0} is now ready for backup", dbInfo.DatabaseName));
        }
    }
}
