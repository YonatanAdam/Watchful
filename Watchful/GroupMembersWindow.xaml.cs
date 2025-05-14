using System;
using System.Windows;
using System.Windows.Controls;
using Model;
using ViewModel;

namespace Watchful
{
    /// <summary>
    /// Interaction logic for GroupMembersWindow.xaml.
    /// Displays and manages the members and rules of a group, allowing admin actions such as removing members and deleting rules.
    /// </summary>
    public partial class GroupMembersWindow : Window
    {
        private int _groupId;
        private Group _currentGroup;
        private readonly MapPage _mapPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupMembersWindow"/> class.
        /// </summary>
        /// <param name="groupId">The ID of the group to display.</param>
        /// <param name="mapPage">The map page instance for updating map location.</param>
        public GroupMembersWindow(int groupId, MapPage mapPage)
        {
            InitializeComponent();
            _groupId = groupId;
            _mapPage = mapPage;

            LoadGroupData();
        }

        /// <summary>
        /// Gets a value indicating whether the current user is the admin of the group.
        /// </summary>
        public bool IsCurrentUserAdmin => MainWindow.CurrentUser.Id == _currentGroup?.Admin?.Id;

        /// <summary>
        /// Loads group data, including group details, members, and rules, and binds them to the UI.
        /// </summary>
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

        /// <summary>
        /// Handles the click event for editing a rule (currently not implemented).
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
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

        /// <summary>
        /// Handles the click event for deleting a selected rule from the group.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
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

        /// <summary>
        /// Handles the click event for removing a selected member from the group.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
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

        /// <summary>
        /// Handles the click event for the close button to close the window.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the selection changed event on the members data grid to update the map location to the selected user's location.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The selection changed event arguments.</param>
        private void MembersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MembersDataGrid.SelectedItem is User selectedUser)
            {
                _mapPage.UpdateMapLocation(selectedUser.Latitude, selectedUser.Longitude);
            }
        }

        /// <summary>
        /// Handles the selection changed event on the rules data grid to update the map location to the selected rule's location.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The selection changed event arguments.</param>
        private void RulesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RulesDataGrid.SelectedItem is Rule selectedRule)
            {
                _mapPage.UpdateMapLocation(selectedRule.Latitude, selectedRule.Longitude);
            }
        }
    }
}
