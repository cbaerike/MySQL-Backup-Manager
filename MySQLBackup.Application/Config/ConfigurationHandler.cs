using Microsoft.Win32;
using System;
using System.IO;
using System.Xml;

namespace MySQLBackup.Application.Config
{
    public class ConfigurationHandler
    {
        /// <summary>
        /// The Backup root location.
        /// </summary>
        public static readonly string ROOT_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\";

        /// <summary>
        /// The configuration file location.
        /// </summary>
        public static readonly string CONFIGURATION_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Configuration\";

        /// <summary>
        /// The database config file - this holds the scheduled backups.
        /// </summary>
        public static readonly string DB_CONFIG_FILE = CONFIGURATION_LOCATION + "Databases.xml";

        /// <summary>
        /// The application config file - this holds general application config settings.
        /// </summary>
        public static readonly string APP_CONFIG_FILE = CONFIGURATION_LOCATION + "Configuration.xml";

        /// <summary>
        /// The default backup location.
        /// </summary>
        public static readonly string DEFAULT_BACKUP_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Backup\";

        /// <summary>
        /// Sets the backup location.
        /// </summary>
        /// <param name="location">The new location.</param>
        public static void SetBackupLocation(string location)
        {
            XmlDocument document = new XmlDocument();
            document.Load(ConfigurationHandler.APP_CONFIG_FILE);
            XmlNode backupLocationNode = document.SelectSingleNode("Configuration/BackupLocation");

            //If location doesn't ends with backslash, then add it before setting the backup location
            backupLocationNode.InnerText = (location.Trim().EndsWith(@"\")) ? location : location + @"\";

            document.Save(ConfigurationHandler.APP_CONFIG_FILE);

            //Create the new directory if it doesn't exist
            if (!Directory.Exists(Path.GetDirectoryName(backupLocationNode.InnerText))) { 
                Directory.CreateDirectory(Path.GetDirectoryName(backupLocationNode.InnerText)); }
        }

        /// <summary>
        /// Sets the "delete backups older than" days. If set to 0, no backups will be deleted.
        /// </summary>
        /// <param name="days">The days.</param>
        public static void SetDeleteBackupsOlderThanDays(int days)
        {
            XmlDocument document = new XmlDocument();
            document.Load(ConfigurationHandler.APP_CONFIG_FILE);
            XmlNode deleteBackupsOlderThanNode = document.SelectSingleNode("Configuration/DeleteBackupsOlderThan");

            deleteBackupsOlderThanNode.InnerText = Convert.ToString(days);
            document.Save(ConfigurationHandler.APP_CONFIG_FILE);
        }

        /// <summary>
        /// Gets the backup location.
        /// </summary>
        /// <returns></returns>
        public static string GetBackupLocation()
        {
            XmlDocument document = new XmlDocument();
            document.Load(ConfigurationHandler.APP_CONFIG_FILE);
            XmlNode backupLocationNode = document.SelectSingleNode("Configuration/BackupLocation");
            return backupLocationNode.InnerText;
        }

        /// <summary>
        /// Gets the "delete backups older than" days.
        /// </summary>
        /// <returns></returns>
        public static int GetDeleteBackupsOlderThanDays()
        {
            XmlDocument document = new XmlDocument();
            document.Load(ConfigurationHandler.APP_CONFIG_FILE);
            XmlNode deleteBackupsOlderThanNode = document.SelectSingleNode("Configuration/DeleteBackupsOlderThan");
            return Convert.ToInt32(deleteBackupsOlderThanNode.InnerText);
        }

        /// <summary>
        /// Lookup the MySQL Installation Bin path from the registry
        /// </summary>
        /// <returns></returns>
        public static string RetrieveMySQLInstallationBinPath()
        {
            string binLocation = null;
            //64-bit MySQL
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\MySQL AB");
            if (null == registryKey)
            {
                //32-bit MySQL on 64-bit OS
                registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\MySQL AB");
            }
            if (null == registryKey)
            {
                //64-bit MariaDB
                registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Monty Program AB");
            }
            if (null == registryKey)
            {
                //32-bit MariaDB on 64-bit OS
                registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Monty Program AB");
            }
            if (null != registryKey)
            {
                foreach (string subkey in registryKey.GetSubKeyNames())
                {
                    RegistryKey myKey = registryKey.OpenSubKey(subkey);
                    //MySQL Install Dir
                    object location = myKey.GetValue("Location");
                    if (null != location && !string.IsNullOrEmpty(location.ToString()))
                    {
                        binLocation = location.ToString();
                        break;
                    }
                    //MariaDB Install Dir
                    location = myKey.GetValue("INSTALLDIR").ToString();
                    if (null != location && !string.IsNullOrEmpty(location.ToString()))
                    {
                        binLocation = location.ToString();
                        break;
                    }
                }
            }
            //The directory path may not end with a trailing slash. 
            //--> delete any trailing slashes if they're there, then add the \bin\ suffix.
            if (!string.IsNullOrEmpty(binLocation))
            {
                binLocation.TrimEnd('\\');
                return binLocation + @"\bin\";
            }
            return null;
        }
    }
}
