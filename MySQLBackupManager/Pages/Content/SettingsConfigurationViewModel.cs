using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySQLBackupManager.Pages.Commands;
using MySQLBackup.Application.Config;

namespace MySQLBackupManager.Pages.Content
{
    class SettingsConfigurationViewModel : NotifyPropertyChanged
    {
        //Object Variables
        private string backupLocation;
        private int deleteBackupAfterDays;

        public SettingsConfigurationViewModel()
        {
            ChangeLocationButton = new ButtonClickCommand(this.ChangeBackupLocationClickEvent);

            SyncFromConfiguration();
        }

        /**
         * Synchronise with the values from the Configuration file.
         */
        private void SyncFromConfiguration()
        {
            this.backupLocation = ConfigurationXmlHandler.GetBackupLocation();
            this.deleteBackupAfterDays = ConfigurationXmlHandler.GetDeleteBackupsOlderThanDays();
        }

        //Properties
        public string BackupLocation
        {
            get { return this.backupLocation; }
            set
            {
                this.backupLocation = value;
                OnPropertyChanged("BackupLocation");

                //Save the modification in the Configurations File.
                ConfigurationXmlHandler.SetBackupLocation(this.backupLocation);
            }
        }

        public int DeleteBackupAfterDays
        {
            get { return this.deleteBackupAfterDays; }
            set
            {
                this.deleteBackupAfterDays = value;
                OnPropertyChanged("DeleteBackupAfterDays");

                //Save the modification in the Configurations File.
                ConfigurationXmlHandler.SetDeleteBackupsOlderThanDays(this.deleteBackupAfterDays);
            }
        }

        public ButtonClickCommand ChangeLocationButton { get; set; }

        private void ChangeBackupLocationClickEvent()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.BackupLocation = dialog.SelectedPath;
            }
        }
    }
}
