using System;
using System.Windows;
using System.Windows.Controls;
using Model;
using ViewModel;

namespace Watchful
{
    public partial class GroupMembersWindow : Window
    {
        private int _groupId;
        private Group _currentGroup;

        public GroupMembersWindow(int groupId)
        {
            InitializeComponent();
            _groupId = groupId;
            LoadGroupInfo();
            LoadGroupMembers();
        }

        private void LoadGroupInfo()
        {
            GroupDB groupDB = new GroupDB();
            _currentGroup = groupDB.GetGroupById(_groupId);

            // Display the group name
            GroupNameText.Text = _currentGroup.GroupName;

            // Display the admin name
            AdminNameText.Text = _currentGroup.Admin.Name;
        }

        private void LoadGroupMembers()
        {
            UserDB userDB = new UserDB();

            // Get all members of the group
            UserList members = userDB.GetAllUsersByGroupId(_groupId);

            // Bind members to the DataGrid
            MembersTable.ItemsSource = members;
        }

        private void ModifyUser_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected user
            User selectedUser = MembersTable.SelectedItem as User;

            if (selectedUser != null)
            {
                // Open a dialog or window to modify the user
                ModifyMemberWindow modifyUserWindow = new ModifyMemberWindow(selectedUser);
                modifyUserWindow.ShowDialog();

                // Reload the members list after modification
                LoadGroupMembers();
            }
            else
            {
                MessageBox.Show("Please select a user to modify.", "No User Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
