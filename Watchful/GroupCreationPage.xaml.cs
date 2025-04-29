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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViewModel;

namespace Watchful
{
    /// <summary>
    /// Interaction logic for GroupCreationPage.xaml
    /// </summary>
    public partial class GroupCreationPage : Page
    {

        private readonly MainWindow _mainWindow;
        public GroupCreationPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            string GroupName = GroupNameTextBox.Text;
            string PassCode = PassCodeTextBox.Password;

            // Basic validation - replace with actual authentication logic
            if (string.IsNullOrEmpty(GroupName) || string.IsNullOrEmpty(PassCode))
            {
                MessageBox.Show("Please enter both group name and passcode.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (JoinModeRadio.IsChecked == true)
            {
                // join mode
                // Create an instance of GroupDB
                GroupDB groupDB = new GroupDB();
                Group group = groupDB.GetGroupByNameAndPass(GroupName, PassCode);
                if (group == null)
                {
                    MessageBox.Show($"Group {GroupName} was not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                bool added = groupDB.AddUserById(group.Id, MainWindow.CurrentUser.Id);
                if(added)
                {
                    MessageBox.Show($"Successfully joined: {GroupName}.", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                    _mainWindow.MainFrame.Navigate(new MapPage(_mainWindow));
                } else
                {
                    MessageBox.Show($"There was an error entering {GroupName}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else if (CreateModeRadio.IsChecked==true)
            {
                // create mode
                // Create an instance of GroupDB
                GroupDB groupDB = new GroupDB();
                // Validate the details
                Group group = groupDB.CreateGroup(GroupName, PassCode, MainWindow.CurrentUser);

                if (group != null)
                {
                    // Assuming login is successful
                    MainWindow.CurrentGroup = group;

                    _mainWindow.MainFrame.Navigate(new MapPage(_mainWindow));
                }
                else
                {
                    // Login failed
                    MessageBox.Show("Invalid group passcode.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }


        }

        private void ModeRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (_mainWindow == null) return;
            if (CreateModeRadio.IsChecked == true)
            {
                HeaderText.Text = "Create a group";
                CreateJoinButton.Content = "Create";
            }
            else if (JoinModeRadio.IsChecked == true)
            {
                HeaderText.Text = "Join a group";
                CreateJoinButton.Content = "Join";
            }
        }

    }
}
