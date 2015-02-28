using MySQLBackup.Application.Logging;
using System.Windows;
using System.Windows.Controls;

namespace MySQLBackupManager.Pages
{
    /// <summary>
    /// Interaction logic for ShowLogPage.xaml
    /// </summary>
    public partial class ShowLogPage : Page
    {
        private readonly LogHandler logHandler = new LogHandler();

        public ShowLogPage()
        {
            InitializeComponent();

            LogTextBox.Text = logHandler.GetLogText().Replace("\n", "");
        }

        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            logHandler.ClearLog();
            LogTextBox.Text = logHandler.GetLogText().Replace("\n", "");
        }
    }
}
