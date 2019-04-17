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
        private void UserInfos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dg = sender as DataGrid;
            dg.UnselectAllCells();
        }

        #endregion

        #region DateTime Check

        /// <summary>
        /// Überprüft ob das EndDatum nach dem Start Datum liegt und auch nach dem aktuellen Datum
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Frm_end_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // Die aktuelle Zeit und Datum
            DateTime now = DateTime.Now;
            // Hollt das Datum aus dem DatePicker für Start
            var startDate = ((DatePicker)this.FindName("frm_start")).SelectedDate;

            // Hollt den DatePicker für das Enddatum
            var endDate_Picker = (DatePicker)this.FindName("frm_end");
            // Hollt das Datum aus dem DatePicker für Ende
            var endDate = endDate_Picker.SelectedDate;

            if (endDate == null)
                return;

            // Fehlermeldungsprüfungen
            // Wenn das Enddatum vor dem aktuellen liegt
            if (endDate < now)
            {
                MessageBox.Show("Das Enddatum darf nicht in der Vergangenheit liegen.", "Datumsfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                endDate_Picker.SelectedDate = null;
            }
            // Wenn das Enddatum vor dem Startdatum liegt
            else if (endDate < startDate)
            {
                MessageBox.Show("Das Enddatum darf nicht vor dem Startdatum liegen.", "Datumsfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                endDate_Picker.SelectedDate = null;
            }
            
        }

        /// <summary>
        /// Überprüft ob das StartDatum vor dem End Datum liegt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Frm_start_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // Hollt den DatePicker für das Startdatum
            var startDate_Picker = (DatePicker)this.FindName("frm_start");
            // Hollt das Datum aus dem DatePicker für Start
            var startDate = startDate_Picker.SelectedDate;

            // Hollt das Datum aus dem DatePicker für Ende
            var endDate = ((DatePicker)this.FindName("frm_end")).SelectedDate;

            // Fehlermeldungsprüfungen
            if (endDate == null)
                return;
            // Wenn das Startdatum nach dem Enddatum liegt
            if (startDate > endDate)
            {
                MessageBox.Show("Das Startdatum darf nicht nach dem Enddatum liegen.", "Datumsfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                startDate_Picker.SelectedDate = null;
            }
        }

        #endregion
    }
}
