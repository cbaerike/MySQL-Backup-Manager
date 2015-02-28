using MySQLBackup.Application.Model;
using MySQLBackup.Application.Util;
using System;
using System.Collections.Generic;
using System.Xml;

namespace MySQLBackup.Application.Config
{
    public class DatabasesHandler
    {
        public void InsertDatabaseNode(DatabaseInfo databaseInfo)
        {
            XmlDocument document = new XmlDocument();
            document.Load(Utilities.CONFIGURATION_LOCATION + "Databases.xml");

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
            document.Save(Utilities.CONFIGURATION_LOCATION + "Databases.xml");
        }

        /**
         * Remove a database node from the Databases.xml file according to the provided database name
         */
        public void RemoveDatabaseNode(string databaseName)
        {
            XmlDocument document = new XmlDocument();
            document.Load(Utilities.CONFIGURATION_LOCATION + "Databases.xml");

            //Fetch the database node if present and remove it
            XmlNode databaseNode = document.SelectSingleNode("Databases/Database[@Name='" + databaseName.ToLower() + "']");
            if (databaseNode != null)
            {
                databaseNode.ParentNode.RemoveChild(databaseNode);
            }

            document.Save(Utilities.CONFIGURATION_LOCATION + "Databases.xml");
        }

        /**
         * Fetch a specific database node, as a DatabaseInfo Object. Null is returned if
         * no matching database node was found
         */
        public DatabaseInfo GetDatabaseNode(string databaseName)
        {
            DatabaseInfo dbInfo = null;

            XmlDocument document = new XmlDocument();
            document.Load(Utilities.CONFIGURATION_LOCATION + "Databases.xml");

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

        public List<DatabaseInfo> GetAllDatabaseNodes()
        {
            List<DatabaseInfo> databaseList = new List<DatabaseInfo>();

            XmlDocument document = new XmlDocument();
            document.Load(Utilities.CONFIGURATION_LOCATION + "Databases.xml");

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

        /**
         * Update an existing database node.
         */
        public void UpdateDatabaseNode(DatabaseInfo dbInfo)
        {
            this.RemoveDatabaseNode(dbInfo.DatabaseName);
            this.InsertDatabaseNode(dbInfo);
        }
    }
}
