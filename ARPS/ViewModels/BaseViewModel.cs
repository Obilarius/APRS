using System.ComponentModel;

namespace ARPS.ViewModels
{
    /// <summary>
    /// Basis ViewModel das PropertyChanged ausführt wenn es gebraucht wird.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Das Event wird ausgeführt wenn ein Child property geändert wird.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
