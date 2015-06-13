using MySQLBackup.Application.Model;
using MySQLBackup.Application.Util;
using System;
using System.Collections.Generic;
using System.IO;
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
        private const string currentFileVersion = "1.1";

        #region File creation and update
        /// <summary>
        /// Create the Databases.xml file with default values.
        /// </summary>
        internal void CreateNewDatabasesFile()
        {
            XElement document = new XElement("Databases", new XAttribute("Version", currentFileVersion));
            document.Save(ConfigurationHandler.DB_CONFIG_FILE);
        }

        /// <summary>
        /// Updates the databases file to the latest version.
        /// </summary>
        internal void UpdateDatabasesFileVersion()
        {
            try
            {
                XElement document = XElement.Load(ConfigurationHandler.DB_CONFIG_FILE);
                string fileVersion = (null == document.Attribute("Version") ? "1.0" : document.Attribute("Version").Value);
                switch (fileVersion)
                {
                    case "1.0":
                        //Make a backup copy
                        File.Copy(ConfigurationHandler.DB_CONFIG_FILE, ConfigurationHandler.DB_CONFIG_FILE.Replace(".xml", ".1.0.xml"), true);
                        //Read the old file contents into memory
                        List<DatabaseInfo> databaseList = new List<DatabaseInfo>();
                        var databaseNodeList = document.Elements("Database");
                        foreach (var databaseNode in databaseNodeList)
                        {
                            DatabaseInfo dbInfo = new DatabaseInfo();
                            dbInfo.ID = Guid.NewGuid();
                            dbInfo.DatabaseName = databaseNode.Attribute("Name").Value;
                            dbInfo.Host = databaseNode.Element("Host").Value;
                            dbInfo.User = databaseNode.Element("User").Value;
                            dbInfo.Password = EncryptionHelper.Decrypt(databaseNode.Element("Password").Value);
                            XElement backupSettingsNode = databaseNode.Element("BackupSettings");
                            dbInfo.StartTimeString = backupSettingsNode.Element("StartTime").Value;
                            databaseList.Add(dbInfo);
                        }
                        //Then delete the old file and create a new one.
                        File.Delete(ConfigurationHandler.DB_CONFIG_FILE);
                        CreateNewDatabasesFile();
                        foreach (DatabaseInfo dbInfo in databaseList)
                        {
                            this.InsertDatabaseNode(dbInfo);
                        }
                        break;
                    case "1.1":
                    default://Nothing to do. This is the latest version.
                        break;
                }
            }
            catch (Exception ex)
            {
                Logging.LogHandler logHandler = new Logging.LogHandler();
                logHandler.LogMessage(Logging.LogHandler.MessageType.ERROR, "Error updating database config file: " + ex.ToString());
            }
        }
        #endregion

        #region Database node manipulation
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
                dbInfo.StartTimeString = backupSettingsNode.Element("StartTime").Value;
            }
            return dbInfo;
        }
        #endregion
    }
}
