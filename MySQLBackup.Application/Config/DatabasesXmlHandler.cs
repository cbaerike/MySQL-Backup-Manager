using MySQLBackup.Application.Model;
using MySQLBackup.Application.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MySQLBackup.Application.Config
{
    /// <summary>
    /// Takes care of XML manipulations in the databases config file.
    /// </summary>
    public class DatabasesXmlHandler
    {
        /// <summary>
        /// The current version of the database config file.
        /// </summary>
        private const string fileVersion = "1.1";

        #region File methods
        /// <summary>
        /// Create the Databases.xml file with default values.
        /// </summary>
        internal static void CreateNewDatabasesFile()
        {
            XElement document = new XElement("Databases", new XAttribute("Version", fileVersion));
            document.Save(ConfigurationHandler.DB_CONFIG_FILE);
        }

        /// <summary>
        /// Updates the databases file to the latest version.
        /// </summary>
        internal static void UpdateDatabasesFileVersion()
        {
#warning TODO!
        }
        #endregion
        /// <summary>
        /// Inserts the database node.
        /// </summary>
        /// <param name="databaseInfo">The database information.</param>
        public void InsertDatabaseNode(DatabaseInfo databaseInfo)
        {
            XElement document = XElement.Load(ConfigurationHandler.DB_CONFIG_FILE);
            XElement newDatabaseNode = new XElement("Database", new XAttribute("ID", databaseInfo.ID),
                new XElement("Name", databaseInfo.DatabaseName),
                new XElement("Host", databaseInfo.Host),
                new XElement("User", databaseInfo.User),
                new XElement("Password", EncryptionHelper.Encrypt(databaseInfo.Password)),
                new XElement("BackupSettings",
                    new XElement("StartTime", databaseInfo.StartTime.ToString())));
            document.Add(newDatabaseNode);
            document.Save(ConfigurationHandler.DB_CONFIG_FILE);
        }

        /// <summary>
        /// Remove the given database node.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        public void RemoveDatabaseNode(Guid databaseId)
        {
            XElement document = XElement.Load(ConfigurationHandler.DB_CONFIG_FILE);
            var databaseNode = document
                .Elements("Database")
                .FirstOrDefault(x => x.Attribute("ID").Value == databaseId.ToString());
            if (null != databaseNode)
            {
                databaseNode.Remove();
            }
            document.Save(ConfigurationHandler.DB_CONFIG_FILE);
        }

        /// <summary>
        /// Gets the specified database node.
        /// </summary>
        /// <param name="databaseId">The database identifier.</param>
        /// <returns></returns>
        public DatabaseInfo GetDatabaseNode(Guid databaseId)
        {
            XElement document = XElement.Load(ConfigurationHandler.DB_CONFIG_FILE);
            var databaseNode = document
                .Elements("Database")
                .FirstOrDefault(x => x.Attribute("ID").Value == databaseId.ToString());
            return ParseDatabaseNode(databaseNode);
        }

        /// <summary>
        /// Gets all database nodes.
        /// </summary>
        /// <returns></returns>
        public List<DatabaseInfo> GetAllDatabaseNodes()
        {
            List<DatabaseInfo> databaseList = new List<DatabaseInfo>();
            XElement document = XElement.Load(ConfigurationHandler.DB_CONFIG_FILE);
            var databaseNodeList = document
                .Elements("Database");
            foreach (var databaseNode in databaseNodeList)
            {
                DatabaseInfo node = ParseDatabaseNode(databaseNode);
                if (null != node)
                {
                    databaseList.Add(node);
                }
            }
            return databaseList;
        }

        /// <summary>
        /// Updates the database node.
        /// </summary>
        /// <param name="dbInfo">The database information.</param>
        public void UpdateDatabaseNode(DatabaseInfo dbInfo)
        {
            this.RemoveDatabaseNode(dbInfo.ID);
            this.InsertDatabaseNode(dbInfo);
        }

        /// <summary>
        /// Parses the database node.
        /// </summary>
        /// <param name="databaseNode">The database node.</param>
        /// <returns></returns>
        private DatabaseInfo ParseDatabaseNode(XElement databaseNode)
        {
            DatabaseInfo dbInfo = null;
            Guid databaseId;
            if (databaseNode != null && Guid.TryParse(databaseNode.Attribute("ID").Value, out databaseId))
            {
                dbInfo = new DatabaseInfo();
                dbInfo.ID = databaseId;
                dbInfo.DatabaseName = databaseNode.Element("Name").Value;
                dbInfo.Host = databaseNode.Element("Host").Value;
                dbInfo.User = databaseNode.Element("User").Value;
                dbInfo.Password = EncryptionHelper.Decrypt(databaseNode.Element("Password").Value);

                XElement backupSettingsNode = databaseNode.Element("BackupSettings");

                string startTime = backupSettingsNode.Element("StartTime").Value;
                string[] timeSplit = startTime.Split(':');
                dbInfo.StartTimeHour = Convert.ToInt32(timeSplit[0]);
                dbInfo.StartTimeMinute = Convert.ToInt32(timeSplit[1]);
            }

            return dbInfo;
        }
    }
}
