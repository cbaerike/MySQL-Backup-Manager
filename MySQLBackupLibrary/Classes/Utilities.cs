using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLBackupLibrary.Classes
{
    class Utilities
    {
        public static readonly string ROOT_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\";
        public static readonly string CONFIGURATION_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Configuration\";
        public static readonly string DEFAULT_BACKUP_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Backup\";

        /**
         * Lookup the MySQL Installation Bin path from the registry
         */
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
