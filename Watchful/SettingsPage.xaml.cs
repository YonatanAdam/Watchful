using System.Windows;
using System.Windows.Controls;

namespace Watchful
{
    public partial class SettingsPage : Page
    {
        private readonly MainWindow _mainWindow;

        public SettingsPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Load existing settings (placeholder logic)
            DarkThemeCheckBox.IsChecked = true; // Assume dark theme is enabled by default
            NotificationsCheckBox.IsChecked = false; // Assume notifications are disabled by default
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the previous page
            if (_mainWindow.MainFrame.NavigationService.CanGoBack)
            {
                _mainWindow.MainFrame.NavigationService.GoBack();
            }
        }
    }
}