using MySQLBackup.Application.Model;
using MySQLBackupManager.Pages.Content;
using System;
using System.Collections.Generic;
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

namespace MySQLBackupManager.Views
{
    /// <summary>
    /// Interaction logic for DatabasesPage.xaml
    /// </summary>
    public partial class DatabasesPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabasesPage"/> class.
        /// </summary>
        public DatabasesPage()
        {
            InitializeComponent();
            this.DataContext = new DatabasesViewModel();
        }

        /// <summary>
        /// Handles the Loaded event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new DatabasesViewModel();
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the DatabasesGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void DatabasesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dbInfo = DatabasesGrid.SelectedItem as DatabaseInfo;
            if (dbInfo != null)
            {
                NavigationCommands.GoToPage.Execute(new Uri("/Pages/ModifyDatabasePage.xaml#" + dbInfo.DatabaseName, UriKind.Relative), FirstFloor.ModernUI.Windows.Navigation.NavigationHelper.FindFrame(null, this));
            }
        }
    }
}
