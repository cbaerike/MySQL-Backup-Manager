using MySQLBackup.Application.Model;
using System;

namespace MySQLBackup.Application.Backup
{
    public class BackupHandler
    {
        /// <summary>
        /// Creates a backup of the given database.
        /// </summary>
        /// <param name="databaseId">The database identifier.</param>
        /// <returns></returns>
        public bool CreateBackup(Guid databaseId)
        {
            MySQLDumpProcess dumpProcess = new MySQLDumpProcess();
            return dumpProcess.ProcessMySqlDump(databaseId);
        }

        /// <summary>
        /// Restore a specific database, from a backup dump file.
        /// </summary>
        /// <param name="dumpFilePath">The dump file path.</param>
        /// <param name="databaseId">The database identifier.</param>
        public void RestoreDatabase(string dumpFilePath, Guid databaseId)
        {
            RestoreDatabaseProcess restoreProcess = new RestoreDatabaseProcess();
            restoreProcess.RestoreDatabase(dumpFilePath, databaseId);
        }
    }
}
