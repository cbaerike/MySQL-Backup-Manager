using MySQLBackup.Application.Config;
using MySQLBackup.Application.Logging;
using MySQLBackup.Application.Model;
using MySQLBackup.Application.Util;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MySQLBackup.Application.Backup
{
    class MySQLDumpProcess
    {
        private bool isServerDown;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySQLDumpProcess"/> class.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        public MySQLDumpProcess()
        {
            this.isServerDown = false;
        }

        /// <summary>
        /// Processes MySQL dump for the given database.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        public bool ProcessMySqlDump(string databaseName)
        {
            Boolean success = false;
            DatabaseInfo dbInfo = new DatabasesHandler().GetDatabaseNode(databaseName);
            if (dbInfo != null)
            {
                System.Threading.Thread.Sleep(1000);   //Let Application Sleep for 1 second, preventing multiple backup executions of the same database.
                Process process = null;
                if (!isServerDown)
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = "mysqldump";
                    psi.RedirectStandardInput = false;
                    psi.RedirectStandardOutput = true;
                    psi.RedirectStandardError = true;
                    psi.StandardOutputEncoding = Encoding.UTF8;
                    psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} --add-drop-database --add-drop-table --add-locks --comments --create-options --dump-date --lock-tables --databases ""{3}""", dbInfo.User, dbInfo.Password, dbInfo.Host, dbInfo.DatabaseName);
                    psi.UseShellExecute = false;
                    psi.CreateNoWindow = true;

                    try
                    {
                        process = Process.Start(psi);
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();
                        if (!ErrorHandler.HasErrorOccured(error, ref this.isServerDown))
                        {
                            this.WriteBackupFile(dbInfo.Host, dbInfo.DatabaseName, output);
                            success = true;
                            new LogHandler().LogMessage(LogHandler.MessageType.INFO, "Backup created of the database " + dbInfo.DatabaseName);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.ToLower().Contains("cannot find the file"))
                        {
                            new LogHandler().LogMessage(LogHandler.MessageType.ERROR, @"The system could not find the MySQLDump.exe file. Please add the MySQL \bin path to your system's PATH variable.");
                        }
                        else
                        {
                            new LogHandler().LogMessage(LogHandler.MessageType.ERROR, @"Unknown exception occurred during backup: " + ex.ToString());
                        }
                    }
                }

                if (process != null)
                {
                    process.WaitForExit();
                }
            }
            return success;
        }

        /// <summary>
        /// Write output to a backup file for the specified database.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="output">The output.</param>
        private void WriteBackupFile(string hostName, string databaseName, string output)
        {
            string backupLocation = ConfigurationHandler.GetBackupLocation() + hostName + @"\" + databaseName + @"\";
            if (!Directory.Exists(backupLocation))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(backupLocation));
            }

            DateTime dateTime = DateTime.Now;
            String filename = backupLocation + string.Format("{0}_{1}-{2}-{3}_{4}.dump", databaseName, dateTime.Day, dateTime.Month, dateTime.Year, dateTime.ToString("HHmm"));
            using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
            {
                writer.WriteLine(output);
                writer.Close();
            }
        }
    }
}
