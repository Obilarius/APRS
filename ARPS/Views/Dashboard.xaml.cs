using ARPS.ViewModels;
using System.Windows.Controls;

namespace ARPS.Views
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        public Dashboard()
        {
            InitializeComponent();
            this.DataContext = new DashboardViewModel();
        }

        /// <summary>
        /// Verhindet das selektieren eines Eintrags
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dg = sender as DataGrid;
            dg.UnselectAllCells();
        }
    }
}
