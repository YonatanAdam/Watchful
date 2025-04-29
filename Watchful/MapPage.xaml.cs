using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using GMap.NET;
using System.Windows;
using System.Net;
using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;
using ViewModel;
using Location = Model.Location;
using Model;

namespace Watchful
{
    /// <summary>
    /// Interaction logic for MapPage.xaml
    /// </summary>
    public partial class MapPage : Page
    {

        private DispatcherTimer _zoomUpdateTimer;
        private readonly MainWindow _mainWindow;

        public MapPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            Map_load();
            FillUserGroups();
        }

        private void FillUserGroups()
        {
            // Create an instance of GroupDB to fetch groups
            GroupDB groupDB = new GroupDB();

            // Get the list of groups that the current user is in
            var groups = groupDB.GetAllGroupsForUser(MainWindow.CurrentUser.Id);

            // Set the ComboBox ItemsSource to the groups list
            groupSelector.ItemsSource = groups;

            // Optionally, set a display member if you're using a custom object (e.g., display the GroupName)
            groupSelector.DisplayMemberPath = "GroupName";

            // If you want to select the first group by default (optional)
            if (groups.Count > 0)
            {
                groupSelector.SelectedIndex = 0;
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the SettingsPage with page caching
            _mainWindow.MainFrame.NavigationService.Navigate(new SettingsPage(_mainWindow));

            // Optional: Remove back entry to prevent multiple instances
            _mainWindow.MainFrame.NavigationService.RemoveBackEntry();
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
                // mapTypeBox.SelectedIndex = 3;
                GMaps.Instance.Mode = AccessMode.ServerAndCache; // First run must be with server so the map could load, and then it can be changes to CacheOnly.
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
        private void GroupSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (groupSelector.SelectedItem is Group selectedGroup)
            //{
            //    // Handle selected group here
            //    MainWindow.CurrentGroup = selectedGroup;
            //}
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
                    Stroke = System.Windows.Media.Brushes.Fuchsia,
                    StrokeThickness = 1.5,
                    Fill = System.Windows.Media.Brushes.Fuchsia
                }
            };

            // Add the marker to the map
            gmap.Markers.Add(marker);
        }

        private void GroupButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainFrame.Navigate(new GroupCreationPage(_mainWindow));
        }
        public void UpdateLocation()
        {
            var db = new LocationDB();
            const string tableName = "LocationTbl";

            // Retrieve the user's latest location from the database
            var entities = db.Select(tableName);

            // Find the current user's location (assuming there's a global `currentUserId`)
            Location currentUserLocation = entities
                .Cast<Location>()
                .FirstOrDefault(loc => loc.Id == MainWindow.CurrentUser.Id);

            if (currentUserLocation == null)
            {
                MessageBox.Show("User location not found.");
                return;
            }

            // Remove the previous marker of the current user
            var oldMarker = gmap.Markers
                .FirstOrDefault(m => m.Position.Lat == currentUserLocation.Latitude &&
                                     m.Position.Lng == currentUserLocation.Longitude);

            if (oldMarker != null)
            {
                gmap.Markers.Remove(oldMarker);
            }

            // Add a new green marker at the updated location
            PointLatLng newLatLng = new PointLatLng(currentUserLocation.Latitude, currentUserLocation.Longitude);

            var newMarker = new GMapMarker(newLatLng)
            {
                Shape = new System.Windows.Shapes.Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Stroke = System.Windows.Media.Brushes.Green,
                    StrokeThickness = 1.5,
                    Fill = System.Windows.Media.Brushes.Green
                }
            };

            // Add the new marker to the map
            gmap.Markers.Add(newMarker);
        }


        private void LoadMarks_OnClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Loading DB");
            var db = new LocationDB(); // Ensure LocationDB is correctly implemented
            Console.WriteLine("DONE Loading DB");

            const string tableName = "LocationTbl"; // Your table name

            // Fetch entities from the table
            var entities = db.Select(tableName);
            Console.WriteLine($"Data from {tableName}:");
            string dbPath = "D:\\Watchful\\ViewModel\\WatchfulDB2.accdb";
            Console.WriteLine("Database Path: " + dbPath);


            // Print each entity's details 
            foreach (var entity in entities)
            {
                var tempLoc = (Location)entity;
                // PointLatLng latLng = gmap.FromLocalToLatLng((int)tempLoc.Latitude, (int)tempLoc.Longitude);
                PointLatLng latLng = new PointLatLng(tempLoc.Latitude, tempLoc.Longitude);

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
                UpdateLocation();
                Console.WriteLine(entity.ToString());
            }

            Console.WriteLine($"End of {tableName}");
        }

        private void AddMark_OnClick(object sender, RoutedEventArgs e)
        {
            (double Lat, double Lng) = (gmap.Position.Lat, gmap.Position.Lng);

            Location location = new Location(Lat, Lng);
            LocationDB db = new LocationDB();
            db.Insert(location);
        }

        private void listMembers_Click(object sender, RoutedEventArgs e)
        {
            Group SelectedGroup = groupSelector.SelectedItem as Group;

            if (SelectedGroup == null)
            {
                MessageBox.Show("Please select a group first.", "No Group Selected",
                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Create and show the members list window
            GroupMembersWindow membersWindow = new GroupMembersWindow(selectedGroup.Id);
            membersWindow.Owner = this; // Set the owner to current window
            membersWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            membersWindow.ShowDialog(); // Show as modal dialog
        }
    }
}
