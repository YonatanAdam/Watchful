﻿using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using GMap.NET;
using System.Windows;
using System.Net;
using System;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;

namespace testmap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private DispatcherTimer _zoomUpdateTimer;

        public MainWindow()
        {
            InitializeComponent();
            Map_load();


        }

        private void Map_load(/*object sender, EventArgs e*/)
        {
            try
            {
                // Rotation
                gmap.Bearing = 0;

                gmap.CanDragMap = true;
                gmap.DragButton = MouseButton.Left;

                gmap.MaxZoom = 18;
                gmap.MinZoom = 3;
                
                gmap.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
                gmap.ShowTileGridLines = false;
                gmap.Zoom = 8;
                gmap.ShowCenter = true;
                gmap.MapProvider = GMapProviders.OpenStreetMap;
                // In your window/page load or initialization method
                mapTypeBox.SelectedIndex = 3;
                GMaps.Instance.Mode = AccessMode.ServerAndCache; // First run must be with server so the map could load and then it can be changes to CacheOnly.
                gmap.Position = new PointLatLng(31.4117257, 35.0818155); // Start in Israel

                GMapProvider.Language = LanguageType.Hebrew;

                GMapProvider.WebProxy = WebRequest.GetSystemWebProxy();
                GMapProvider.WebProxy.Credentials = CredentialCache.DefaultCredentials;

                // Initialize zoom display
                zoomLvl.Text = $"Zoom Level: {gmap.Zoom}";
                coordinates.Text = $"Coordinate: {gmap.Position.Lat:F3}, {gmap.Position.Lng:F3}";

                gmap.OnPositionChanged += Gmap_OnPositionChanged;

                // Start the timer to update zoom level periodically
                StartZoomUpdateTimer();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to set web proxy: {ex.Message}");
            }
        }

        // Start the timer to update the zoom level every 100 ms
        private void StartZoomUpdateTimer()
        {
            _zoomUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100) // Update every 0.1 second
            };

            _zoomUpdateTimer.Tick += (sender, args) =>
            {
                // Update the zoom level text block
                zoomLvl.Text = $"Zoom Level: {gmap.Zoom}";
            };

            _zoomUpdateTimer.Start();
        }

        // Event handler for OnPositionChanged
        private void Gmap_OnPositionChanged(PointLatLng point)
        {
            // Update the coordinates TextBlock with the new position
            coordinates.Text = $"Coordinate: {point.Lat:F3}, {point.Lng:F3}";
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
        private void MapTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mapTypeBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string mapType = selectedItem.Tag.ToString();
                switch (mapType)
                {
                    case "GoogleMap":
                        gmap.MapProvider = GMapProviders.GoogleMap;
                        break;
                    case "GoogleSatellite":
                        gmap.MapProvider = GMapProviders.GoogleSatelliteMap;
                        break;
                    case "BingMap":
                        gmap.MapProvider = GMapProviders.BingMap;
                        break;
                    case "OpenStreetMap":
                        gmap.MapProvider = GMapProviders.OpenStreetMap;
                        break;
                    default:
                        gmap.MapProvider = GMapProviders.OpenStreetMap;
                        break;
                }
            }
        }

        private void Gmap_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get the mouse position relative to the GMapControl
            var point = e.GetPosition(gmap);

            // Convert the screen coordinates to geographical coordinates (latitude, longitude)
            PointLatLng latLng = gmap.FromLocalToLatLng((int)point.X, (int)point.Y);

            // Create a new marker at the clicked position
            var marker = new GMapMarker(latLng)
            {
                Shape = new System.Windows.Shapes.Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Stroke = System.Windows.Media.Brushes.Red,
                    StrokeThickness = 1.5,
                    Fill = System.Windows.Media.Brushes.Red
                }
            };

            // Add the marker to the map
            gmap.Markers.Add(marker);
        }

        private void ClearMarks_Click(object sender, RoutedEventArgs e)
        {
            gmap.Markers.Clear();
        }
    }
}
