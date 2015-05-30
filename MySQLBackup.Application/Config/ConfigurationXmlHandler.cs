using System;
using System.IO;
using System.Xml;

namespace MySQLBackup.Application.Config
{
    /// <summary>
    /// Takes care of XML manipulations in the mail config file.
    /// </summary>
    public class ConfigurationXmlHandler
    {
        /// <summary>
        /// Create the Configuration.xml file with default values.
        /// </summary>
        internal static void CreateNewConfigurationFile()
        {
            XmlDocument document = new XmlDocument();
            //Create declaration
            XmlNode declarationNode = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.AppendChild(declarationNode);
            //Create configuration node
            XmlNode configNode = document.CreateElement("Configuration");
            document.AppendChild(configNode);
            //Create BackupLocation node
            XmlNode backupLocationNode = document.CreateElement("BackupLocation");
            backupLocationNode.AppendChild(document.CreateTextNode(ConfigurationHandler.DEFAULT_BACKUP_LOCATION));
            configNode.AppendChild(backupLocationNode);
            //Create DeleteBackupsOlderThan node
            XmlNode deleteBackupsOlderThanNode = document.CreateElement("DeleteBackupsOlderThan");
            deleteBackupsOlderThanNode.AppendChild(document.CreateTextNode("7"));
            configNode.AppendChild(deleteBackupsOlderThanNode);
            document.Save(ConfigurationHandler.APP_CONFIG_FILE);
        }

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
            if (!Directory.Exists(Path.GetDirectoryName(backupLocationNode.InnerText)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(backupLocationNode.InnerText));
            }
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

    }
}
