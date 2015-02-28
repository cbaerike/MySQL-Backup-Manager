using MySQLBackup.Application.Util;
using System;
using System.IO;
using System.Xml;

namespace MySQLBackup.Application.Config
{
    public class ConfigurationHandler
    {
        /// <summary>
        /// Sets the backup location.
        /// </summary>
        /// <param name="location">The new location.</param>
        public static void SetBackupLocation(string location)
        {
            XmlDocument document = new XmlDocument();
            document.Load(Utilities.CONFIGURATION_LOCATION + "Configuration.xml");
            XmlNode backupLocationNode = document.SelectSingleNode("Configuration/BackupLocation");

            //If location doesn't ends with backslash, then add it before setting the backup location
            backupLocationNode.InnerText = (location.Trim().EndsWith(@"\")) ? location : location + @"\";

            document.Save(Utilities.CONFIGURATION_LOCATION + "Configuration.xml");

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
            document.Load(Utilities.CONFIGURATION_LOCATION + "Configuration.xml");
            XmlNode deleteBackupsOlderThanNode = document.SelectSingleNode("Configuration/DeleteBackupsOlderThan");

            deleteBackupsOlderThanNode.InnerText = Convert.ToString(days);
            document.Save(Utilities.CONFIGURATION_LOCATION + "Configuration.xml");
        }

        /// <summary>
        /// Gets the backup location.
        /// </summary>
        /// <returns></returns>
        public static string GetBackupLocation()
        {
            XmlDocument document = new XmlDocument();
            document.Load(Utilities.CONFIGURATION_LOCATION + "Configuration.xml");
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
            document.Load(Utilities.CONFIGURATION_LOCATION + "Configuration.xml");
            XmlNode deleteBackupsOlderThanNode = document.SelectSingleNode("Configuration/DeleteBackupsOlderThan");
            return Convert.ToInt32(deleteBackupsOlderThanNode.InnerText);
        }
    }
}
