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
        #region Public Propertys

        /// <summary>
        /// Der Ordnername der in der rechten Übersicht angezeigt wird
        /// </summary>
        public string FolderName { get; set; } = "-";

        /// <summary>
        /// Der volle Pfad des Ordners der in der rechten Übersicht angezeigt wird
        /// </summary>
        public string FullPath { get; set; } = "-";

        /// <summary>
        /// Der Besitzer des Ordners der in der rechten Übersicht angezeigt wird
        /// </summary>
        public string Owner { get; set; } = "-";

        /// <summary>
        /// Eine Liste mit allen Ordnern
        /// </summary>
        public ObservableCollection<DirectoryItemViewModel> Items { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ResourcesViewModel()
        {
            // Erstelle eine Liste mit Servernamen die angezeigt werden sollen
            var servers = new List<string>
            {
                "Apollon"
            };

            // Holt eine Liste mit dem Server und den SharedFolders
            var nodes = DirectoryStructure.GetServers(servers);
            // Erstellt die ViewModels für die Nodes
            this.Items = new ObservableCollection<DirectoryItemViewModel>(
                nodes.Select(c => new DirectoryItemViewModel(c.Id, c.FullPath, c.Type, c.Owner)));
        }

        #endregion
    }
}
