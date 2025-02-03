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
using ViewModel;

namespace Watchful
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {

        private readonly MainWindow _mainWindow;
        
        public LoginPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            // Basic validation - replace with actual authentication logic
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Create an instance of UserDB
            UserDB userDb = new UserDB();

            // Validate the username and password
            if (userDb.ValidateUser(username, password))
            {
                // Assuming login is successful
                _mainWindow.MainFrame.Navigate(new MapPage(_mainWindow));
            }
            else
            {
                // Login failed
                MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the SignUpPage
            _mainWindow.MainFrame.Navigate(new SignUpPage(_mainWindow));
        }
    }
}
