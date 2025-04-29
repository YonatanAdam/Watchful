using Model;
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
using System.Windows.Shapes;
using ViewModel;

namespace Watchful
{
    /// <summary>
    /// Interaction logic for ModifyMemberWindow.xaml
    /// </summary>
    public partial class ModifyMemberWindow : Window
    {
        private User _user;

        public ModifyMemberWindow(User user)
        {
            InitializeComponent();
            _user = user;

            // Populate fields with user data
            UserNameTextBox.Text = _user.Name;
            LatitudeTextBox.Text = _user.Latitude.ToString();
            LongitudeTextBox.Text = _user.Longitude.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Update user details
            _user.Name = UserNameTextBox.Text;
            _user.Latitude = double.Parse(LatitudeTextBox.Text);
            _user.Longitude = double.Parse(LongitudeTextBox.Text);

            // Save changes to the database
            UserDB userDB = new UserDB();
            userDB.Update(_user);

            MessageBox.Show("User details updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}
