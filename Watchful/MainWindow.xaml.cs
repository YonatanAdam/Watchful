using Model;
using System.Windows;
using System.Windows.Navigation;

namespace Watchful
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Current logged in user
        public static User CurrentUser { get; set; }
        public static Group CurrentGroup { get; set; } // Not sure if this is needed


        public MainWindow()
        {
            InitializeComponent();
            // Attach handler for navigation event
            MainFrame.NavigationService.Navigated += NavigationService_Navigated;
            MainFrame.Navigate(new LoginPage(this));
        }

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is MapPage mapPage)
            {
                mapPage.ShowUsersOnMap();
            }
        }

    }
}
