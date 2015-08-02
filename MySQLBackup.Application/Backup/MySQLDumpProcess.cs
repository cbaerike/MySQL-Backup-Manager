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
        public bool ProcessMySqlDump(Guid databaseId)
        {
            Boolean success = false;
            DatabaseInfo dbInfo = new DatabasesXmlHandler().GetDatabaseNode(databaseId);
            if (dbInfo != null)
            {
                System.Threading.Thread.Sleep(1000);   //Let Application Sleep for 1 second, preventing multiple backup executions of the same database.
                Process process = null;
                if (!isServerDown)
                {
                    string dumpOptions = " ";
                    if (dbInfo.AddUseDatabase)
                    {
                        dumpOptions += "--databases ";
                    };
                    dumpOptions += string.Format(@"""{0}"" -u{1} -p{2} -h{3} -P{4} --add-drop-database --add-drop-table --add-locks --comments --create-options --dump-date --lock-tables"
                       , dbInfo.DatabaseName
                       , dbInfo.User
                       , dbInfo.Password
                       , dbInfo.HostNoPort
                       , dbInfo.Port);
                    if (dbInfo.IncludeRoutines)
                    {
                        dumpOptions += " --routines";
                    }
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = "mysqldump";
                    psi.RedirectStandardInput = false;
                    psi.RedirectStandardOutput = true;
                    psi.RedirectStandardError = true;
                    psi.StandardOutputEncoding = Encoding.UTF8;
                    psi.Arguments = dumpOptions;
                    psi.UseShellExecute = false;
                    psi.CreateNoWindow = true;

                    try
                    {
                        process = Process.Start(psi);
                        //string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();
                        if (!ErrorHandler.HasErrorOccured(error, ref this.isServerDown))
                        {
                            string backupLocation;
                            if (CreateBackupLocation(dbInfo.HostNoPort, dbInfo.DatabaseName, out backupLocation))
                            {
                                this.WriteBackupFile(backupLocation, dbInfo.DatabaseName, process.StandardOutput);
                                success = true;
                                new LogHandler().LogMessage(LogHandler.MessageType.INFO, "Backup created of the database " + dbInfo.DatabaseName);
                            }
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
        /// Creates the backup location.
        /// </summary>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="backupLocation">The backup location.</param>
        /// <returns></returns>
        private bool CreateBackupLocation(string hostName, string databaseName, out string backupLocation)
        {
            bool error = false;
            backupLocation = ConfigurationXmlHandler.GetBackupLocation() + hostName + @"\" + databaseName + @"\";
            if (!Directory.Exists(backupLocation))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(backupLocation));
                }
                catch (Exception ex)
                {
                    error = true;
                    new LogHandler().LogMessage(LogHandler.MessageType.ERROR, "Cannot create directory " + backupLocation + Environment.NewLine + ex.ToString());
                }
            }
            return !error;
        }

        /// <summary>
        /// Write output to a backup file for the specified database.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="output">The output.</param>
        private void WriteBackupFile(string backupLocation, string databaseName, StreamReader output)
        {
            DateTime dateTime = DateTime.Now;
            String filename = backupLocation + string.Format("{0}_{1}-{2}-{3}_{4}.dump", databaseName, dateTime.Day, dateTime.Month, dateTime.Year, dateTime.ToString("HHmm"));
            using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
            {
                writer.AutoFlush = true;
                string tmp;
                while (null != (tmp = output.ReadLine()))
                {
                    writer.WriteLine(tmp);
                }
                writer.Close();
            }
        }
    }
}
