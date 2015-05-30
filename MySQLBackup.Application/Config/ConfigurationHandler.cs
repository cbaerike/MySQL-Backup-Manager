using Microsoft.Win32;
using System;
using System.IO;

namespace MySQLBackup.Application.Config
{
    /// <summary>
    /// Provides common configuration file methods.
    /// </summary>
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
        public static readonly string DB_CONFIG_FILENAME = "Databases.xml";

        /// <summary>
        /// The database config file (including full path) - this holds the scheduled backups.
        /// </summary>
        public static readonly string DB_CONFIG_FILE = CONFIGURATION_LOCATION + DB_CONFIG_FILENAME;

        /// <summary>
        /// The application config file - this holds general application config settings.
        /// </summary>
        public static readonly string APP_CONFIG_FILE = CONFIGURATION_LOCATION + "Configuration.xml";

        /// <summary>
        /// The default backup location.
        /// </summary>
        public static readonly string DEFAULT_BACKUP_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Backup\";

        /// <summary>
        /// Initializes the config folders and files.
        /// </summary>
        public void InitializeConfigFiles()
        {
            if (!Directory.Exists(ConfigurationHandler.CONFIGURATION_LOCATION)) { Directory.CreateDirectory(Path.GetDirectoryName(ConfigurationHandler.CONFIGURATION_LOCATION)); }
            if (!Directory.Exists(ConfigurationHandler.DEFAULT_BACKUP_LOCATION)) { Directory.CreateDirectory(Path.GetDirectoryName(ConfigurationHandler.DEFAULT_BACKUP_LOCATION)); }
            if (!File.Exists(ConfigurationHandler.APP_CONFIG_FILE)) { ConfigurationXmlHandler.CreateNewConfigurationFile(); }
            DatabasesXmlHandler dbHandler = new DatabasesXmlHandler();
            if (!File.Exists(ConfigurationHandler.DB_CONFIG_FILE)) { dbHandler.CreateNewDatabasesFile(); }
            dbHandler.UpdateDatabasesFileVersion();
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
