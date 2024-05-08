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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System_shield.pages.toolbox;

namespace System_shield.pages
{
    /// <summary>
    /// home.xaml 的交互逻辑
    /// </summary>
    public partial class home : Page
    {
        public home()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.PagesNavigation.Navigate(new Uri("pages/toolbox/vpn.xaml", UriKind.Relative));
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.PagesNavigation.Navigate(new Uri("pages/toolbox/Microsoft Defender.xaml", UriKind.Relative));
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.PagesNavigation.Navigate(new Uri("pages/toolbox/File transfers.xaml", UriKind.Relative));
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.PagesNavigation.Navigate(new Uri("pages/toolbox/System settings.xaml", UriKind.Relative));
            }
        }
    }
}
