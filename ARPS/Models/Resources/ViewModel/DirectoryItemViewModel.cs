using ARPS.ViewModels;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ARPS
{
    public class DirectoryItemViewModel : BaseViewModel
    {
        #region Public Propertys

        public DirectoryItem Item { get; set; }

        public string Name { get { return Item.Name; } }

        /// <summary>
        /// Eine Liste mit allen Kindelementen (Unterordnern) von diesem Item
        /// </summary>
        public ObservableCollection<DirectoryItemViewModel> Children { get; set; }

        /// <summary>
        /// Sagt uns ob das aktuelle Element Kinder hat und dammit aufgeklappt werden kann
        /// </summary>
        public bool CanExpand { get { return Item.HasChildren; } }

        /// <summary>
        /// Sagt uns ob das aktuelle Item aufgeklappt ist oder nicht
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return Children?.Count(f => f != null) > 0;
            }
            set
            {
                // Wenn uns das UI mitteilt das Verzeichnis aufzuklappen
                if (value == true)
                    // Finde alle Kinder
                    Expand();
                // Wenn uns das UI mitteilt das Verzeichnis einzuklappen
                else
                    this.ClearChildren();
            }
        }

        #endregion

        #region Selected Item

        private bool _isSelected;
        /// <summary>
        /// Sagt uns ob das aktuelle Item selektiert ist oder nicht
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    if (_isSelected)
                    {
                        SelectedItem = this;
                    }
                }
            }
        }

        private DirectoryItemViewModel _selectedItem = null;
        /// <summary>
        /// Speichert das selektierte Item 
        /// </summary>
        public DirectoryItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            private set
            {
                // Falls ein anderes Item selektiert wurde
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnSelectedItemChanged();
                }
            }
        }

        /// <summary>
        /// Wird ausgeführt wenn ein anderes Item selektiert wurde
        /// </summary>
        private void OnSelectedItemChanged()
        {
           // ResourcesViewModel.selectedChange(Item);
        }

        #endregion

        #region Public Commands

        /// <summary>
        /// Der Befehl um das Item aufzuklappen
        /// </summary>
        public ICommand ExpandCommand { get; set; }

        #endregion

        #region Constructor

        private readonly IEventAggregator eventAggregator;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public DirectoryItemViewModel(DirectoryItem item, IEventAggregator eventAggregator)
        {
            // Erstelle Commands
            this.ExpandCommand = new RelayCommand(Expand);

            // Setze Propertys
            this.Item = item;
            this.eventAggregator = eventAggregator;

            // Falls der Ordner aufgeklappt werden kann wird ein Dummy Element als Child gesetzt
            if (CanExpand)
            {
                Children = new ObservableCollection<DirectoryItemViewModel>
                {
                    null
                };
            }
                
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Löscht alle Kinder in der Liste. Fügt ein Dummy Item hinzu damit das Symbol zum aufklappen angezeit wird, falls es nötig ist.
        /// </summary>
        private void ClearChildren()
        {
            // Prüft ob das aktuelle Item aktuell Kinder hat
            bool hasChilds = (Children.Count() > 0) ? true : false;

            // Löscht alle Items aus der Liste
            Children = new ObservableCollection<DirectoryItemViewModel>();

            // Fügt ein Dummy Item hinzu, falls das Item vorher Kinder hatte
            if(hasChilds)
                this.Children.Add(null);
        }

        #endregion

        /// <summary>
        /// Klappt das Verzeichnis auf und findet alle Kinder
        /// </summary>
        private void Expand()
        {
            // Item hat keine Kinder und kann daher nicht aufgeklappt werden
            if (CanExpand == false)
                return;

            // Finde alle Kinder
            List<DirectoryItem> children = (Item.Type == DirectoryItemType.Server) ? 
                DirectoryStructure.GetChildren(Item.FullPath) : DirectoryStructure.GetChildren(Item.Id);
            Children = new ObservableCollection<DirectoryItemViewModel>(
                children.Select(c => new DirectoryItemViewModel(c, eventAggregator)));
        }
    }
}
