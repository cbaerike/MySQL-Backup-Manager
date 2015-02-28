using MySQLBackup.Application.Model;
using MySQLBackup.Application.Logging;
using MySQLBackup.Application.Util;
using System.Diagnostics;
using System.Text;

namespace MySQLBackup.Application.Backup
{
    class RestoreDatabaseProcess
    {
        private string dumpFilePath = "";
        private DatabaseInfo dbInfo = null;

        public RestoreDatabaseProcess(string dumpFilePath, DatabaseInfo dbInfo)
        {
            this.dumpFilePath = dumpFilePath;
            this.dbInfo = dbInfo;
        }

        /**
         * Restore a database, from a specific backup dump file.
         */
        public void RestoreDatabase(Process process)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "mysql";
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.StandardOutputEncoding = Encoding.UTF8;
            psi.Arguments = string.Format(@"-u {0} -p{1} -h {2}", dbInfo.User, dbInfo.Password, dbInfo.Host);
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
