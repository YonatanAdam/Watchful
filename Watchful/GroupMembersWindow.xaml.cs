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

        private readonly MapPage _mapPage;

        public GroupMembersWindow(int groupId, MapPage mapPage)
        {
            InitializeComponent();
            _groupId = groupId;
            _mapPage = mapPage;

            LoadGroupData();
        }

        public bool IsCurrentUserAdmin => MainWindow.CurrentUser.Id == _currentGroup?.Admin?.Id;


        private void LoadGroupData()
        {
            GroupDB groupDB = new GroupDB();
            _currentGroup = groupDB.GetGroupById(_groupId);

            // Bind group name
            this.DataContext = _currentGroup;

            // Load members
            UserDB userDB = new UserDB();
            MembersDataGrid.ItemsSource = userDB.GetAllUsersByGroupId(_groupId);

            // Load rules
            RuleDB ruleDB = new RuleDB();
            RulesDataGrid.ItemsSource = ruleDB.GetAllRulesByGroupId(_groupId);
        }

        private void EditRule_Click(object sender, RoutedEventArgs e)
        {
            if (RulesDataGrid.SelectedItem is Rule selectedRule)
            {
                // EditRuleWindow editRuleWindow = new EditRuleWindow(selectedRule);
                /*if (editRuleWindow.ShowDialog() == true)
                {
                    // Refresh rules after editing
                    LoadGroupData();
                }*/
            }
        }
        private void DeleteRule_Click(object sender, RoutedEventArgs e)
        {
            if (RulesDataGrid.SelectedItem is Rule selectedRule)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete the rule '{selectedRule.RuleName}'?",
                                                          "Confirm Deletion",
                                                          MessageBoxButton.YesNo,
                                                          MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    RuleDB ruleDB = new RuleDB();
                    ruleDB.Delete(selectedRule);
                    BaseDB.SaveChanges();

                    // Refresh rules after deletion
                    LoadGroupData();
                }
            }
        }

        private void RemoveMember_Click(object sender, RoutedEventArgs e)
        {
            if (MembersDataGrid.SelectedItem is User selectedUser)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to remove '{selectedUser.Name}' from the group?",
                                                          "Confirm Removal",
                                                          MessageBoxButton.YesNo,
                                                          MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Remove the user from the group
                        UserDB userDB = new UserDB();
                        userDB.RemoveUserFromGroup(selectedUser.Id, _groupId);

                        // Save changes to the database
                        int rowsAffected = BaseDB.SaveChanges();

                        // Check if any rows were affected
                        if (rowsAffected > 0)
                        {
                            // Refresh the members list
                            LoadGroupData();

                            MessageBox.Show($"'{selectedUser.Name}' has been removed from the group.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show($"No changes were made. '{selectedUser.Name}' may not have been part of the group.", "No Changes", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while removing the member: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a member to remove.", "No Member Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Handle selection change on the Members data grid
        private void MembersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MembersDataGrid.SelectedItem is User selectedUser)
            {
                // Update the map location to the selected user's latitude and longitude
                _mapPage.UpdateMapLocation(selectedUser.Latitude, selectedUser.Longitude);
            }
        }

        // Handle selection change on the Rules data grid
        private void RulesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RulesDataGrid.SelectedItem is Rule selectedRule)
            {
                // Update the map location to the selected rule's latitude and longitude
                _mapPage.UpdateMapLocation(selectedRule.Latitude, selectedRule.Longitude);
            }
        }

    }
}
