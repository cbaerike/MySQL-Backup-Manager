using FirstFloor.ModernUI.Windows;
using MySQLBackup.Application.Backup;
using MySQLBackup.Application.Config;
using MySQLBackup.Application.Logging;
using MySQLBackup.Application.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MySQLBackupManager.Pages
{
    /// <summary>
    /// Allows modification of database settings.
    /// </summary>
    public partial class ModifyDatabasePage : Page, IContent
    {
        /// <summary>
        /// The database handler for interacting with the DB config file
        /// </summary>
        private readonly DatabasesXmlHandler dbHandler = new DatabasesXmlHandler();

        /// <summary>
        /// Gets or sets the current database information.
        /// </summary>
        private DatabaseInfo CurrentDbInfo { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyDatabasePage"/> class.
        /// </summary>
        public ModifyDatabasePage()
        {
            InitializeComponent();
            this.DataContext = CurrentDbInfo;
        }

        /// <summary>
        /// Called when navigation to a content fragment begins: Loads the info for the selected database.
        /// </summary>
        /// <param name="e">An object that contains the navigation data.</param>
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
                this.DataContext = CurrentDbInfo;
            }
        }

        /// <summary>
        /// Called when this instance is no longer the active content in a frame.
        /// </summary>
        /// <param name="e">An object that contains the navigation data.</param>
        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        /// <summary>
        /// Called when a this instance becomes the active content in a frame.
        /// </summary>
        /// <param name="e">An object that contains the navigation data.</param>
        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        /// <summary>
        /// Called just before this instance is no longer the active content in a frame.
        /// </summary>
        /// <param name="e">An object that contains the navigation data.</param>
        /// <remarks>
        /// The method is also invoked when parent frames are about to navigate.
        /// </remarks>
        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }

        /// <summary>
        /// Handles the Click event of the ModifyDatabaseButton control: Saves the new settings and navigates to the overview.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ModifyDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            dbHandler.UpdateDatabaseNode(CurrentDbInfo);
            new LogHandler().LogMessage(LogHandler.MessageType.INFO, string.Format("The database {0} has been successfully modified", CurrentDbInfo.DatabaseName));
            NavigationCommands.GoToPage.Execute(new Uri("/Pages/DatabasesPage.xaml", UriKind.Relative), FirstFloor.ModernUI.Windows.Navigation.NavigationHelper.FindFrame(null, this));
        }

        /// <summary>
        /// Handles the Click event of the RemoveDatabaseButton control: Deleted the database from the config file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Handles the Click event of the MakeManualBackupButton control: Creates a manual backup of the selected database.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
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
