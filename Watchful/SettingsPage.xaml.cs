using Model;
using System.Windows;
using System.Windows.Controls;
using ViewModel;

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
            LatitudeTextBox.Text = "";
            LongitudeTextBox.Text = "";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the previous page
            if (_mainWindow.MainFrame.NavigationService.CanGoBack)
            {
                _mainWindow.MainFrame.NavigationService.GoBack();
            }
        }

        private void SaveLocation_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(LatitudeTextBox.Text, out double lat) && double.TryParse(LongitudeTextBox.Text, out double lon))
            {
                UserDB userDb = new UserDB();
                User currentUser = MainWindow.CurrentUser;

                currentUser.Latitude = lat;
                currentUser.Longitude = lon;

                userDb.Update(currentUser);
                int rows = BaseDB.SaveChanges();


                //bool success = userDb.UpdateUserLocation(currentUserId, lat, lon);
                //_ = BaseDB.SaveChanges();

                if (rows>0)
                {
                    // update map pin


                    MessageBox.Show("Location updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    MessageBox.Show("Failed to update location.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid latitude or longitude values.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}