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
    /// Interaction logic for RulesWindow.xaml
    /// </summary>
    public partial class RulesWindow : Window
    {
        private int _groupId;
        // private Group _currentGroup;
        private readonly MapPage _mapPage;

        public RulesWindow(int groupId, double latitude, double longitude, MapPage mapPage)
        {
            InitializeComponent();
            _groupId = groupId;

            // Pre-fill the latitude and longitude fields
            LatitudeTextBox.Text = latitude.ToString("F6");
            LongitudeTextBox.Text = longitude.ToString("F6");
            _mapPage = mapPage;
        }

        // When the ComboBox selection changes
        private void RuleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedRule = (ComboBoxItem)RuleComboBox.SelectedItem;
            if (selectedRule != null)
            {
                string rule = selectedRule.Content.ToString();

                if (rule == "Check if member is within radius" || rule == "Check if member has left the radius")
                {
                    // Enable the fields for selecting radius
                    LatitudeTextBox.IsEnabled = true;
                    LongitudeTextBox.IsEnabled = true;
                    RadiusSlider.IsEnabled = true; // Enable the slider
                }
            }
        }


        // Event for the "Save Rule" button
        private void SaveRuleButton_Click(object sender, RoutedEventArgs e)
        {
            RuleDB ruleDB = new RuleDB();
            try
            {
                // Validate inputs
                if (RuleComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select a rule type.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string ruleName = RuleNameTextBox.Text;
                string ruleType = ((ComboBoxItem)RuleComboBox.SelectedItem).Content.ToString();
                if (string.IsNullOrEmpty(ruleName) || string.IsNullOrEmpty(ruleType))
                {
                    MessageBox.Show("Rule name and type are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!double.TryParse(LatitudeTextBox.Text, out double latitude) ||
                    !double.TryParse(LongitudeTextBox.Text, out double longitude))
                {
                    MessageBox.Show("Invalid latitude or longitude.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                double radius = RadiusSlider.Value;

                // Create and insert the rule
                Rule newRule = new Rule
                {
                    RuleName = ruleName,
                    RuleType = ruleType,
                    Latitude = latitude,
                    Longitude = longitude,
                    Radius = radius,
                    GroupId = _groupId
                };

                ruleDB.Insert(newRule);
                int rowsAffected = BaseDB.SaveChanges();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Rule saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _mapPage.ShowUsersAndRulesOnMap();
                }
                else
                {
                    MessageBox.Show("Failed to save the rule. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving rule: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Event for the "Back" button
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Close the current window

        }

        // Method to receive the coordinates from the MapPage
        public void SetCoordinates(double latitude, double longitude)
        {
            // You can display or use these coordinates here in the Rules Window
            MessageBox.Show($"Selected Location: Latitude = {latitude}, Longitude = {longitude}");
        }
    }
}
