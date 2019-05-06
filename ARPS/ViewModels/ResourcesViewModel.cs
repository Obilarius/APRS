using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ARPS.ViewModels
{
    /// <summary>
    /// Das ViewModel für die Unterseite "Resourcen"
    /// </summary>
    public class ResourcesViewModel : BindableBase
    {
        

        /// <summary>
        /// Die Liste die alle Items hält
        /// </summary>
        public ObservableCollection<DirectoryItemViewModel> Items { get; set; }


        private readonly IEventAggregator eventAggregator;


        private DirectoryItemViewModel selectedItem;

        /// <summary>
        /// Das ausgewählte Item
        /// </summary>
        public DirectoryItemViewModel SelectedItem {
            get => selectedItem;
            set
            {
                selectedItem = value;
            }
        }


        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ResourcesViewModel(IEventAggregator eventAggregator)
        {
            Items = new ObservableCollection<DirectoryItemViewModel>();
            List<DirectoryItem> servers = DirectoryStructure.GetServers();

            foreach (DirectoryItem server in servers)
            {
                var newItem = new DirectoryItemViewModel(server, eventAggregator);
                Items.Add(newItem);
                SelectedItem = newItem;
            }

            this.eventAggregator = eventAggregator;

            // Abboniert das Event wenn sich das SelectedItem ändert
            eventAggregator.GetEvent<PubSubEvent<DirectoryItemViewModel>>().Subscribe(SelectedItemChange);

        }

        #endregion

        /// <summary>
        /// Wird ausgeführ wenn ein neues Item Selektiert wird
        /// </summary>
        /// <param name="newSelectedItem"></param>
        private void SelectedItemChange(DirectoryItemViewModel newSelectedItem)
        {
            // Falls das neue Item dem alten entspricht wird nichts gemacht
            if (newSelectedItem == null || newSelectedItem == SelectedItem)
                return;

            // Setzt das Selected Item auf das neue Item
            SelectedItem = newSelectedItem;
        }


    }
}
