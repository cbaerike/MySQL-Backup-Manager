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
        private readonly DatabasesXmlHandler dbHandler = new DatabasesXmlHandler();

        private DatabaseInfo CurrentDbInfo { get; set; }

        public ModifyDatabasePage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
            Guid databaseId;
            if (Guid.TryParse(e.Fragment, out  databaseId))
            {
                DatabaseInfo dbInfo = dbHandler.GetDatabaseNode(databaseId);
                if (dbInfo == null)
                {
                    FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage("The database requested was not found!", "Not Found", MessageBoxButton.OK);
                    NavigationCommands.GoToPage.Execute(new Uri("/Pages/DatabasesPage.xaml", UriKind.Relative), FirstFloor.ModernUI.Windows.Navigation.NavigationHelper.FindFrame(null, this));
                }
                CurrentDbInfo = dbInfo;
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
            dbHandler.UpdateDatabaseNode(CurrentDbInfo);
            new LogHandler().LogMessage(LogHandler.MessageType.INFO, string.Format("The database {0} has been successfully modified", CurrentDbInfo.DatabaseName));
            NavigationCommands.GoToPage.Execute(new Uri("/Pages/DatabasesPage.xaml", UriKind.Relative), FirstFloor.ModernUI.Windows.Navigation.NavigationHelper.FindFrame(null, this));
        }

        private void RemoveDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            var result = FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage(string.Format("Are you sure that you want to remove the database '{0}'?\nThis action can't be undone!", CurrentDbInfo.DatabaseName), "Remove Database", MessageBoxButton.YesNo);

            if (result.ToString().ToLower().Equals("yes"))
            {
                dbHandler.RemoveDatabaseNode(CurrentDbInfo.ID);
                new LogHandler().LogMessage(LogHandler.MessageType.INFO, string.Format("The database {0} has been successfully removed", CurrentDbInfo.DatabaseName));
                NavigationCommands.GoToPage.Execute(new Uri("/Pages/DatabasesPage.xaml", UriKind.Relative), FirstFloor.ModernUI.Windows.Navigation.NavigationHelper.FindFrame(null, this));
            }
        }

        private void MakeManualBackupButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BackupHandler backupHandler = new BackupHandler();
                if (backupHandler.CreateBackup(CurrentDbInfo.ID))
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
        }
    }
}
