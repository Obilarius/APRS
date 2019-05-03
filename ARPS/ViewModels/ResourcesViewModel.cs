using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ARPS.ViewModels
{
    /// <summary>
    /// Das ViewModel für die Unterseite "Resourcen"
    /// </summary>
    public class ResourcesViewModel : BaseViewModel
    {
        public ObservableCollection<DirectoryItemViewModel> Items { get; set; }


        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ResourcesViewModel()
        {
            Items = new ObservableCollection<DirectoryItemViewModel>();
            List<DirectoryItem> servers = DirectoryStructure.GetServers();

            foreach (DirectoryItem server in servers)
            {
                Items.Add(new DirectoryItemViewModel(server));
            }
        }

        #endregion
    }
}
