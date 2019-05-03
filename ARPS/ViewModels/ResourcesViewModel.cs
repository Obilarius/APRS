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

        }

        #endregion
    }
}
