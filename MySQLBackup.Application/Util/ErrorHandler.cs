using MySQLBackup.Application.Logging;

namespace MySQLBackup.Application.Util
{
    internal class ErrorHandler
    {
        /// <summary>
        /// Find out if an error has occured during the backup dump. Returns true if error has occured
        /// </summary>
        /// <param name="errorOutput">The output to search for errors</param>
        /// <param name="isServerDown">If the error indicates that the MySQL server is down, this will return true.</param>
        /// <returns></returns>
        public static bool HasErrorOccured(string errorOutput, ref bool isServerDown)
        {
            LogHandler logHandler = new LogHandler();
            bool errorOccured = false;

            //Can't find database error
            if (errorOutput.Contains("Got error: 1049"))
            {
                logHandler.LogMessage(LogHandler.MessageType.ERROR, errorOutput.Substring(errorOutput.IndexOf("Got error: 1049")));
                errorOccured = true;
            }
            //Can't find host error
            else if (errorOutput.Contains("Got error: 2005"))
            {
                logHandler.LogMessage(LogHandler.MessageType.ERROR, errorOutput.Substring(errorOutput.IndexOf("Got error: 2005")));
                errorOccured = true;
            }
            //Wrong user/password error
            else if (errorOutput.Contains("Got error: 1045"))
            {
                logHandler.LogMessage(LogHandler.MessageType.ERROR, errorOutput.Substring(errorOutput.IndexOf("Got error: 1045")));
                errorOccured = true;
            }
            //Can't connect to MySQL (probably is server down)
            else if (errorOutput.Contains("Got error: 2003"))
            {
                logHandler.LogMessage(LogHandler.MessageType.ERROR, errorOutput.Substring(errorOutput.IndexOf("Got error: 2003")).TrimEnd('\r', '\n'));
                isServerDown = true;
                errorOccured = true;
            }
            //unknown error
            else if (errorOutput.Contains("Got error: "))
            {
                logHandler.LogMessage(LogHandler.MessageType.ERROR, errorOutput.Substring(errorOutput.IndexOf("Got error: ")));
                errorOccured = true;
            }

            return errorOccured;
        }
    }
}
