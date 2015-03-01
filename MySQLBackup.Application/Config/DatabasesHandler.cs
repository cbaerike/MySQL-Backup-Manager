using MySQLBackup.Application.Model;
using MySQLBackup.Application.Util;
using System;
using System.Collections.Generic;
using System.Xml;

namespace MySQLBackup.Application.Config
{
    public class DatabasesHandler
    {
        /// <summary>
        /// Inserts the database node.
        /// </summary>
        /// <param name="databaseInfo">The database information.</param>
        public void InsertDatabaseNode(DatabaseInfo databaseInfo)
        {
            XmlDocument document = new XmlDocument();
            document.Load(ConfigurationHandler.DB_CONFIG_FILE);

            //Create the database node
            XmlNode databaseNode = document.CreateNode(XmlNodeType.Element, "Database", null);
            XmlAttribute databaseNameAttr = document.CreateAttribute("Name");
            databaseNameAttr.InnerText = databaseInfo.DatabaseName.ToLower();
            databaseNode.Attributes.Append(databaseNameAttr);

            //Create the Host node
            XmlNode hostNode = document.CreateElement("Host");
            hostNode.InnerText = databaseInfo.Host;
            databaseNode.AppendChild(hostNode);

            //Create the User node
            XmlNode userNode = document.CreateElement("User");
            userNode.InnerText = databaseInfo.User;
            databaseNode.AppendChild(userNode);

            //Create the Password Node
            XmlNode passwordNode = document.CreateElement("Password");
            passwordNode.InnerText = EncryptionHelper.Encrypt(databaseInfo.Password);
            databaseNode.AppendChild(passwordNode);

            //Create the Backup Settings Node
            XmlNode backupSettingsNode = document.CreateNode(XmlNodeType.Element, "BackupSettings", null);

            //Create the Start Time Node
            XmlNode startTimeNode = document.CreateElement("StartTime");
            startTimeNode.InnerText = databaseInfo.StartTime.ToString();
            backupSettingsNode.AppendChild(startTimeNode);

            //Append backup Settings Node to database node
            databaseNode.AppendChild(backupSettingsNode);

            //Append the database node to the colections element Databases.
            document.DocumentElement.AppendChild(databaseNode);

            //Save the Databases file
            document.Save(ConfigurationHandler.DB_CONFIG_FILE);
        }

        /// <summary>
        /// Remove the given database node.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        public void RemoveDatabaseNode(string databaseName)
        {
            XmlDocument document = new XmlDocument();
            document.Load(ConfigurationHandler.DB_CONFIG_FILE);

            //Fetch the database node if present and remove it
            XmlNode databaseNode = document.SelectSingleNode("Databases/Database[@Name='" + databaseName.ToLower() + "']");
            if (databaseNode != null)
            {
                databaseNode.ParentNode.RemoveChild(databaseNode);
            }

            document.Save(ConfigurationHandler.DB_CONFIG_FILE);
        }

        /// <summary>
        /// Gets the specified database node.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <returns></returns>
        public DatabaseInfo GetDatabaseNode(string databaseName)
        {
            DatabaseInfo dbInfo = null;

            XmlDocument document = new XmlDocument();
            document.Load(ConfigurationHandler.DB_CONFIG_FILE);

            //Fetch the database node if present
            XmlNode databaseNode = document.SelectSingleNode("Databases/Database[@Name='" + databaseName.ToLower() + "']");

            if (databaseNode != null)
            {
                dbInfo = new DatabaseInfo();
                dbInfo.DatabaseName = databaseName.ToLower();
                dbInfo.Host = databaseNode["Host"].InnerText;
                dbInfo.User = databaseNode["User"].InnerText;
                dbInfo.Password = EncryptionHelper.Decrypt(databaseNode["Password"].InnerText);

                XmlNode backupSettingsNode = databaseNode["BackupSettings"];

                string startTime = backupSettingsNode["StartTime"].InnerText;
                string[] timeSplit = startTime.Split(':');
                dbInfo.StartTimeHour = Convert.ToInt32(timeSplit[0]);
                dbInfo.StartTimeMinute = Convert.ToInt32(timeSplit[1]);
            }

            return dbInfo;
        }

        /// <summary>
        /// Gets all database nodes.
        /// </summary>
        /// <returns></returns>
        public List<DatabaseInfo> GetAllDatabaseNodes()
        {
            List<DatabaseInfo> databaseList = new List<DatabaseInfo>();

            XmlDocument document = new XmlDocument();
            document.Load(ConfigurationHandler.DB_CONFIG_FILE);

            XmlNodeList databaseNodeList = document.SelectNodes("Databases/Database");
            foreach (XmlNode node in databaseNodeList)
            {
                DatabaseInfo dbInfo = new DatabaseInfo();
                dbInfo.DatabaseName = node.Attributes["Name"].Value;
                dbInfo.Host = node["Host"].InnerText;
                dbInfo.User = node["User"].InnerText;
                dbInfo.Password = EncryptionHelper.Decrypt(node["Password"].InnerText);

                XmlNode backupSettingsNode = node["BackupSettings"];

                string startTime = backupSettingsNode["StartTime"].InnerText;
                string[] timeSplit = startTime.Split(':');
                dbInfo.StartTimeHour = Convert.ToInt32(timeSplit[0]);
                dbInfo.StartTimeMinute = Convert.ToInt32(timeSplit[1]);

                databaseList.Add(dbInfo);
            }

            return databaseList;
        }

        /// <summary>
        /// Updates the database node.
        /// </summary>
        /// <param name="dbInfo">The database information.</param>
        public void UpdateDatabaseNode(DatabaseInfo dbInfo)
        {
            this.RemoveDatabaseNode(dbInfo.DatabaseName);
            this.InsertDatabaseNode(dbInfo);
        }
    }
}
