using MySQLBackup.Application.Logging;
using System;

namespace MySQLBackup.Application.Model
{
    public class DatabaseInfo
    {
        private int startTimeHour;
        private int startTimeMinute;

        public string Host { get; set; }
        public string HostNoPort { 
            get 
            {
                if (string.IsNullOrEmpty(Host)) return Host;

                int portPos = Host.IndexOf(":");

                if (portPos != -1 && portPos < Host.Length - 1)
                    return Host.Substring(0, portPos);
                else
                    return Host;
            }
        }
        public int Port
        {
            get
            {
                if (string.IsNullOrEmpty(Host)) return 0;
                
                int portPos = Host.IndexOf(":");

                if (portPos != -1 && portPos < Host.Length - 1)
                {
                    try
                    {
                       return int.Parse(Host.Substring(portPos + 1));
                    }
                    catch (FormatException)
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
        public string User { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
        public TimeSpan StartTime { get { return new TimeSpan(startTimeHour, startTimeMinute, 0); } }

        public int StartTimeHour
        {
            set
            {
                startTimeHour = (value < 0 || value > 23) ? 0 : value;
            }
        }

        public int StartTimeMinute
        {
            set
            {
                startTimeMinute = (value < 0 || value > 59) ? 0 : value;
            }
        }

        public override string ToString()
        {
            return DatabaseName;
        }
    }
}
