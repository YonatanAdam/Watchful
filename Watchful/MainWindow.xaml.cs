using Model;
using System.Windows;

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
            MainFrame.Navigate(new LoginPage(this));
        }
    }
}
