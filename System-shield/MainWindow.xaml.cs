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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace System_shield
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PagesNavigation.Navigate(new Uri("pages/home.xaml", UriKind.Relative));

        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void rdHome_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(new System.Uri("pages/home.xaml", UriKind.RelativeOrAbsolute));
        }

        private void rdSounds_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(new System.Uri("pages/toolbox.xaml", UriKind.RelativeOrAbsolute));
        }

        private void rdNotes_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(new System.Uri("pages/Plug-in management.xaml", UriKind.RelativeOrAbsolute));
        }

        private void rdPayment_Click(object sender, RoutedEventArgs e)
        {

        }
        private void PagesNavigation_Navigated(object sender, NavigationEventArgs e)
        {
            Page newPage = e.Content as Page;
            if (newPage != null)
            {
                newPage.Loaded += NewPage_Loaded;
            }
        }
        private void NewPage_Loaded(object sender, RoutedEventArgs e)
        {
            Page newPage = sender as Page;
            if (newPage != null)
            {
                newPage.Loaded -= NewPage_Loaded;

                TranslateTransform translateTransform = new TranslateTransform();
                newPage.RenderTransform = translateTransform;

                DoubleAnimation slideUpAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = -70,
                    Duration = TimeSpan.FromSeconds(0.3),
                    EasingFunction = new PowerEase { EasingMode = EasingMode.EaseInOut, Power = 3 }
                };

                translateTransform.BeginAnimation(TranslateTransform.YProperty, slideUpAnimation);
            }
        }

        public class ServerInfo
        {
            public string Address { get; internal set; }
            public string Latency { get; internal set; }
        }
    }

}
