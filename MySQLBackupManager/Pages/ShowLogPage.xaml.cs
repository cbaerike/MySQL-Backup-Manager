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

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowLogPage"/> class.
        /// </summary>
        public ShowLogPage()
        {
            InitializeComponent();
            LogTextBox.Text = logHandler.GetLogText().Replace("\n", "");
        }

        /// <summary>
        /// Scrolls the logfile textbox to the end, so that newest log entries are shown.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            LogTextBox.ScrollToEnd();
            base.OnRender(drawingContext);
        }

        /// <summary>
        /// Handles the Click event of the ClearLogButton control: Clears the log file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            logHandler.ClearLog();
            LogTextBox.Text = logHandler.GetLogText().Replace("\n", "");
        }
    }
}
