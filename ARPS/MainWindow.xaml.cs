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
using ARPS.ViewModels;
using MahApps.Metro.Controls;

namespace ARPS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new DashboardViewModel();
        }

        private void Button_Dashboard_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new DashboardViewModel();
            ResetButtonColor();
            btn_dashboard.Background = (Brush)Application.Current.FindResource("ArgesGrauBlau_mittel");
        }

        private void Button_Ressources_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new ResourcesViewModel();
            ResetButtonColor();
            btn_ressources.Background = (Brush)Application.Current.FindResource("ArgesGrauBlau_mittel");
        }

        private void Button_Permissions_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new PermissionsViewModel();
            ResetButtonColor();
            btn_permissions.Background = (Brush)Application.Current.FindResource("ArgesGrauBlau_mittel");
        }

        private void Button_Schedule_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new ScheduleViewModel();
            ResetButtonColor();
            btn_schedule.Background = (Brush)Application.Current.FindResource("ArgesGrauBlau_mittel");
        }

        private void ResetButtonColor()
        {
            btn_dashboard.Background = (Brush)Application.Current.FindResource("ArgesGrauBlau_dunkel");
            btn_permissions.Background = (Brush)Application.Current.FindResource("ArgesGrauBlau_dunkel");
            btn_ressources.Background = (Brush)Application.Current.FindResource("ArgesGrauBlau_dunkel");
            btn_schedule.Background = (Brush)Application.Current.FindResource("ArgesGrauBlau_dunkel");
        }
    }
}
