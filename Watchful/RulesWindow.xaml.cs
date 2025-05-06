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

namespace Watchful
{
    /// <summary>
    /// Interaction logic for RulesWindow.xaml
    /// </summary>
    public partial class RulesWindow : Window
    {
        private int _groupId;
        private Group _currentGroup;

        public RulesWindow(int groupId, double latitude, double longitude)
        {
            InitializeComponent();
            _groupId = groupId;

            // Pre-fill the latitude and longitude fields
            LatitudeTextBox.Text = latitude.ToString("F6");
            LongitudeTextBox.Text = longitude.ToString("F6");
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
            try
            {
                // Get the values from the input fields
                double latitude = double.Parse(LatitudeTextBox.Text);
                double longitude = double.Parse(LongitudeTextBox.Text);
                double radius = RadiusSlider.Value; // Get the value from the slider

                // Logic to save or apply the rule
                MessageBox.Show($"Rule saved with latitude: {latitude}, longitude: {longitude}, radius: {radius}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving rule: {ex.Message}");
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
