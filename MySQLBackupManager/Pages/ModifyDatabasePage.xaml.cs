using FirstFloor.ModernUI.Windows;
using MySQLBackup.Application.Backup;
using MySQLBackup.Application.Config;
using MySQLBackup.Application.Logging;
using MySQLBackup.Application.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MySQLBackupManager.Pages
{
    /// <summary>
    /// Interaction logic for ModifyDatabasePage.xaml
    /// </summary>
    public partial class ModifyDatabasePage : Page, IContent
    {
        private readonly DatabasesHandler dbHandler = new DatabasesHandler();

        private string currentDatabaseName;
        public string CurrentDatabaseName
        {
            get
            {
                return this.currentDatabaseName;
            }
            set
            {
                this.currentDatabaseName = value;
            }
        }

        private DatabaseInfo currentDbInfo;
        public DatabaseInfo CurrentDbInfo
        {
            get
            {
                return this.currentDbInfo;
            }
            set
            {
                this.currentDbInfo = value;
            }
        }

        private string currentStartTime;
        public string CurrentStartTime
        {
            get
            {
                return this.currentStartTime;
            }
            set
            {
                this.currentStartTime = value;
            }
        }

        public ModifyDatabasePage()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
            if (e.Fragment != "")
            {
                DatabaseInfo dbInfo = dbHandler.GetDatabaseNode(e.Fragment);
                if (dbInfo == null)
                {
                    FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage("The database requested was not found!", "Not Found", MessageBoxButton.OK);
                    NavigationCommands.GoToPage.Execute(new Uri("/Pages/DatabasesPage.xaml", UriKind.Relative), FirstFloor.ModernUI.Windows.Navigation.NavigationHelper.FindFrame(null, this));
                }
                CurrentDbInfo = dbInfo;
                CurrentDatabaseName = dbInfo.DatabaseName;
                CurrentStartTime = dbInfo.StartTime.ToString();
            }
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }

        private void ModifyDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            string[] startTimeSplit = startTime.Text.Split(':');
            CurrentDbInfo.StartTimeHour = Convert.ToInt32(startTimeSplit[0]);
            CurrentDbInfo.StartTimeMinute = Convert.ToInt32(startTimeSplit[1]);

            if (CurrentDatabaseName.ToLower().Equals(CurrentDbInfo.DatabaseName.ToLower()))
            {
                dbHandler.UpdateDatabaseNode(CurrentDbInfo);
            }
            else
            {
                dbHandler.RemoveDatabaseNode(CurrentDatabaseName);
                dbHandler.InsertDatabaseNode(CurrentDbInfo);
            }


            CurrentDatabaseName = "";
            new LogHandler().LogMessage(LogHandler.MessageType.INFO, string.Format("The database {0} has been successfully modified", CurrentDbInfo.DatabaseName));
            NavigationCommands.GoToPage.Execute(new Uri("/Pages/DatabasesPage.xaml", UriKind.Relative), FirstFloor.ModernUI.Windows.Navigation.NavigationHelper.FindFrame(null, this));
        }

        private void RemoveDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            var result = FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage(string.Format("Are you sure that you want to remove the database '{0}'?\nThis action can't be undone!", CurrentDatabaseName), "Remove Database", MessageBoxButton.YesNo);

            if (result.ToString().ToLower().Equals("yes"))
            {
                dbHandler.RemoveDatabaseNode(CurrentDatabaseName);
                new LogHandler().LogMessage(LogHandler.MessageType.INFO, string.Format("The database {0} has been successfully removed", CurrentDatabaseName));
                NavigationCommands.GoToPage.Execute(new Uri("/Pages/DatabasesPage.xaml", UriKind.Relative), FirstFloor.ModernUI.Windows.Navigation.NavigationHelper.FindFrame(null, this));
            }
        }

        private void MakeManualBackupButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BackupHandler backupHandler = new BackupHandler();
                if (backupHandler.CreateBackup(currentDbInfo.DatabaseName))
                {
                    FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage(string.Format("A backup of the database {0} has been created!", CurrentDbInfo.DatabaseName), "Success", MessageBoxButton.OK);
                }
                else
                {
                    FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage(string.Format("The backup of database {0} failed. Please check the log for details", CurrentDbInfo.DatabaseName), "Error", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage(ex.ToString(), "Error", MessageBoxButton.OK);
            }
            CurrentDatabaseName = "";
        }
    }
}
