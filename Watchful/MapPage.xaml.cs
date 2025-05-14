using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using GMap.NET;
using System.Windows;
using System.Net;
using System;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;
using ViewModel;
using Model;
using System.Windows.Media;
using System.Security.Cryptography;

namespace Watchful
{
    /// <summary>
    /// Interaction logic for MapPage.xaml.
    /// Displays a map with user and rule markers, allows group selection, and provides map-related actions.
    /// </summary>
    public partial class MapPage : Page
    {
        private DispatcherTimer _zoomUpdateTimer;
        private readonly MainWindow _mainWindow;
        private Group _currentGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapPage"/> class.
        /// </summary>
        /// <param name="mainWindow">The main application window.</param>
        public MapPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            Map_load();
            FillUserGroups();
            SetCurrentGroup();
        }

        /// <summary>
        /// Fills the group selector ComboBox with all groups the current user is a member of.
        /// </summary>
        private void FillUserGroups()
        {
            GroupDB groupDB = new GroupDB();
            var groups = groupDB.GetAllGroupsForUser(MainWindow.CurrentUser.Id);
            groupSelector.ItemsSource = groups;
            groupSelector.DisplayMemberPath = "GroupName";
            if (groups.Count > 0)
            {
                groupSelector.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Sets the current group based on the selected item in the group selector.
        /// </summary>
        private void SetCurrentGroup()
        {
            _currentGroup = (Group)groupSelector.SelectedItem;
            ShowUsersAndRulesOnMap();
        }

        /// <summary>
        /// Displays all users and rules of the current group as markers on the map.
        /// </summary>
        public void ShowUsersAndRulesOnMap()
        {
            gmap.Markers.Clear();
            UserDB userDB = new UserDB();
            RuleDB ruleDB = new RuleDB();

            if (_currentGroup == null) return;
            RulesList rules = ruleDB.GetAllRulesByGroupId(_currentGroup.Id);
            UserList members = userDB.GetAllUsersByGroupId(_currentGroup.Id);

            foreach (var member in members)
            {
                PointLatLng position = new PointLatLng(member.Latitude, member.Longitude);
                Color uniqueColor = GenerateColorFromId(member.Id);

                var marker = new GMapMarker(position)
                {
                    Shape = new System.Windows.Shapes.Ellipse
                    {
                        Width = 10,
                        Height = 10,
                        Stroke = new SolidColorBrush(uniqueColor),
                        StrokeThickness = 1.5,
                        Fill = new SolidColorBrush(uniqueColor)
                    }
                };

                gmap.Markers.Add(marker);
            }

            foreach (var rule in rules)
            {
                PointLatLng position = new PointLatLng(rule.Latitude, rule.Longitude);

                var marker = new GMapMarker(position)
                {
                    Shape = new System.Windows.Shapes.Ellipse
                    {
                        Width = rule.Radius / 500,
                        Height = rule.Radius / 500,
                        Stroke = System.Windows.Media.Brushes.Red,
                        StrokeThickness = 1.5,
                        Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(80, 255, 0, 0)),
                    }
                };

                gmap.Markers.Add(marker);
            }
        }

        /// <summary>
        /// Generates a unique color based on the given user ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>A unique <see cref="Color"/> for the user.</returns>
        private Color GenerateColorFromId(int id)
        {
            byte[] idBytes = BitConverter.GetBytes(id);
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(idBytes);
                byte r = hash[0];
                byte g = hash[1];
                byte b = hash[2];
                return Color.FromRgb(r, g, b);
            }
        }

        /// <summary>
        /// Initializes and configures the map control.
        /// </summary>
        private void Map_load()
        {
            try
            {
                gmap.Bearing = 0;
                gmap.CanDragMap = true;
                gmap.DragButton = MouseButton.Left;
                gmap.MaxZoom = 18;
                gmap.MinZoom = 3;
                gmap.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
                gmap.ShowTileGridLines = false;
                gmap.Zoom = 10;
                gmap.ShowCenter = false;
                gmap.MapProvider = GMapProviders.OpenStreetMap;
                GMaps.Instance.Mode = AccessMode.ServerAndCache;
                gmap.Position = new PointLatLng(31.4117257, 35.0818155);
                GMapProvider.Language = LanguageType.Hebrew;
                GMapProvider.WebProxy = WebRequest.GetSystemWebProxy();
                GMapProvider.WebProxy.Credentials = CredentialCache.DefaultCredentials;
                zoomLvl.Text = $"Zoom Level: {gmap.Zoom}";
                coordinates.Text = $"Coordinate: {gmap.Position.Lat:F3}, {gmap.Position.Lng:F3}";
                gmap.OnPositionChanged += Gmap_OnPositionChanged;
                StartZoomUpdateTimer();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to set web proxy: {ex.Message}");
            }
        }

        /// <summary>
        /// Starts a timer to periodically update the zoom level display.
        /// </summary>
        private void StartZoomUpdateTimer()
        {
            _zoomUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };

            _zoomUpdateTimer.Tick += (sender, args) =>
            {
                zoomLvl.Text = $"Zoom Level: {gmap.Zoom}";
            };

            _zoomUpdateTimer.Start();
        }

        /// <summary>
        /// Handles the map position changed event to update the coordinates display.
        /// </summary>
        /// <param name="point">The new map position.</param>
        private void Gmap_OnPositionChanged(PointLatLng point)
        {
            coordinates.Text = $"Coordinate: {point.Lat:F3}, {point.Lng:F3}";
        }

        /// <summary>
        /// Handles the window size changed event to resize the map control.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            gmap.Width = e.NewSize.Width;
            gmap.Height = e.NewSize.Height;
            gmap.InvalidateVisual();
        }

        /// <summary>
        /// Handles the window closing event to dispose of the map control and shut down the application.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            gmap.Dispose();
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Handles the group selector selection changed event to update the current group.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void GroupSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetCurrentGroup();
        }

        /// <summary>
        /// Handles the right mouse button down event on the map to add a rule or update user location.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The mouse button event arguments.</param>
        private void Gmap_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(gmap);
            PointLatLng latLng = gmap.FromLocalToLatLng((int)point.X, (int)point.Y);

            if (_currentGroup != null && MainWindow.CurrentUser.Id == _currentGroup.Admin.Id)
            {
                RulesWindow rulesWindow = new RulesWindow(_currentGroup.Id, latLng.Lat, latLng.Lng, this);
                rulesWindow.Owner = _mainWindow;
                rulesWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                rulesWindow.ShowDialog();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Would you like to update your location to:\nLatitude: {latLng.Lat:F6}\nLongitude: {latLng.Lng:F6}?",
                    "Update Location",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    UserDB userDb = new UserDB();
                    User currentUser = MainWindow.CurrentUser;
                    currentUser.Latitude = latLng.Lat;
                    currentUser.Longitude = latLng.Lng;
                    userDb.Update(currentUser);
                    int rows = BaseDB.SaveChanges();

                    if (rows > 0)
                    {
                        ShowUsersAndRulesOnMap();
                        MessageBox.Show("Location updated successfully!", "Success",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to update location.", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the group creation button click event to navigate to the group creation page.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void GroupButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainFrame.Navigate(new GroupCreationPage(_mainWindow));
        }

        /// <summary>
        /// Handles the list members button click event to show the group members window.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void listMembers_Click(object sender, RoutedEventArgs e)
        {
            Group SelectedGroup = groupSelector.SelectedItem as Group;

            if (SelectedGroup == null)
            {
                MessageBox.Show("Please select a group first.", "No Group Selected",
                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            GroupMembersWindow membersWindow = new GroupMembersWindow(SelectedGroup.Id, this);
            membersWindow.Owner = _mainWindow;
            membersWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            membersWindow.ShowDialog();
        }

        /// <summary>
        /// Handles the sign out button click event to confirm and sign out the user.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to sign out?",
                                                     "Confirm Sign Out",
                                                     MessageBoxButton.YesNo,
                                                     MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _mainWindow.SignOut();
            }
        }

        /// <summary>
        /// Updates the map's center location to the specified latitude and longitude.
        /// </summary>
        /// <param name="latitude">The latitude to center the map on.</param>
        /// <param name="longitude">The longitude to center the map on.</param>
        public void UpdateMapLocation(double latitude, double longitude)
        {
            var location = new PointLatLng(latitude, longitude);
            gmap.Position = location;
        }
    }
}
