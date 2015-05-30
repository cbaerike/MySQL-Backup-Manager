using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySQLBackup.Application.Config;
using MySQLBackup.Application.Logging;
using MySQLBackup.Application.Model;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace MySQLBackup.ApplicationTest
{
    [TestClass]
    public class MySQLBackupApplicationUnitTest
    {
        [TestMethod]
        public void ModifyBackupLocationTest()
        {
            ConfigurationXmlHandler.SetBackupLocation(@"C:\MyTestBackupLocation");

            XmlDocument document = new XmlDocument();
            document.Load(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Configuration\Configuration.xml");
            XmlNode backupLocationNode = document.SelectSingleNode("Configuration/BackupLocation");

            Assert.AreEqual(@"C:\MyTestBackupLocation\", backupLocationNode.InnerText);

            //Delete the test directory
            Directory.Delete(@"C:\MyTestBackupLocation\");

            ConfigurationXmlHandler.SetBackupLocation(@"C:\ProgramData\MySQLBackup\Backup\");
        }

        [TestMethod]
        public void ModifyDeleteBackupsOlderThanDaysTest()
        {
            ConfigurationXmlHandler.SetDeleteBackupsOlderThanDays(14);

            XmlDocument document = new XmlDocument();
            document.Load(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Configuration\Configuration.xml");
            XmlNode deleteBackupsOlderThanNode = document.SelectSingleNode("Configuration/DeleteBackupsOlderThan");

            Assert.AreEqual("14", deleteBackupsOlderThanNode.InnerText);
            ConfigurationXmlHandler.SetDeleteBackupsOlderThanDays(7);
        }

        [TestMethod]
        public void RetrieveBackupLocationTest()
        {
            string backupLocation = ConfigurationXmlHandler.GetBackupLocation();
            Assert.AreEqual(@"C:\ProgramData\MySQLBackup\Backup\", backupLocation);
        }

        [TestMethod]
        public void RetrieveDeleteBackupOlderThanDaysTest()
        {
            int days = ConfigurationXmlHandler.GetDeleteBackupsOlderThanDays();
            Assert.AreEqual(7, days);
        }

        [TestMethod]
        public void InsertNewDatabaseNodeToDatabasesXMLFileTest()
        {
            DatabasesXmlHandler dbHandler = new DatabasesXmlHandler();
            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.ID = Guid.NewGuid();
            dbInfo.Host = "localhost";
            dbInfo.User = "test";
            dbInfo.Password = "secret";
            dbInfo.DatabaseName = "test_database";
            dbInfo.StartTimeHour = 4;
            dbInfo.StartTimeMinute = 30;

            dbHandler.InsertDatabaseNode(dbInfo);

            XElement document = XElement.Load(ConfigurationHandler.DB_CONFIG_FILE);
            var databaseNode = document
                .Elements("Database")
                .FirstOrDefault(x => x.Attribute("ID").Value == dbInfo.ID.ToString());
            string databaseName = databaseNode.Element("Name").Value;

            Assert.AreEqual("test_database", databaseName);

            //remove the database node we just created
            dbHandler.RemoveDatabaseNode(dbInfo.ID);
        }

        [TestMethod]
        public void RemoveSpecificDatabaseNodeTest()
        {
            DatabasesXmlHandler dbHandler = new DatabasesXmlHandler();
            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.ID = Guid.NewGuid();
            dbInfo.Host = "localhost";
            dbInfo.User = "test";
            dbInfo.Password = "secret";
            dbInfo.DatabaseName = "TestDatabase";
            dbInfo.StartTimeHour = 4;
            dbInfo.StartTimeMinute = 30;

            dbHandler.InsertDatabaseNode(dbInfo);
            dbHandler.RemoveDatabaseNode(dbInfo.ID);

            XElement document = XElement.Load(ConfigurationHandler.DB_CONFIG_FILE);
            var databaseNode = document
                .Elements("Database")
                .FirstOrDefault(x => x.Attribute("ID").Value == dbInfo.ID.ToString());

            Assert.IsNull(databaseNode);
        }

        [TestMethod]
        public void RetrieveSpecificDatabaseNodeTest()
        {
            DatabasesXmlHandler dbHandler = new DatabasesXmlHandler();
            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.ID = Guid.NewGuid();
            dbInfo.Host = "localhost";
            dbInfo.User = "test";
            dbInfo.Password = "secret";
            dbInfo.DatabaseName = "TestDatabase";
            dbInfo.StartTimeHour = 4;
            dbInfo.StartTimeMinute = 30;

            dbHandler.InsertDatabaseNode(dbInfo);

            DatabaseInfo dbInfo2 = dbHandler.GetDatabaseNode(dbInfo.ID);

            Assert.AreEqual("TestDatabase", dbInfo2.DatabaseName);

            dbHandler.RemoveDatabaseNode(dbInfo.ID);
        }

        [TestMethod]
        public void UpdateSpecificDatabaseNodeTest()
        {
            DatabasesXmlHandler dbHandler = new DatabasesXmlHandler();
            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.ID = Guid.NewGuid();
            dbInfo.Host = "localhost";
            dbInfo.User = "test";
            dbInfo.Password = "secret";
            dbInfo.DatabaseName = "TestDatabase";
            dbInfo.StartTimeHour = 4;
            dbInfo.StartTimeMinute = 30;

            dbHandler.InsertDatabaseNode(dbInfo);

            //modify the dbInfo start time
            dbInfo.StartTimeHour = 22;
            dbInfo.StartTimeMinute = 59;

            dbHandler.UpdateDatabaseNode(dbInfo);

            dbInfo = dbHandler.GetDatabaseNode(dbInfo.ID)
;
            Assert.AreEqual("22:59:00", dbInfo.StartTime.ToString());

            dbHandler.RemoveDatabaseNode(dbInfo.ID);
        }

        [TestMethod]
        public void LogMessageAtDefaultLocationTest()
        {
            LogHandler logHandler = new LogHandler();
            logHandler.LogMessage(LogHandler.MessageType.INFO, "This is a test log message");

            StreamReader reader = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Log.txt");
            string output = reader.ReadLine();
            reader.Close();

            Assert.IsTrue(File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Log.txt"));
            Assert.IsNotNull(logHandler.GetLogText());

            //Delete test Log file
            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Log.txt");

            logHandler = null;
        }

        [TestMethod]
        public void LogMessageAtCustomLocationTest()
        {
            string logfile = @"C:\LogMessageTest\CustomLog.log";
            LogHandler logHandler = new LogHandler();
            logHandler.LogFile = logfile;
            logHandler.LogMessage(LogHandler.MessageType.ERROR, "This is a test log message");

            Assert.IsTrue(File.Exists(logfile));

            StreamReader reader = new StreamReader(logfile);
            string output = reader.ReadLine();
            reader.Close();

            Assert.IsTrue(output.Length > 0);

            //Delete test Log file and custom directory
            File.Delete(logfile);
            Directory.Delete(Path.GetDirectoryName(logfile));
        }

        [TestMethod]
        public void ClearLogAtDefaultLocationTest()
        {
            LogHandler logHandler = new LogHandler();
            logHandler.ClearLog();

            StreamReader reader = new StreamReader(ConfigurationHandler.ROOT_LOCATION + "Log.txt");
            string output = reader.ReadLine();
            reader.Close();

            Assert.IsNull(output);

            logHandler = null;
        }

        [TestMethod]
        public void ClearLogAtCustomLocationTest()
        {
            string logfile = @"C:\LogMessageTest\CustomLog.log";
            LogHandler logHandler = new LogHandler();
            logHandler.LogFile = logfile;
            logHandler.LogMessage(LogHandler.MessageType.ERROR, "This is a test log message");
            logHandler.ClearLog();

            StreamReader reader = new StreamReader(logfile);
            string output = reader.ReadLine();
            reader.Close();

            Assert.IsNull(output);

            //Delete test Log file and custom directory
            File.Delete(logfile);
            Directory.Delete(Path.GetDirectoryName(logfile));
            logHandler = null;
        }

        [TestMethod]
        public void GetLogTextCustomDefaultLocation()
        {
            string logfile = @"C:\LogMessageTest\CustomLog.log";
            LogHandler logHandler = new LogHandler();
            logHandler.LogFile = logfile;
            logHandler.LogMessage(LogHandler.MessageType.ERROR, "This is a test log message");

            Assert.IsNotNull(logHandler.GetLogText());

            //Delete test Log file and custom directory
            File.Delete(logfile);
            Directory.Delete(Path.GetDirectoryName(logfile));
            logHandler = null;
        }

        [TestMethod]
        public void RetrieveAllDatabaseNodesTest()
        {
            DatabasesXmlHandler dbHandler = new DatabasesXmlHandler();
            DatabaseInfo dbInfo = new DatabaseInfo();
            Guid tmpGuid = Guid.NewGuid();
            dbInfo.ID = tmpGuid;
            dbInfo.Host = "localhost";
            dbInfo.User = "test";
            dbInfo.Password = "secret";
            dbInfo.DatabaseName = "TestDatabase";
            dbInfo.StartTimeHour = 4;
            dbInfo.StartTimeMinute = 30;


            dbHandler.InsertDatabaseNode(dbInfo);
            dbInfo.ID = Guid.NewGuid();
            dbInfo.DatabaseName = "NewDatabase";
            dbHandler.InsertDatabaseNode(dbInfo);

            Assert.IsTrue(1 < dbHandler.GetAllDatabaseNodes().Count);

            dbHandler.RemoveDatabaseNode(dbInfo.ID);
            dbHandler.RemoveDatabaseNode(tmpGuid);
        }

        [TestMethod]
        public void GetMySQLBinPathTest()
        {
            String binLocation = ConfigurationHandler.RetrieveMySQLInstallationBinPath();
            Assert.IsTrue(!String.IsNullOrEmpty(binLocation) && binLocation.ToLower().Contains("bin"));
        }
    }
}
