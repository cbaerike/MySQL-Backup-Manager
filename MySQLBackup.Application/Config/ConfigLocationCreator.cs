using MySQLBackup.Application.Util;
using System.IO;
using System.Xml;

namespace MySQLBackup.Application.Config
{
    public class ConfigLocationCreator
    {
        /// <summary>
        /// Creates the configuration locations.
        /// </summary>
        public void CreateConfigLocations()
        {
            BuildCommonApplicationDataLocation();
            BuildConfigurationFiles();
        }

        /// <summary>
        /// Build the directory structure for the configuration and the backup in the common application data location if it doesn't exist.
        /// </summary>
        private void BuildCommonApplicationDataLocation()
        {
            if (!Directory.Exists(Utilities.CONFIGURATION_LOCATION)) { Directory.CreateDirectory(Path.GetDirectoryName(Utilities.CONFIGURATION_LOCATION)); }
            if (!Directory.Exists(Utilities.DEFAULT_BACKUP_LOCATION)) { Directory.CreateDirectory(Path.GetDirectoryName(Utilities.DEFAULT_BACKUP_LOCATION)); }
        }

        /// <summary>
        /// Check if the Configuration.xml and Databases.xml exists in the Configurations location. If they doesn't exists, then create the files.
        /// </summary>
        private void BuildConfigurationFiles()
        {
            if (!File.Exists(Utilities.CONFIGURATION_LOCATION + "Configuration.xml")) { CreateNewConfigurationFile(); }
            if (!File.Exists(Utilities.CONFIGURATION_LOCATION + "Databases.xml")) { CreateNewDatabasesFile(); }
        }

        /// <summary>
        /// Create the Configuration.xml file with default values.
        /// </summary>
        private void CreateNewConfigurationFile()
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
            backupLocationNode.AppendChild(document.CreateTextNode(Utilities.DEFAULT_BACKUP_LOCATION));
            configNode.AppendChild(backupLocationNode);

            //Create DeleteBackupsOlderThan node
            XmlNode deleteBackupsOlderThanNode = document.CreateElement("DeleteBackupsOlderThan");
            deleteBackupsOlderThanNode.AppendChild(document.CreateTextNode("7"));
            configNode.AppendChild(deleteBackupsOlderThanNode);

            document.Save(Utilities.CONFIGURATION_LOCATION + "Configuration.xml");
        }

        /// <summary>
        /// Create the Databases.xml file with default values.
        /// </summary>
        private void CreateNewDatabasesFile()
        {
            XmlDocument document = new XmlDocument();

            //Create declaration
            XmlNode declarationNode = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.AppendChild(declarationNode);

            //Create Databases node
            XmlNode databasesNode = document.CreateElement("Databases");
            document.AppendChild(databasesNode);

            document.Save(Utilities.CONFIGURATION_LOCATION + "Databases.xml");
        }
    }
}
