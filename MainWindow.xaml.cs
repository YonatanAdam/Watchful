using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using GMap.NET;
using System.Windows;
using System.Net;
using System;
using System.Windows.Input;
using System.Windows.Controls;

namespace testmap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void map_load(object sender, EventArgs e)
        {
            // Rotation
            gmap.Bearing = 0;

            gmap.CanDragMap = true;
            gmap.DragButton = MouseButton.Left;

            gmap.MaxZoom = 18;
            gmap.MinZoom = 2;

            gmap.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
            gmap.ShowTileGridLines = false;
            gmap.Zoom = 2;
            gmap.ShowCenter = false;
            gmap.MapProvider = GMapProviders.GoogleMap;
            GMaps.Instance.Mode = AccessMode.ServerAndCache; // First run must be with server so the map could load and then it can be changes to CacheOnly.
            gmap.Position = new PointLatLng(31.4117257, 35.0818155); // Start in Israel

            GMapProvider.Language = LanguageType.Hebrew; // not very good:(

            try
            {
                GMapProvider.WebProxy = WebRequest.GetSystemWebProxy();
                GMapProvider.WebProxy.Credentials = CredentialCache.DefaultCredentials;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to set web proxy: {ex.Message}");
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            gmap.Width = e.NewSize.Width;
            gmap.Height = e.NewSize.Height;
            gmap.InvalidateVisual();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Properly dispose of GMapControl to release any resources
            gmap.Dispose();

            // Ensure the application terminates fully
            Application.Current.Shutdown();
        }

        private void zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (gmap != null)
            {
                // Set the GMap zoom level based on the slider value
                gmap.Zoom = zoomSlider.Value;
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            map_load(sender, e);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void GoToButton_Click(object sender, RoutedEventArgs e)
        {
            double lat, lng;
            if (double.TryParse(latTextBox.Text, out lat) && double.TryParse(lngTextBox.Text, out lng))
            {
                gmap.Position = new PointLatLng(lat, lng);
            }
            else
            {
                MessageBox.Show("קואורדינטות לא חוקיות. נא להזין קו רוחב וקו אורך חוקיים.");
            }
        }

        private void accessModeBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (accessModeBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string mode = selectedItem.Tag.ToString();
                switch (mode)
                {
                    case "ServerAndCache":
                        GMaps.Instance.Mode = AccessMode.ServerAndCache;
                        break;
                    case "CacheOnly":
                        GMaps.Instance.Mode = AccessMode.CacheOnly;
                        break;
                    default:
                        // Handle unexpected values if necessary
                        break;
                }
            }
        }
    }
}
