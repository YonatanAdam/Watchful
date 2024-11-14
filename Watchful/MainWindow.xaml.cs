using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using GMap.NET;
using System.Windows;
using System.Net;
using System;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Navigation;

namespace Watchful
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // private DispatcherTimer _zoomUpdateTimer;

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new LoginPage(this));
        }
    }
}
