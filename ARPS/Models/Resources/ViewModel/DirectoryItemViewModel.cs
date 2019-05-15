using ARPS.ViewModels;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ARPS
{
    public class DirectoryItemViewModel : BindableBase
    {
        #region Public Propertys

        #region ACEs
        /// <summary>
        /// Alle ACEs des Ordners
        /// </summary>
        public List<DirectoryACE> ACEs { get; private set; }

        /// <summary>
        /// Alle berechtigten User des Ordners. Die berechtigten Gruppen werden rekursiv ausgelesen bis nur noch User in der Liste sind
        /// </summary>
        public List<DirectoryACE> AllAuthorizedUserACE { get; private set; }

        public List<DirectoryACE> NTFSRights
        {
            get
            {
                if (AllAuthorizedUserACE == null)
                    return null;

                var dis = AllAuthorizedUserACE.Distinct().ToList();

                var dis2 = (
                    from ace in AllAuthorizedUserACE
                    orderby ace.IdentityName, ace.Rights descending
                    group ace by new
                    {
                        ace.SID,
                        ace.Rights,
                        ace.Type,
                        ace.IsInherited,
                        ace.InheritanceFlags,
                        ace.PropagationFlags
                    } into aceg
                    select aceg.First()
                ).ToList();

                return dis2;
            }
        }

        /// <summary>
        /// AccountsWithPermission die in der gleichnamigen Tabelle angezeigt werden
        /// </summary>
        public List<AccountWithPermissions> AccountsWithPermission { get; private set; }

        #endregion

        /// <summary>
        /// Das DirectoryItem das hinter dem ViewModel steht
        /// </summary>
        public DirectoryItem Item { get; set; }

        /// <summary>
        /// Der Name des Ordners
        /// </summary>
        public string Name { get { return Item.Name; } }

        public ADElement Owner { get; set; }

        /// <summary>
        /// Der Name des Owners mit dem Pricipal oder SamAccountName in Klammern
        /// </summary>
        public string OwnerNameWithPricipal
        {
            get
            {
                if (Owner == null)
                    return null;

                return (Owner.Type == ADElementType.User || Owner.Type == ADElementType.Administrator)
                    ? $"{Owner.Name} ({Owner.PricipalName})"
                    : $"{Owner.Name} ({Owner.SamAccountName})";
            }
        }

        /// <summary>
        /// Die Größe des Ordners in der jeweilig besten lesbaren Größe (TB, GB, MB, KB, Byte)
        /// </summary>
        public string ReadableSize {
            get
            {
                if (Item.Type == DirectoryItemType.Server)
                    return "";
                
                return DirectoryStructure.GetBytesReadable(Item.Size);
            }
        }

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

            Owner = ADStructure.GetADElement(Item.Owner);
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

        /// <summary>
        /// Ruft die ACEs ab und speichert sie im ACEs Property
        /// </summary>
        public void FillACEs()
        {
            ACEs = new List<DirectoryACE>(DirectoryStructure.GetACEs(Item.Id));
        }

        /// <summary>
        /// Ruft die ACEs der User ab (aufgelöste Gruppen) und speichert sie
        /// </summary>
        public void FillAllAuthorizedUserACE()
        {
            //Prüft ob die ACEs schon gefüllt sind
            if (ACEs == null)
                FillACEs();

            AllAuthorizedUserACE = new List<DirectoryACE>(DirectoryStructure.GetAllAuthorizedUser(ACEs));
        }


        /// <summary>
        /// Füllt das Property AccountsWithPermissions mit einer Zusammenfassung der User die Berechtigt sind.
        /// Beinhaltet Count (Wie oft idt der User berechtigt) und InheritedCount (Wie oft ist ein User durch Vererbung berechtigt)
        /// </summary>
        public void FillAccountWithPermissons()
        {
            // Falls die ACE nocht nicht vorhanden sind werden diese gefüllt
            if (AllAuthorizedUserACE == null)
                FillAllAuthorizedUserACE();

            // Fragt die ACE ab und Gruppiert und sortiert die Liste
            var ret = from ul in AllAuthorizedUserACE
                      group ul by new
                      {
                          ul.IdentityName,
                          ul.SID
                      } into gul
                      orderby gul.Key.IdentityName
                      select new
                      {
                          gul.Key.SID,
                          gul.Key.IdentityName,
                          Count = gul.Count(),
                          InheritedCount = gul.Sum(p => (p.IsInherited) ? 1 : 0)
                      };

            // Legt für das Property eine leere Liste an
            this.AccountsWithPermission = new List<AccountWithPermissions>();

            // geht über alle Einträge in der gefilterten und sortierten Liste und fügt sie dem Property hinzu.
            foreach (var user in ret)
            {
                // Prüft ob User Administrator ist
                bool isAdmin = ADStructure.IsUserAdmin(user.SID);
                ADElementType type = (isAdmin) ? ADElementType.Administrator : ADElementType.User;

                this.AccountsWithPermission.Add(new AccountWithPermissions(type, user.IdentityName, user.Count, user.InheritedCount, user.SID));
            }
        }


        #endregion

        #region Expand
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
        #endregion
    }
}
