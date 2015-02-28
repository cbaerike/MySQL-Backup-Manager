using MySQLBackup.Application.Model;

namespace MySQLBackup.Application.Backup
{
    public class BackupHandler
    {
        /// <summary>
        /// Creates a backup of the given database.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        public bool CreateBackup(string databaseName)
        {
            MySQLDumpProcess dumpProcess = new MySQLDumpProcess();
            return dumpProcess.ProcessMySqlDump(databaseName);
        }

        /// <summary>
        /// Restore a specific database, from a backup dump file.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="dumpFilePath">The dump file path.</param>
        /// <param name="dbInfo">The database information.</param>
        public void RestoreDatabase(System.Diagnostics.Process process, string dumpFilePath, DatabaseInfo dbInfo)
        {
            RestoreDatabaseProcess restoreProcess = new RestoreDatabaseProcess(dumpFilePath, dbInfo);
            restoreProcess.RestoreDatabase(process);
        }
    }
}
