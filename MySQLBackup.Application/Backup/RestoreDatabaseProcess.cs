using MySQLBackup.Application.Config;
using MySQLBackup.Application.Logging;
using MySQLBackup.Application.Model;
using MySQLBackup.Application.Util;
using System;
using System.Diagnostics;
using System.Text;

namespace MySQLBackup.Application.Backup
{
    /// <summary>
    /// Takes care of the database restore process.
    /// </summary>
    class RestoreDatabaseProcess
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestoreDatabaseProcess"/> class.
        /// </summary>
        public RestoreDatabaseProcess()
        {
        }

        /// <summary>
        /// Restores the database.
        /// </summary>
        /// <param name="dumpFilePath">The dump file path.</param>
        /// <param name="dbInfo">The database information.</param>
        public void RestoreDatabase(string dumpFilePath, Guid databaseId)
        {
            Process process = null;
            DatabaseInfo dbInfo = new DatabasesXmlHandler().GetDatabaseNode(databaseId);
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "mysql";
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.StandardOutputEncoding = Encoding.UTF8;
            psi.Arguments = string.Format(@"-u {0} -p{1} -h {2} -P{3}", dbInfo.User, dbInfo.Password, dbInfo.HostNoPort, dbInfo.Port);
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            process = Process.Start(psi);

            using (var stdin = new System.IO.StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8))
            using (var reader = System.IO.File.OpenText(@dumpFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    stdin.WriteLine(line);
                }
                stdin.Close();
            }

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            bool isServerDown = false;

            if (!ErrorHandler.HasErrorOccured(error, ref isServerDown))
            {
                new LogHandler().LogMessage(LogHandler.MessageType.INFO, "The database " + dbInfo.DatabaseName + ", has been restored, from this backup dump file '" + dumpFilePath + "'");
            }

            if (process != null)
            {
                process.WaitForExit();
            }
        }
    }
}
