using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySQLBackup.Application.Backup;
using System.IO;
using System.Xml;
using MySQLBackup.Application.Logging;
using MySQLBackup.Application.Config;
using MySQLBackup.Application.Model;

namespace MySQLBackup.ApplicationTest
{
    [TestClass]
    public class MySQLBackupApplicationUnitTest
    {
        [TestMethod]
        public void ModifyBackupLocationTest()
        {
            ConfigurationHandler.SetBackupLocation(@"C:\MyTestBackupLocation");

            XmlDocument document = new XmlDocument();
            document.Load(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Configuration\Configuration.xml");
            XmlNode backupLocationNode = document.SelectSingleNode("Configuration/BackupLocation");

            Assert.AreEqual(@"C:\MyTestBackupLocation\", backupLocationNode.InnerText);

            //Delete the test directory
            Directory.Delete(@"C:\MyTestBackupLocation\");

            ConfigurationHandler.SetBackupLocation(@"C:\ProgramData\MySQLBackup\Backup\");
        }

        [TestMethod]
        public void ModifyDeleteBackupsOlderThanDaysTest()
        {
            ConfigurationHandler.SetDeleteBackupsOlderThanDays(14);

            XmlDocument document = new XmlDocument();
            document.Load(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Configuration\Configuration.xml");
            XmlNode deleteBackupsOlderThanNode = document.SelectSingleNode("Configuration/DeleteBackupsOlderThan");

            Assert.AreEqual("14", deleteBackupsOlderThanNode.InnerText);
            ConfigurationHandler.SetDeleteBackupsOlderThanDays(7);
        }

        [TestMethod]
        public void RetrieveBackupLocationTest()
        {
            string backupLocation = ConfigurationHandler.GetBackupLocation();
            Assert.AreEqual(@"C:\ProgramData\MySQLBackup\Backup\", backupLocation);
        }

        [TestMethod]
        public void RetrieveDeleteBackupOlderThanDaysTest()
        {
            int days = ConfigurationHandler.GetDeleteBackupsOlderThanDays();
            Assert.AreEqual(7, days);
        }

        [TestMethod]
        public void InsertNewDatabaseNodeToDatabasesXMLFileTest()
        {
            DatabasesHandler dbHandler = new DatabasesHandler();
            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.Host = "localhost";
            dbInfo.User = "test";
            dbInfo.Password = "secret";
            dbInfo.DatabaseName = "test_database";
            dbInfo.StartTimeHour = 4;
            dbInfo.StartTimeMinute = 30;

            dbHandler.InsertDatabaseNode(dbInfo);

            XmlDocument document = new XmlDocument();
            document.Load(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Configuration\Databases.xml");
            XmlNode databaseNode = document.SelectSingleNode("Databases/Database[@Name='test_database']");
            string databaseNameAttr = databaseNode.Attributes["Name"].Value;

            Assert.AreEqual("test_database", databaseNameAttr);

            //remove the database node we just created
            databaseNode.ParentNode.RemoveChild(databaseNode);
            document.Save(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Configuration\Databases.xml");
        }

        [TestMethod]
        public void RemoveSpecificDatabaseNodeTest()
        {
            DatabasesHandler dbHandler = new DatabasesHandler();
            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.Host = "localhost";
            dbInfo.User = "test";
            dbInfo.Password = "secret";
            dbInfo.DatabaseName = "TestDatabase";
            dbInfo.StartTimeHour = 4;
            dbInfo.StartTimeMinute = 30;

            dbHandler.InsertDatabaseNode(dbInfo);
            dbHandler.RemoveDatabaseNode(dbInfo.DatabaseName);

            XmlDocument document = new XmlDocument();
            document.Load(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Configuration\Databases.xml");
            XmlNode databaseNode = document.SelectSingleNode("Databases/Database[@Name='" + dbInfo.DatabaseName + "']");

            Assert.IsNull(databaseNode);
        }

        [TestMethod]
        public void RetrieveSpecificDatabaseNodeTest()
        {
            DatabasesHandler dbHandler = new DatabasesHandler();
            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.Host = "localhost";
            dbInfo.User = "test";
            dbInfo.Password = "secret";
            dbInfo.DatabaseName = "TestDatabase";
            dbInfo.StartTimeHour = 4;
            dbInfo.StartTimeMinute = 30;

            dbHandler.InsertDatabaseNode(dbInfo);

            DatabaseInfo dbInfo2 = dbHandler.GetDatabaseNode(dbInfo.DatabaseName);

            Assert.AreEqual("testdatabase", dbInfo2.DatabaseName);

            dbHandler.RemoveDatabaseNode(dbInfo2.DatabaseName);
        }

        [TestMethod]
        public void UpdateSpecificDatabaseNodeTest()
        {
            DatabasesHandler dbHandler = new DatabasesHandler();
            DatabaseInfo dbInfo = new DatabaseInfo();
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

            Assert.AreEqual("22:59:00", dbInfo.StartTime.ToString());

            dbHandler.RemoveDatabaseNode(dbInfo.DatabaseName);
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

            StreamReader reader = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\MySQLBackup\Log.txt");
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
            DatabasesHandler dbHandler = new DatabasesHandler();
            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.Host = "localhost";
            dbInfo.User = "test";
            dbInfo.Password = "secret";
            dbInfo.DatabaseName = "TestDatabase";
            dbInfo.StartTimeHour = 4;
            dbInfo.StartTimeMinute = 30;

            dbHandler.InsertDatabaseNode(dbInfo);
            dbInfo.DatabaseName = "NewDatabase";
            dbHandler.InsertDatabaseNode(dbInfo);

            Assert.IsTrue(1 < dbHandler.GetAllDatabaseNodes().Count);

            dbHandler.RemoveDatabaseNode(dbInfo.DatabaseName);
            dbHandler.RemoveDatabaseNode("TestDatabase");
        }

        [TestMethod]
        public void GetMySQLBinPathTest()
        {
            String binLocation = MySQLBackup.Application.Util.Utilities.RetrieveMySQLInstallationBinPath();
            Assert.IsTrue(!String.IsNullOrEmpty(binLocation) && binLocation.ToLower().Contains("bin"));
        }
    }
}
