using System.Windows.Controls;
using System.Windows.Navigation;

namespace MySQLBackupManager.Pages
{
    /// <summary>
    /// Interaction logic for AppDetailsPage.xaml
    /// </summary>
    public partial class AppDetailsPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDetailsPage"/> class.
        /// </summary>
        public AppDetailsPage()
        {
            InitializeComponent();
            TxtAppVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        }

        /// <summary>
        /// Handles the RequestNavigate event of the Hyperlink control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RequestNavigateEventArgs"/> instance containing the event data.</param>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}
