using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ARPS.ViewModels
{
    public class PermissionsViewModel : BaseViewModel
    {
        #region Commands
        /// <summary>
        /// Der Command wenn der Benutzer nach einem User suchen will
        /// </summary>
        public ICommand SearchUserCommand { get; set; }

        /// <summary>
        /// Der Command um die Suche nach einem User zu löschen
        /// </summary>
        public ICommand ClearSearchUserCommand { get; set; }

        /// <summary>
        /// Command wird ausgeführt wenn sich der selektierte User ändert
        /// </summary>
        public ICommand UserSelectionChangedCommand { get; set; }
        #endregion

        /// <summary>
        /// Hält alle AD User
        /// </summary>
        public List<ADElement> AllUsers { get; set; }

        /// <summary>
        /// Hält die gefilterten AD User
        /// </summary>
        public ObservableCollection<ADElement> UsersFiltered { get; set; }

        /// <summary>
        /// Der selektierte User
        /// </summary>
        private ADElement _selectedUser { get; set; }
        public ADElement SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (value == _selectedUser)
                    return;

                _selectedUser = value;

                if (value != null)
                    // Ruft die Funktion die zb. die UserInfos aktualisiert
                    UserSelectionChanged();
            }
        }

        /// <summary>
        /// Anzeigetiefe der Ordnerpfade
        /// </summary>
        public int ViewDeepth { get; set; }


        #region Search
        /// <summary>
        /// Der letzte Suchtext der gesucht wurde
        /// </summary>
        private string LastSearchTextUser { get; set; }

        /// <summary>
        /// Der Suchtext nach dem die User gefiltert werden
        /// </summary>
        public string SearchTextUser { get; set; }
        #endregion

        #region Konstruktor
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PermissionsViewModel()
        {
            // Erstelle Commands
            SearchUserCommand = new RelayCommand(SearchUser);
            ClearSearchUserCommand = new RelayCommand(ClearSearchUser);

            // Erstellt die beiden Listen für die User
            AllUsers = new List<ADElement>();
            UsersFiltered = new ObservableCollection<ADElement>();

            AsyncLoadedAllUsers();

            // Setze Suchtiefe bei Start auf 3
            ViewDeepth = 3;
        }
        #endregion

        #region Async Function Calls

        /// <summary>
        /// Ruft async alle User ab
        /// </summary>
        private async void AsyncLoadedAllUsers()
        {
            await GetAllADUsers();
            AllUsers = new List<ADElement>(UsersFiltered);
        }

        /// <summary>
        /// Liste mit allen aktivien Usern zurück
        /// </summary>
        /// <returns></returns>
        private Task GetAllADUsers()
        {
            return Task.Factory.StartNew(() =>
            {

                // Erstellt den Eintrag der während des Ladens angezeigt wird
                ADElement loading = new ADElement("loading...");

                // Fügt den Lade User der Liste hinzu die angezeigt wird
                UsersFiltered = new ObservableCollection<ADElement>
                {
                    loading
                };

                List<ADElement> AllUsers = ADStructure.GetAllADUsers(true);

                // Sortiert die Liste Aller User
                AllUsers.Sort((x, y) => x.Name.CompareTo(y.Name));

                // Kopiert zum Start alle User in die gefilterte Liste
                UsersFiltered = new ObservableCollection<ADElement>(AllUsers);
            });
        }

        #endregion


        public PermissionItemCollection PermissionItemColl { get; set; }
        /// <summary>
        /// Wenn ein neuer User selektiert wird
        /// </summary>
        public void UserSelectionChanged()
        {
            PermissionItemColl = new PermissionItemCollection(SelectedUser.SID);
            PermissionItemColl.FillItemsWithShares();




            //// Erstellt eine neue Liste mit den neuen Infos
            //UserInfos = new ObservableCollection<UserInfoEntry>
            //{
            //    new UserInfoEntry("Name", SelectedUser.DisplayName),
            //    new UserInfoEntry("Email", SelectedUser.EmailAddress),
            //    new UserInfoEntry("Letzter Login", SelectedUser.LastLogon.ToString())
            //};

            //List<GroupPrincipal> tmpList = new List<GroupPrincipal>(); ;
            //// Liest alle Gruppen aus in dennen der SelectedUser Mitglied ist
            //PrincipalSearchResult<Principal> grp = SelectedUser.GetGroups();
            //// Geht über alle Gruppen und fügt sie zur Liste hinzu
            //foreach (var g in grp)
            //{
            //    tmpList.Add(g as GroupPrincipal);
            //}
            //// Sortiert die Gruppen
            //tmpList.Sort((x, y) => x.Name.CompareTo(y.Name));

            //// Setzt SelectedUserGroups auf die Ausgelesene Liste
            //SelectedUserGroups = new ObservableCollection<GroupPrincipal>(tmpList);
        }

        #region UserSearch

        /// <summary>
        /// Sucht den User und filtert die Ansicht
        /// </summary>
        public void SearchUser()
        {
            // Prüft damit nicht der selbe Text nochmal gesucht wird
            if ((string.IsNullOrEmpty(LastSearchTextUser) && 
                string.IsNullOrEmpty(SearchTextUser)) ||
                string.Equals(LastSearchTextUser, SearchTextUser))
                return;

            // Wenn wir keinen Suchtext haben
            if (string.IsNullOrEmpty(SearchTextUser))
            {
                // Mache gefilterte Liste gleich Allen Usern
                UsersFiltered = new ObservableCollection<ADElement>(AllUsers);

                // Setze letzte Suche
                LastSearchTextUser = SearchTextUser;

                return;
            }

            // Finde alle Items die den Text enthalten
            UsersFiltered = new ObservableCollection<ADElement>(
                AllUsers.Where(x => x.Name.ToLower().Contains(SearchTextUser.ToLower())));

            // Setze letzte Suche
            LastSearchTextUser = SearchTextUser;
        }

        /// <summary>
        /// Löscht die Suche und zeigt wieder alle User an
        /// </summary>
        public void ClearSearchUser()
        {
            //Prüft ob SearchText schon leer ist
            if (!string.IsNullOrEmpty(SearchTextUser))
            {
                // Löscht den Suchtext
                SearchTextUser = string.Empty;

                // Zeigt wieder alle User an
                UsersFiltered = new ObservableCollection<ADElement>(AllUsers);
            }
        }

        #endregion
    }
}
