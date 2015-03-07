using MySQLBackup.Application.Config;
using System;
using System.IO;

namespace MySQLBackup.Application.Logging
{
    public class LogHandler
    {
        /// <summary>
        /// All available message types.
        /// </summary>
        public enum MessageType
        {
            ERROR,
            WARNING,
            SUCCESS,
            INFO
        };

        private string logFile = ConfigurationHandler.ROOT_LOCATION + "Log.txt";

        /// <summary>
        /// Gets or sets the log location.
        /// </summary>
        /// <value>
        /// The log location.
        /// </value>
        public string LogFile
        {
            get
            {
                return this.logFile;
            }
            set
            {
                //Currently we don't have a way in the UI to set a custom log location. Keeping this available for later.
                this.logFile = value;
                if (!Directory.Exists(Path.GetDirectoryName(logFile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(logFile));
                }
            }
        }


        /// <summary>
        /// Writes a message to the log file.
        /// </summary>
        /// <param name="data">The data.</param>
        public void LogMessage(MessageType messageType, string message)
        {
            try
            {
                StreamWriter writer = File.AppendText(LogFile);
                if (writer != null)
                {
                    writer.WriteLine(string.Format("{0:yyyy-MM-dd HH:mm} - {1} - {2}", DateTime.Now, messageType.ToString(), message));
                }
                writer.Close();
            }
            catch (Exception) { }  //No way to report back any errors. No need to handle them then.
        }

        /// <summary>
        /// Gets the log text.
        /// </summary>
        /// <returns></returns>
        public string GetLogText()
        {
            string result = String.Empty;
            try
            {
                StreamReader reader = new StreamReader(LogFile);
                result = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ex)
            {
                result = ex.ToString() + Environment.NewLine + "Logfile: " + LogFile;
            }
            return result;
        }

        /// <summary>
        /// Clears the log.
        /// </summary>
        public void ClearLog()
        {
            try
            {
                System.IO.File.WriteAllText(LogFile, String.Empty);
            }
            catch (Exception) { }  //No way to report back any errors. No need to handle them then.
        }
    }
}
