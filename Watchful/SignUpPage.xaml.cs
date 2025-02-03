using Model;
using System.Windows;
using System.Windows.Controls;
using ViewModel;

namespace Watchful
{
    /// <summary>
    /// Interaction logic for SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        private readonly MainWindow _mainWindow;

        public SignUpPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            // Basic validation for empty fields
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("All fields are required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Passwords must match
            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Create an instance of UserDB
            UserDB userDb = new UserDB();

            // Validate if the username already exists in the database
            if (!userDb.ValidateNewUser(username))
            {
                MessageBox.Show("A user with this username already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Create a new user object
            User newUser = new User
            {
                Name = username,
                Password = password // You can hash the password here for security purposes
            };

            // Insert the new user into the database
            userDb.Insert(newUser);

            // Assuming signup is successful
            MessageBox.Show("Account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    
            // Save changes (to the database)
            UserDB.SaveChanges();

            // Redirect to login page after successful sign-up
            _mainWindow.MainFrame.Navigate(new LoginPage(_mainWindow));
        }
        
        private void BackToLoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to LoginPage
            _mainWindow.MainFrame.Navigate(new LoginPage(_mainWindow));
        }
    }
}
