﻿using ARPS.Models.Schedule;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ARPS.ViewModels
{
    /// <summary>
    /// Das ViewModel der Schedule View
    /// Die Klasse beinhaltet die Logik zur Schedule Seite des UI
    /// 
    /// IDataErrorInfo - Validierung - Video[https://www.youtube.com/watch?v=OOHDie8BdGI]
    /// ASYNC / AWAIT - Video[https://www.youtube.com/watch?v=6_GTdR0gBVE]
    /// </summary>
    public class ScheduleViewModel : BindableBase, IDataErrorInfo
    {
        #region Commands

        #region Search
        /// <summary>
        /// Der Command wenn der Benutzer nach einem User suchen will
        /// </summary>
        public ICommand SearchUserCommand { get; set; }

        /// <summary>
        /// Der Command um die Suche nach einem User zu löschen
        /// </summary>
        public ICommand ClearSearchUserCommand { get; set; }

        /// <summary>
        /// Der Command wenn der Benutzer nach einer Gruppe suchen will
        /// </summary>
        public ICommand SearchGroupCommand { get; set; }

        /// <summary>
        /// Der Command um die Suche nach einer Gruppe zu löschen
        /// </summary>
        public ICommand ClearSearchGroupCommand { get; set; }

        #endregion

        /// <summary>
        /// Command wird ausgeführt wenn sich der selektierte User ändert
        /// </summary>
        public ICommand UserSelectionChangedCommand { get; set; }

        #region PlannedGroups

        /// <summary>
        /// Command zum hinzufügen der Gruppe zu den geplanten Gruppen
        /// </summary>
        public ICommand AddGroupToPlanCommand { get; set; }

        /// <summary>
        /// Command zum entfernen einer Gruppe aus den geplannten Gruppen
        /// </summary>
        public ICommand RemoveGroupFromPlanCommand { get; set; }

        #endregion

        /// <summary>
        /// Command zum Eintragen einer Planung
        /// </summary>
        public ICommand SubmitPlanningFormCommand { get; set; }

        /// <summary>
        /// Command zum starten des Deamons
        /// </summary>
        public ICommand DeamonStartCommand { get; set; }

        #region HistoryLog
        /// <summary>
        /// Command zum sotieren des HistoryLogs nach Header
        /// </summary>
        public ICommand HistoryLogHeaderClickCommand { get; set; }

        /// <summary>
        /// Command zum Löschen eines HistoryLogEntrys
        /// </summary>
        public ICommand RemoveHistoryLogEntryCommand { get; set; }
        #endregion

        #endregion

        #region Propertys
        /// <summary>
        /// Der selektierte User
        /// </summary>
        private UserPrincipal _selectedUser { get; set; }
        public UserPrincipal SelectedUser
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
        /// Enthält die Informationen über den selektierten User
        /// </summary>
        public ObservableCollection<UserInfoEntry> UserInfos { get; set; }

        /// <summary>
        /// Die Gruppen in dennen der selektierte User Mitglied ist
        /// </summary>
        public ObservableCollection<GroupPrincipal> SelectedUserGroups { get; set; }

        /// <summary>
        /// Die Gruppen die geplant sind
        /// </summary>
        public ObservableCollection<GroupPrincipal> PlannedGroups { get; set; }

        /// <summary>
        /// Die Liste aller geplanten Gruppenmitgliedschaften aus der Datenbank
        /// </summary>
        public ObservableCollection<HistoryLogEntry> HistoryLog { get; set; }

        /// <summary>
        /// Nach welcher Spalte wurde sortiert
        /// </summary>
        private string SortedHeader { get; set; }
        /// <summary>
        /// Welche Richtung wurde sortiert ASC (1) oder DESC (-1)
        /// </summary>
        private int SortedDirection { get; set; }

        public bool FormEnabled { get; private set; } = true;

        #region Form Properties

        /// <summary>
        /// Startdatum
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Enddatum
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);

        /// <summary>
        /// Falls HasNoEnd true ist wird der EndDate DatePicker deaktiviert damit das Datum nicht geändrt werden kann
        /// </summary>
        public bool EndDateEnabled { get; set; } = true;

        /// <summary>
        /// Im DatePicker Kalender werden nur Daten nach diesem Datum angezeigt
        /// </summary>
        public DateTime DisplayStartDateEnd { get; set; } = DateTime.Now;

        /// <summary>
        /// Die Checkbox die aussagt ob die Gruppe wieder automatisch entfernt werden soll oder nicht
        /// </summary>
        public bool HasNoEnd { get; set; }

        /// <summary>
        /// Die Initialen des Users der die Planung durchgeführt hat
        /// </summary>
        public string EditorInitials { get; set; }

        /// <summary>
        /// Ein Kommentar wieso diese Gruppenplanung so ist wie sie ist
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Fehlermeldungen die beim absenden des Forms auftreten
        /// </summary>
        public string FormErrorMessage { get; set; }

        #endregion

        #region User and Group Lists

        /// <summary>
        /// Hält alle AD User
        /// </summary>
        public List<UserPrincipal> AllUsers { get; set; }

        /// <summary>
        /// Hält die gefilterten AD User
        /// </summary>
        public ObservableCollection<UserPrincipal> UsersFiltered { get; set; }

        /// <summary>
        /// Hält alle AD Gruppen
        /// </summary>
        public List<GroupPrincipal> AllGroups { get; set; }

        /// <summary>
        /// Hält die gefilterten AD Gruppen
        /// </summary>
        public ObservableCollection<GroupPrincipal> GroupsFiltered { get; set; }

        #endregion

        #region SearchText

        /// <summary>
        /// Der letzte Suchtext der gesucht wurde
        /// </summary>
        private string LastSearchTextUser { get; set; }

        /// <summary>
        /// Der Suchtext nach dem die User gefiltert werden
        /// </summary>
        public string SearchTextUser { get; set; }

        /// <summary>
        /// Der letzte Suchtext der gesucht wurde
        /// </summary>
        private string LastSearchTextGroup { get; set; }

        /// <summary>
        /// Der Suchtext nach dem die Gruppen gefiltert werden
        /// </summary>
        public string SearchTextGroup { get; set; }

        #endregion

        #endregion

        #region Form Validation

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error
        {
            get
            {
                return null;
            }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                return GetValidationError(propertyName);
            }
        }

        #endregion


        /// <summary>
        /// Alle FormularFelder die überprüft werden müssen
        /// </summary>
        static readonly string[] ValidatedProperties =
        {
            "StartDate",
            "EndDate",
            "HasNoEnd",
            "EditorInitials",
            "Comment"
        };


        /// <summary>
        /// Das Property das aussagt ob ein Feld Valid oder nicht ist
        /// </summary>
        public bool IsValid
        {
            get
            {
                foreach (string property in ValidatedProperties)
                {
                    if (GetValidationError(property) != null)
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Prüft das übergebene Feld auf Fehler
        /// </summary>
        /// <param name="propertyName">PropertyName der im static string[] ValidatedProperties beinhaltet ist</param>
        /// <returns>Null oder den ErrorText</returns>
        private string GetValidationError(string propertyName)
        {
            string error = null;

            switch (propertyName)
            {
                case "EditorInitials":
                    error = ValidateInitials();
                    break;
                case "StartDate":
                    error = ValidateStartDate();
                    break;
                case "EndDate":
                    error = ValidateEndDate();
                    break;
                case "HasNoEnd":
                    error = ValidateHasNoEnd();
                    break;
                case "Comment":
                    error = ValidateComment();
                    break;
                default:
                    break;
            }

            return error;
        }


        #region Property Validate Functions

        /// <summary>
        /// Validierung der Initialen
        /// </summary>
        /// <returns></returns>
        private string ValidateInitials()
        {   
            // Überprüft obdie Initialen nicht leer sind
            if (String.IsNullOrWhiteSpace(EditorInitials))
                return "Initialen dürfen nicht leer sein.";

            return null;
        }

        /// <summary>
        /// Validierung des Start Datum
        /// </summary>
        /// <returns></returns>
        private string ValidateStartDate()
        {
            return null;
        }

        /// <summary>
        /// Validierung des End Datum
        /// </summary>
        /// <returns></returns>
        private string ValidateEndDate()
        {
            // Überprüft das Ende nicht vor dem Start ist
            if (EndDate <= StartDate)
                return "Das Enddatum muss nach dem Startdatum liegen.";

            // Überprüft das Ende nicht in der Vergangenheit liegt 
            if (EndDate <= DateTime.Now)
                return "Das Enddatum muss in der Zukunft liegen.";

            return null;
        }

        /// <summary>
        /// Validierung der Checkbox HasNoEnd. Ist immer Valid aber bei true wird hier das EndDatum auf DateTime.MaxValue gesetzt
        /// </summary>
        /// <returns></returns>
        private string ValidateHasNoEnd()
        {
            // Falls true wird das Enddatum auf MaxValue gesetzt
            if (HasNoEnd == true)
            {
                EndDate = DateTime.MaxValue;
                EndDateEnabled = false;
            }
            // Falls false wird das Enddatum wieder auf Morgen gesetzt
            else
            {
                if (EndDate == DateTime.MaxValue)
                    EndDate = DateTime.Now.AddDays(1);
                EndDateEnabled = true;
            }

            return null;
        }

        /// <summary>
        /// Validierung des Kommentars
        /// </summary>
        /// <returns></returns>
        private string ValidateComment()
        {
            // Überprüft das der Kommentar ausgefüllt und nicht leer ist
            if (String.IsNullOrWhiteSpace(Comment))
                return "Kommentar darf nicht leer sein.";

            return null;
        }

        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// Standard Konstruktor
        /// </summary>
        public ScheduleViewModel()
        {
            // Erstelle Commands
            SearchUserCommand = new RelayCommand(SearchUser);
            ClearSearchUserCommand = new RelayCommand(ClearSearchUser);
            SearchGroupCommand = new RelayCommand(SearchGroup);
            ClearSearchGroupCommand = new RelayCommand(ClearSearchGroup);
            AddGroupToPlanCommand = new RelayCommand<GroupPrincipal>(AddGroupToPlan);
            RemoveGroupFromPlanCommand = new RelayCommand<GroupPrincipal>(RemoveGroupFromPlan);
            SubmitPlanningFormCommand = new RelayCommand(SubmitPlanningForm);
            DeamonStartCommand = new RelayCommand(DeamonStart);
            HistoryLogHeaderClickCommand = new RelayCommand<string>(HistoryLogHeaderClick);
            RemoveHistoryLogEntryCommand = new RelayCommand<HistoryLogEntry>(RemoveHistoryLogEntry);

            // Erstellt die beiden Listen für die User
            AllUsers = new List<UserPrincipal>();
            UsersFiltered = new ObservableCollection<UserPrincipal>();

            // Erstellt die beiden Listen für die Gruppen
            AllGroups = new List<GroupPrincipal>();
            GroupsFiltered = new ObservableCollection<GroupPrincipal>();

            AsyncLoadedAllUsers();
            AsyncLoadedAllGroups();

            // Hollt sich die geplanten Mitgliedschaften aus der Dantenbank
            HistoryLog = new ObservableCollection<HistoryLogEntry>();
            // Ruft die übergeben Anzahl an Einträgen ab
            AsyncLoadedHistoryLog(100);
        }
        #endregion

        #region Deamon
        /// <summary>
        /// Leitet den Aufrud an die ScheduleDeamon Klasse weiter und startet den Deamon
        /// </summary>
        public void DeamonStart()
        {
            // Setzt den Cursor auf die Sanduhr
            Mouse.OverrideCursor = Cursors.Wait;

            // Ruft die Funktion der ScheduleDeamon Klasse die den Deamon startet
            ScheduleDeamon.Start();

            // Löd das HistoryLog neu
            AsyncLoadedHistoryLog(100);

            // Setzt den Cursor zurück auf Standard
            Mouse.OverrideCursor = null;
        }
        #endregion

        #region Async Function Calls

        /// <summary>
        /// Ruft async alle User ab
        /// </summary>
        private async void AsyncLoadedAllUsers()
        {
            try
            {
                await GetAllADUsers();
            }
            catch (Exception ex)
            {
                NoConnectionToAD(ex);
            }
        }

        /// <summary>
        /// Ruft async alle Gruppen ab
        /// </summary>
        private async void AsyncLoadedAllGroups()
        {
            try
            {
                await GetAllADGroups();
            }
            catch (Exception ex)
            {
                NoConnectionToAD(ex);
            }
        }

        /// <summary>
        /// Ruft async die History Logs aus der Datenbank ab
        /// </summary>
        private async void AsyncLoadedHistoryLog(int showCount)
        {
            await GetPlanFromMSSQL(showCount);
        }

        #endregion

        #region SUBMIT

        /// <summary>
        /// Methode die Ausgeführt wird sobald der Button zum Mitgliedschaft plannen geklickt wird
        /// </summary>
        public void SubmitPlanningForm ()
        {
            // Löscht die Fehlermeldung
            FormErrorMessage = String.Empty;

            // Geht über alle Formularfelder und überprüft sie.
            foreach (var formProperty in ValidatedProperties)
            {
                // Falls bei einer Validationsprüfung eine Fehlermeldung zurück kommt wird folgende Nachricht ausgegeben
                if (GetValidationError(formProperty) != null)
                {
                    FormErrorMessage = "Bitte füllen Sie erst alle Felder richtig aus.";
                    return;
                }
            }

            PlannedScheduleToMSSQL();
        }


        /// <summary>
        /// Schreibt für jede geplannte Gruppe einen Datensatz in die Datenbank
        /// </summary>
        private void PlannedScheduleToMSSQL()
        {
            // Erstellt eine Datenbankverbindung
            var mssql = new MsSql();
            // Öffnet die Datenbankverbindung
            mssql.Open();

            // Schleife über alle Gruppen der geplanten Gruppen
            foreach (var group in PlannedGroups)
            {
                // Der Insert SQL Befehl
                string sql = $"INSERT INTO schedule (Username, UserSid, Groupname, GroupSid, StartDate, EndDate, Creator, Comment, Status) " +
                    $"VALUES (@username, @usersid, @groupname, @groupsid, @startdate, @enddate, @creator, @comment, @status)";

                SqlCommand cmd = new SqlCommand(sql, mssql.Con);

                // Hängt alle SpaltenValues per Parameter an
                cmd.Parameters.AddWithValue("@username", SelectedUser.Name.ToString());
                cmd.Parameters.AddWithValue("@usersid", SelectedUser.Sid.ToString());
                cmd.Parameters.AddWithValue("@startdate", StartDate);
                cmd.Parameters.AddWithValue("@enddate", EndDate);
                cmd.Parameters.AddWithValue("@creator", EditorInitials);
                cmd.Parameters.AddWithValue("@comment", Comment);
                cmd.Parameters.AddWithValue("@status", "planned");
                cmd.Parameters.AddWithValue("@groupname", group.Name.ToString());
                cmd.Parameters.AddWithValue("@groupsid", group.Sid.ToString());
                
                // Führt die Query aus
                cmd.ExecuteNonQuery();
            }

            // Schliest die Datenbankverbindung
            mssql.Close();

            // Lädt die HistoryLogs neu
            AsyncLoadedHistoryLog(100);
        }

        #endregion

        #region PlannedGroup

        /// <summary>
        /// Fügt die übergebene Gruppen zur Liste der geplanten Gruppen hinzu
        /// </summary>
        /// <param name="group">Die Gruppe die geplant werden soll</param>
        public void AddGroupToPlan(GroupPrincipal group)
        {
            // Prüfung ob überhaupt was übergeben wird
            if (group == null)
                return;

            // Wenn es noch keine PlannedGroups gibt, wird eine neue Liste angelegt
            if (PlannedGroups == null)
                PlannedGroups = new ObservableCollection<GroupPrincipal>();

            // Falls die Gruppe schon in der Liste für geplante Gruppen vorhanden ist
            if (PlannedGroups.Contains(group))
                return;

            // Die Übergebene Gruppe wird zur Liste hinzugefügt
            PlannedGroups.Add(group);
        }

        /// <summary>
        /// Löscht die übergebene Gruppe aus der Liste der geplanten Gruppen
        /// </summary>
        /// <param name="group">Die Gruppe die gelöscht werden soll</param>
        public void RemoveGroupFromPlan(GroupPrincipal group)
        {
            // Prüfung ob überhaupt was übergeben wird
            if (group == null)
                return;

            // Löscht die Gruppe aus der Liste
            PlannedGroups.Remove(group);
        }

        #endregion

        #region UserList

        /// <summary>
        /// Wenn ein neuer User selektiert wird
        /// </summary>
        public void UserSelectionChanged()
        {
            // Erstellt eine neue Liste mit den neuen Infos
            UserInfos = new ObservableCollection<UserInfoEntry>
            {
                new UserInfoEntry("Name", SelectedUser.DisplayName),
                new UserInfoEntry("Email", SelectedUser.EmailAddress),
                new UserInfoEntry("Letzter Login", SelectedUser.LastLogon.ToString())
            };

            List<GroupPrincipal> tmpList = new List<GroupPrincipal>(); ;
            // Liest alle Gruppen aus in dennen der SelectedUser Mitglied ist
            PrincipalSearchResult<Principal> grp = SelectedUser.GetGroups();
            // Geht über alle Gruppen und fügt sie zur Liste hinzu
            foreach (var g in grp)
            {
                tmpList.Add(g as GroupPrincipal);
            }
            // Sortiert die Gruppen
            tmpList.Sort( (x,y) => x.Name.CompareTo(y.Name) );

            // Setzt SelectedUserGroups auf die Ausgelesene Liste
            SelectedUserGroups = new ObservableCollection<GroupPrincipal>(tmpList);
        }

        #endregion

        #region UserSearch

        /// <summary>
        /// Sucht den User und filtert die Ansicht
        /// </summary>
        public void SearchUser()
        {
            // Prüft damit nicht der selbe Text nochmal gesucht wird
            if ((string.IsNullOrEmpty(LastSearchTextUser) && string.IsNullOrEmpty(SearchTextUser)) ||
                string.Equals(LastSearchTextUser, SearchTextUser))
                return;

            // Wenn wir keinen Suchtext haben
            if (string.IsNullOrEmpty(SearchTextUser))
            {
                // Mache gefilterte Liste gleich Allen Usern
                UsersFiltered = new ObservableCollection<UserPrincipal>(AllUsers);

                // Setze letzte Suche
                LastSearchTextUser = SearchTextUser;

                return;
            }

            // Finde alle Items die den Text enthalten
            UsersFiltered = new ObservableCollection<UserPrincipal>(
                AllUsers.Where(x => x.Name.ToLower().Contains(SearchTextUser.ToLower())));

            // Setze letzte Suche
            LastSearchTextUser = SearchTextUser;
        }

        /// <summary>
        /// Löscht die Suche und zeigt wieder alle User an
        /// </summary>
        public void ClearSearchUser()
        {
            // Prüft ob SearchText schon leer ist 
            if (!string.IsNullOrEmpty(SearchTextUser))
            {
                // Löscht den Suchtext
                SearchTextUser = string.Empty;

                // Zeigt wieder alle User an
                UsersFiltered = new ObservableCollection<UserPrincipal>(AllUsers);
            }
        }

        #endregion

        #region GroupSearch

        /// <summary>
        /// Sucht die Gruppe und filtert die Ansicht
        /// </summary>
        public void SearchGroup()
        {
            // Prüft damit nicht der selbe Text nochmal gesucht wird
            if ((string.IsNullOrEmpty(LastSearchTextGroup) && string.IsNullOrEmpty(SearchTextGroup)) ||
                string.Equals(LastSearchTextGroup, SearchTextGroup))
                return;

            // Wenn wir keinen Suchtext haben
            if (string.IsNullOrEmpty(SearchTextGroup))
            {
                // Mache gefilterte Liste gleich Allen Gruppen
                GroupsFiltered = new ObservableCollection<GroupPrincipal>(AllGroups);

                // Setze letzte Suche
                LastSearchTextGroup = SearchTextGroup;

                return;
            }

            // Finde alle Items die den Text enthalten
            GroupsFiltered = new ObservableCollection<GroupPrincipal>(
                AllGroups.Where(x => x.Name.ToLower().Contains(SearchTextGroup.ToLower())));

            // Setze letzte Suche
            LastSearchTextGroup = SearchTextGroup;
        }

        /// <summary>
        /// Löscht die Suche und zeigt wieder alle Gruppen an
        /// </summary>
        public void ClearSearchGroup()
        {
            // Prüft ob SearchText schon leer ist 
            if (!string.IsNullOrEmpty(SearchTextGroup))
            {
                // Löscht den Suchtext
                SearchTextGroup = string.Empty;

                // Zeigt wieder alle User an
                GroupsFiltered = new ObservableCollection<GroupPrincipal>(AllGroups);
            }
        }

        #endregion

        #region Get Infos from AD

        /// <summary>
        /// Liest das AD aus und gibt eine Liste mit allen aktivien Usern zurück
        /// </summary>
        /// <returns></returns>
        private Task GetAllADUsers()
        {
            return Task.Factory.StartNew(() =>
            {
                // create your domain context
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
                // define a "query-by-example" principal - here, we search for a UserPrincipal 
                UserPrincipal qbeUser = new UserPrincipal(ctx);

                // Erstellt den Eintrag der während des Ladens angezeigt wird
                UserPrincipal loading = new UserPrincipal(ctx)
                {
                    Name = "loading ..."
                };

                // Fügt den Lade User der Liste hinzu die angezeigt wird
                UsersFiltered = new ObservableCollection<UserPrincipal>
                {
                    loading
                };

                // create your principal searcher passing in the QBE principal    
                PrincipalSearcher srch = new PrincipalSearcher(qbeUser);

                List<UserPrincipal> lst = new List<UserPrincipal>();
                // find all matches
                foreach (var found in srch.FindAll())
                {
                    if (found is UserPrincipal user)
                    {
                        if (user.Enabled.Value)
                            AllUsers.Add(user);
                    }
                }
                // Sortiert die Liste Aller User
                AllUsers.Sort((x, y) => x.Name.CompareTo(y.Name));

                // Kopiert zum Start alle User in die gefilterte Liste
                UsersFiltered = new ObservableCollection<UserPrincipal>(AllUsers);
            });
        }

        /// <summary>
        /// Liest das AD aus und gibt eine Liste mit allen Gruppen zurück
        /// </summary>
        /// <returns></returns>
        private Task GetAllADGroups()
        {
            return Task.Factory.StartNew(() =>
            {
                // create your domain context
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
                GroupPrincipal qbeGrp = new GroupPrincipal(ctx);

                // Erstellt den Eintrag der während des Ladens angezeigt wird
                GroupPrincipal loading = new GroupPrincipal(ctx)
                {
                    Name = "loading ..."
                };

                // Fügt die Lade Gruppe der Liste hinzu die angezeigt wird
                GroupsFiltered = new ObservableCollection<GroupPrincipal>
                {
                    loading
                };

                // create your principal searcher passing in the QBE principal    
                PrincipalSearcher srch = new PrincipalSearcher(qbeGrp);

                List<GroupPrincipal> lst = new List<GroupPrincipal>();
                // find all matches
                foreach (var found in srch.FindAll())
                {
                    if (found is GroupPrincipal grp)
                        AllGroups.Add(grp);
                }
                // Sortiert die Liste mit alles Gruppen
                AllGroups.Sort((x, y) => x.Name.CompareTo(y.Name));

                // Kopiert zum Start alle Gruppen in die gefilterte Liste
                GroupsFiltered = new ObservableCollection<GroupPrincipal>(AllGroups);
            });
        }

        #endregion

        #region History Log
        /// <summary>
        /// Holt sich alle Einträge aus der Datenbank und speichert sie in der Liste "HistoryLog"
        /// </summary>
        /// <param name="showCount">Wie viele Einträge sollen angezeigt werden</param>
        /// <returns></returns>
        private Task GetPlanFromMSSQL(int showCount)
        {
            return Task.Factory.StartNew(() =>
            {
                var mssql = new MsSql();
                mssql.Open();

                string sql = $"SELECT TOP(@showcount) * FROM schedule";
                SqlCommand cmd = new SqlCommand(sql, mssql.Con);

                cmd.Parameters.AddWithValue("@showcount", showCount);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Beim auslesen der Liste wird erst eine leere Liste erstellt
                    HistoryLog = new ObservableCollection<HistoryLogEntry>();

                    while (reader.Read())
                    {
                        var endDate = ((DateTime)reader["EndDate"]).ToShortDateString();

                        HistoryLog.Insert(0, new HistoryLogEntry
                        {
                            ID = (int)reader["ID"],
                            Username = reader["Username"].ToString(),
                            UserSid = reader["UserSid"].ToString(),
                            Groupname = reader["Groupname"].ToString(),
                            GroupSid = reader["GroupSid"].ToString(),
                            StartDate = ((DateTime)reader["StartDate"]).ToShortDateString(),
                            //EndDate = ((DateTime)reader["EndDate"]).ToShortDateString(),
                            EndDate = (endDate == DateTime.MaxValue.ToShortDateString()) ? "∞" : endDate,
                            Status = reader["Status"].ToString(),
                            Creator = reader["Creator"].ToString(),
                            Comment = reader["Comment"].ToString()
                        });
                    }
                }

                mssql.Close();
            });
        }

        /// <summary>
        /// Bei einem Klick auf einen Header wird die Liste sortiert. Abwechselnd ASC und DESC.
        /// </summary>
        /// <param name="sortBy">Das HistoryLogEntry Property nach dem sortiert werden soll</param>
        private void HistoryLogHeaderClick(string sortBy)
        {
            // Falls nach einer neuen Spalte sortiert wird, wird die Richtung nicht beachtet
            if (SortedHeader != sortBy)
                SortedDirection = 0;

            // Sortiert ASC
            if (SortedDirection != 1)
            {
                HistoryLog = new ObservableCollection<HistoryLogEntry>(HistoryLog.OrderBy(x => x.GetType().GetProperty(sortBy).GetValue(x, null) ));
                SortedDirection = 1;
            }
            // Sortiert DESC
            else
            {
                HistoryLog = new ObservableCollection<HistoryLogEntry>(HistoryLog.OrderByDescending(x => x.GetType().GetProperty(sortBy).GetValue(x, null) ));
                SortedDirection = -1;
            }

            // Speichert den Header nach dem Sortiert ist
            SortedHeader = sortBy;
        }

        /// <summary>
        /// Setzt den übergebenen HistoryLogEntry auf 'deleted' und schreibt das in die Datenbank
        /// </summary>
        /// <param name="HLE">Der übergebene HistoryLogEntry der aus der Tabelle kommt</param>
        private void RemoveHistoryLogEntry(HistoryLogEntry HLE)
        {
            // Erstellt eine Datenbankverbindung
            var mssql = new MsSql();
            // Öffnet die Datenbankverbindung
            mssql.Open();

            // Der Update SQL Befehl
            string sql = $"UPDATE schedule SET Status = 'deleted' WHERE ID = @id";

            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            // Hängt die ID als Parameter an
            cmd.Parameters.AddWithValue("@id", HLE.ID);

            // Führt die Query aus
            cmd.ExecuteNonQuery();

            // Schliest die Datenbankverbindung
            mssql.Close();

            // Lädt die HistoryLogs neu
            AsyncLoadedHistoryLog(100);
        }
        #endregion

        #region Error Handling

        private void NoConnectionToAD(Exception ex)
        {
            FormErrorMessage = "KEINE VERBINDUNG ZUM AD!";
            FormEnabled = false;
        }

        #endregion
    }
}
