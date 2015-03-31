using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using MySQLBackup.Application.Config;

namespace MySQLBackupManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            BackupManagerMainWindow.Title += " " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            new ConfigLocationCreator().CreateConfigLocations();
            //Load the user settings
            this.LoadUserSettings();
        }

        /// <summary>
        /// Loads the user settings saved in the settings.settings file.
        /// </summary>
        private void LoadUserSettings()
        {
            AppearanceManager.Current.ThemeSource = Properties.Settings.Default.ApplicationTheme.Equals("LightThemeSource") ? AppearanceManager.LightThemeSource : AppearanceManager.DarkThemeSource;
            AppearanceManager.Current.AccentColor = Properties.Settings.Default.ApplicationColor;
            AppearanceManager.Current.FontSize = Properties.Settings.Default.ApplicationFontSize;
        }
    }
}
