using MySQLBackup.Application.Backup;
using MySQLBackup.Application.Config;
using MySQLBackup.Application.Model;
using MySQLBackupManager.Pages.Content;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MySQLBackupManager.Pages
{
    /// <summary>
    /// Interaction logic for RestoreDatabasePage.xaml
    /// </summary>
    public partial class RestoreDatabasePage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestoreDatabasePage"/> class.
        /// </summary>
        public RestoreDatabasePage()
        {
            InitializeComponent();
            this.DataContext = new DatabasesViewModel();
        }

        /// <summary>
        /// Handles the Click event of the SelectBackupDumpFileButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SelectBackupDumpFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = ConfigurationXmlHandler.GetBackupLocation();
            dlg.DefaultExt = ".dump";
            dlg.Filter = "Backup Dump Files (*.dump)|*.dump|SQL Dump Files (*.sql)|*.sql";
            Boolean? hasSelected = dlg.ShowDialog();
            if (hasSelected == true)
            {
                TextRestoreFile.Text = dlg.FileName;
            }
        }

        /// <summary>
        /// Handles the Click event of the RestoreDatabaseButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void RestoreDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            String filename = TextRestoreFile.Text;
            DatabaseInfo dbInfo = SelectDatabase.SelectedItem as DatabaseInfo;
            if (null == dbInfo)
            {
                FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage(string.Format("Please select a database to restore the file into."), "No database selected", MessageBoxButton.OK);
            }
            else
            {
                if (!File.Exists(filename))
                {
                    FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage(string.Format("The file {0} could not be found. Please select a valid dump file.", filename), "File not found", MessageBoxButton.OK);
                }
                else
                {
                    try
                    {
                        if (FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage(string.Format("You are about to restore the database {0}[{1}] with a backup file. This means that all content in the database will be overwritten, with the information from the backup file. This action can't be undone. Do you want to continue?",dbInfo.Host,dbInfo.DatabaseName), "Proceed with database restore?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            BackupHandler backupHandler = new BackupHandler();
                            backupHandler.RestoreDatabase(filename, dbInfo.ID);
                            FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage(string.Format("The database {0} has been restored from this backup dump file '{1}'", dbInfo.DatabaseName, filename), "Success", MessageBoxButton.OK);
                        }
                    }
                    catch (Exception ex)
                    {
                        FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage(ex.Message,"Error", MessageBoxButton.OK);
                    }
                }
            }
        }
    }
}
