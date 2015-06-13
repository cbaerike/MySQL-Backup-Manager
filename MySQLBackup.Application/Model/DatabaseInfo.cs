using MySQLBackup.Application.Logging;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MySQLBackup.Application.Model
{
    /// <summary>
    /// Holds conifguration information about a database backup
    /// </summary>
    public class DatabaseInfo : INotifyPropertyChanged
    {
        private Guid id;
        private string host;
        private string user;
        private string password;
        private string databaseName;
        private TimeSpan startTime;

        /// <summary>
        /// Gets or sets the database identifier.
        /// </summary>
        public Guid ID
        {
            get { return this.id; }
            set
            {
                if (value != this.id)
                {
                    this.id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the host, including the port number if one is set.
        /// </summary>
        public string Host
        {
            get { return this.host; }
            set
            {
                if (value != this.host)
                {
                    this.host = value;
                    NotifyPropertyChanged();
                }
            }
        }

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
        public string User
        {
            get { return this.user; }
            set
            {
                if (value != this.user)
                {
                    this.user = value;
                    NotifyPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password
        {
            get { return this.password; }
            set
            {
                if (value != this.password)
                {
                    this.password = value;
                    NotifyPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        public string DatabaseName
        {
            get { return this.databaseName; }
            set
            {
                if (value != this.databaseName)
                {
                    this.databaseName = value;
                    NotifyPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public TimeSpan StartTime
        {
            get { return this.startTime; }
            set
            {
                if (value != this.startTime)
                {
                    this.startTime = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the start time string.
        /// </summary>
        public string StartTimeString
        {
            get { return this.StartTime.ToString(@"{0:hh\:mm}"); }
            set
            {
                TimeSpan tmp;
                if (TimeSpan.TryParse(value, out tmp))
                {
                    this.StartTime = tmp;
                }
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

        #region IPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This method is called by the Set accessor of each property. 
        /// The CallerMemberName attribute that is applied to the optional propertyName 
        /// parameter causes the property name of the caller to be substituted as an argument. 
        /// </summary>
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
