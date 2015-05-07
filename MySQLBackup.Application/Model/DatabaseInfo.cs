using MySQLBackup.Application.Logging;
using System;

namespace MySQLBackup.Application.Model
{
    /// <summary>
    /// Holds conifguration information about a database backup
    /// </summary>
    public class DatabaseInfo
    {
        private int startTimeHour;
        private int startTimeMinute;

        /// <summary>
        /// Gets or sets the host, including the port number if one is set.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets the host, without the port number.
        /// </summary>
        public string HostNoPort
        {
            get
            {
                if (string.IsNullOrEmpty(Host)) return Host;

                int portPos = Host.IndexOf(":");

                if (portPos != -1 && portPos < Host.Length - 1)
                {
                    return Host.Substring(0, portPos);
                }
                else
                {
                    return Host;
                }
            }
        }

        /// <summary>
        /// Gets the port.
        /// </summary>
        public int Port
        {
            get
            {
                if (string.IsNullOrEmpty(Host)) return 0;

                int portPos = Host.IndexOf(":");

                if (portPos != -1 && portPos < Host.Length - 1)
                {
                    int port;
                    if (Int32.TryParse(Host.Substring(portPos + 1), out port))
                    {
                        return port;
                    }
                    else
                    {
                        new LogHandler().LogMessage(LogHandler.MessageType.ERROR, string.Format("Format error of port for host {0}", Host));
                        return 3306;
                    }
                }
                else
                {
                    return 3306;
                }
            }
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets the start time.
        /// </summary>
        public TimeSpan StartTime { get { return new TimeSpan(startTimeHour, startTimeMinute, 0); } }

        /// <summary>
        /// Sets the start time hour.
        /// </summary>
        public int StartTimeHour
        {
            set
            {
                startTimeHour = (value < 0 || value > 23) ? 0 : value;
            }
        }

        /// <summary>
        /// Sets the start time minute.
        /// </summary>
        public int StartTimeMinute
        {
            set
            {
                startTimeMinute = (value < 0 || value > 59) ? 0 : value;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that contains the database name.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that contains the database name.
        /// </returns>
        public override string ToString()
        {
            return DatabaseName;
        }
    }
}
