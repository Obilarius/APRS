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


        private static DirectoryItem selectedItem;
        /// <summary>
        /// Das ausgewählte Item
        /// </summary>
        public static DirectoryItem SelectedItem {
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
        public ResourcesViewModel()
        {
            Items = new ObservableCollection<DirectoryItemViewModel>();
            List<DirectoryItem> servers = DirectoryStructure.GetServers();

            foreach (DirectoryItem server in servers)
            {
                Items.Add(new DirectoryItemViewModel(server));
                SelectedItem = server;
            }

            //SelectedItemChangeEvent event = EventAggregator.Instance.GetEvent<SelectedItemChangeEvent>();
            //event.Subscribe(selectedChange);

            
        }

        #endregion

        /// <summary>
        /// Diese Funktion wird aufgerufen wenn sich das Selectierte Item ändert
        /// </summary>
        /// <param name="item"></param>
        protected virtual void selectedChange(object sender, EventArgs e)
        {
            
        }
    }
}
