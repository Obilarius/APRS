using ARPS.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ARPS.Views
{
    /// <summary>
    /// Interaction logic for Schedule.xaml
    /// </summary>
    public partial class Schedule : UserControl
    {
        public Schedule()
        {
            InitializeComponent();
            this.DataContext = new ScheduleViewModel();
        }

        #region Abort Selection

        /// <summary>
        /// Vehindert das Selectieren eines Eintrags
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dg = sender as DataGrid;
            dg.UnselectAllCells();
        }

        /// <summary>
        /// Vehindert das Selectieren eines Eintrags
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dg = sender as ListView;
            dg.UnselectAll();
        }

        #endregion

    }
}
