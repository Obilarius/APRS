using ARPS.Models.Permissions;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ARPS
{
    public class PermissionItem : BindableBase
    {

        #region Commands
        /// <summary>
        /// Der Befehl um das Item aufzuklappen
        /// </summary>
        public ICommand ExpandCommand { get; set; }

        /// <summary>
        /// Command der beim Klick im COntextMenu auf Details ausgelöst wird
        /// </summary>
        public ICommand ContextShowDetailsCommand { get; set; }
        #endregion


        public int PathID { get; set; }
        public string UncPath { get; set; }
        public string OwnerSid { get; set; }
        public bool HasChildren { get; set; }
        public long Size { get; set; }
        public bool IsHidden { get; set; }
        public int ParentId { get; set; }

        /// <summary>
        /// Hält die Rechte für das PermissionItem. Hier werden alle Rechte der directoryAces zusammengerechnet
        /// </summary>
        public DirectoryACE folderAce
        {
            get
            {
                return new DirectoryACE("-1") { Rights = DirectoryACE.CalculateEffectiveRight_ThisFolder(directoryACEs) };
            }
        }

        /// <summary>
        /// Hält die einzelnen Rechte auf das PermissionItem
        /// </summary>
        public List<DirectoryACE> directoryACEs { get; set; }


        public DirectoryItemType ItemType { get; set; }

        /// <summary>
        /// Gibt die Größe lesbar zurück in KB, MB, GB...
        /// </summary>
        public string readableSize {
            get
            {
                return DirectoryStructure.GetBytesReadable(Size);
            }

        }

        /// <summary>
        /// Hält den Owner
        /// </summary>
        public ADElement Owner { get; set; }

        /// <summary>
        /// Gibt den Name des Servers zurück
        /// </summary>
        public string ServerName
        {
            get
            {
                if (UncPath == null)
                    return null;

                return this.UncPath.TrimStart('\\').Split('\\')[0];
            }
        }

        /// <summary>
        /// Gibt den Namen des Ordners zurück
        /// </summary>
        public string FolderName
        {
            get
            {
                if (UncPath == null)
                    return null;

                return this.UncPath.TrimEnd('\\').Split('\\').Last().TrimEnd('$');
            }
        }

        /// <summary>
        /// Hällt alle Gruppen zum selektierten User
        /// </summary>
        List<ADElement> AllGroups { get; set; }

        /// <summary>
        /// Hält die Unterordner
        /// </summary>
        public ObservableCollection<PermissionItem> Children { get; set; }

        #region Expand
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

        /// <summary>
        /// Klappt das Verzeichnis auf und findet alle Kinder
        /// </summary>
        private void Expand()
        {
            // Item hat keine Kinder und kann daher nicht aufgeklappt werden
            if (CanExpand == false)
                return;

            // Finde alle Kinder
            // erstellt eine MSSQL Verbindung und öffnet Sie
            var mssql = new MsSql();
            mssql.Open();

            // Überschreibt die Liste Kinder mit einer neuen leeren Liste
            Children = new ObservableCollection<PermissionItem>();

            foreach (ADElement group in AllGroups)
            {
                // Der SQL Befehl
                string sql = $"SELECT ace.*, d.* " +
                    $"FROM [ARPS_Test].[fs].[aces] ace " +
                    $"LEFT JOIN ARPS_Test.fs.acls acl " +
                    $"ON acl._ace_id = ace._ace_id " +
                    $"JOIN ARPS_Test.fs.dirs d " +
                    $"ON acl._path_id = d._path_id " +
                    $"WHERE acl._type = 0 " +
                    $"AND d._parent_path_id = @ParentId " +
                    $"AND ace._sid = @Sid";

                // Sendet den SQL Befehl an den SQL Server
                SqlCommand cmd = new SqlCommand(sql, mssql.Con);

                //Parameter anhängen
                cmd.Parameters.AddWithValue("@Sid", group.SID);
                cmd.Parameters.AddWithValue("@ParentId", PathID);

                // Benutzt den SQL Reader um über alle Zeilen der Abfrage zu gehen
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int _ace_id = (int)reader["_ace_id"];
                        string _sid = reader["_sid"].ToString();
                        FileSystemRights _rights = (FileSystemRights)reader["_rights"];
                        bool _type = Convert.ToBoolean(reader["_type"]);
                        string _fsr = reader["_fsr"].ToString();
                        bool _is_inherited = Convert.ToBoolean(reader["_is_inherited"]);
                        int _inheritance_flags = (int)reader["_inheritance_flags"];
                        int _propagation_flags = (int)reader["_propagation_flags"];

                        int _path_id = (int)reader["_path_id"];
                        string _unc_path_name = reader["_path_name"].ToString();
                        string _owner_sid = reader["_owner_sid"].ToString();
                        bool _has_children = Convert.ToBoolean(reader["_has_children"]);
                        long _size = (long)reader["_size"];
                        bool _hidden = IsHidden;

                        // Legt für die ausgelesene Zeile ein neues PermissionItem an in dem alle Infos über diesen Ordner gespeichert sind
                        PermissionItem newPI = new PermissionItem(_path_id, _unc_path_name, _owner_sid, _has_children, _size, _hidden, AllGroups, DirectoryItemType.Folder);

                        // Legt für die ausgelesene Zeile ein neues ACE an in dem alle Infos über das Rechte Objekt angeben sind
                        DirectoryACE newACE = new DirectoryACE(_sid, _rights, _type, _fsr, _is_inherited, _inheritance_flags, _propagation_flags);


                        // Falls keine Rechte in diesem Datensatz vergeben werden oder wenn die Rechte nur auf Unterordner gelten
                        // wird der Datensatz nicht hinzugefügt
                        if (newACE.Rights <= 0)
                            continue;

                        // Prüft ob das PermissionItem schon in der Liste vorhanden ist.
                        PermissionItem value = Children.FirstOrDefault(item => item.PathID == newPI.PathID);

                        // Falls der Pfad schon vorhanden ist wird das neue ACE Object dem bestehenden PermissionItem hinzugefügt
                        if (value != null)
                        {
                            // Das neue ACE Objekt wird dem Permission Item (Ordner) hinzugefügt
                            value.directoryACEs.Add(newACE);
                        }
                        // Falls das PermissionItem noch nicht vorhanden ist, wird das PerItem hinzugefügt und das neue Ace wird ihm hinzugefügt
                        else
                        {
                            // Fügt das neue ACE dem neuen PerItem hinzu
                            newPI.directoryACEs.Add(newACE);

                            // Fügt das neue PerItem der Collection hinzu
                            Children.Add(newPI);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Sagt uns ob das aktuelle Element Kinder hat und dammit aufgeklappt werden kann
        /// </summary>
        public bool CanExpand { get { return HasChildren; } }
        #endregion

        /// <summary>
        /// Löscht alle Kinder in der Liste. Fügt ein Dummy Item hinzu damit das Symbol zum aufklappen angezeit wird, falls es nötig ist.
        /// </summary>
        private void ClearChildren()
        {
            // Wenn Node ein Server ist werden die Kinder nicht gelöscht
            if (UncPath == ServerName)
                return;

            // Prüft ob das aktuelle Item aktuell Kinder hat
            bool hasChilds = (Children.Count() > 0) ? true : false;

            // Löscht alle Items aus der Liste
            Children = new ObservableCollection<PermissionItem>();

            // Fügt ein Dummy Item hinzu, falls das Item vorher Kinder hatte
            if (hasChilds)
                this.Children.Add(null);
        }

        /// <summary>
        /// Funktion die Ausgeführt wird wenn jemand auf Details im Kontextmenü klickt
        /// </summary>
        public void ContextShowDetails()
        {
            ShowDetails view = new ShowDetails { DataContext = this };
            view.Show();

            // Füllt den Owner
            Owner = ADStructure.GetADElement(OwnerSid);
        }


        #region Effektive Rechte auf den Ordner, Unterordner und Dateien

        /// <summary>
        /// Gibt einen String zurück der das Recht auf diesen Ordner wiedergibt
        /// </summary>
        public string EffectiveRight_ThisFolder
        {
            get
            {
                FileSystemRights right = DirectoryACE.CalculateEffectiveRight_ThisFolder(directoryACEs);
                return getRightText(right);
            }
            
        }

        /// <summary>
        /// Gibt einen String zurück der das Recht auf die Unterordner wiedergibt
        /// </summary>
        public string EffectiveRight_Subfolder
        {
            get
            {
                var right = DirectoryACE.CalculateEffectiveRight_Subfolder(directoryACEs);
                return getRightText(right);
            }
        }

        /// <summary>
        /// Gibt einen String zurück der das Recht auf die Dateien wiedergibt
        /// </summary>
        public string EffectiveRight_Files
        {
            get
            {
                var right = DirectoryACE.CalculateEffectiveRight_Files(directoryACEs);
                return getRightText(right);
            }
        }

        /// <summary>
        /// Übersetzt das FileSystemRight in einen Text
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        private string getRightText(FileSystemRights right)
        {
            if (right.HasFlag(FileSystemRights.FullControl))
                return "Vollzugriff";
            else if (right.HasFlag(FileSystemRights.Modify))
                return "Ändern";
            else if (right.HasFlag(FileSystemRights.ReadAndExecute) && right.HasFlag(FileSystemRights.Write))
                return "Lesen, Ausführen & Schreiben";
            else if (right.HasFlag(FileSystemRights.ReadAndExecute))
                return "Lesen, Ausführen";
            else if (right.HasFlag(FileSystemRights.Read) && right.HasFlag(FileSystemRights.Write))
                return "Lesen & Schreiben";
            else if (right.HasFlag(FileSystemRights.Read))
                return "Lesen";
            else if (right.HasFlag(FileSystemRights.Write))
                return "Schreiben";
            else if (right.HasFlag(FileSystemRights.Synchronize) || right == (FileSystemRights)0)
                return "Keine Rechte";
            else
                return "Spezielle Berechtigungen";
        }
        #endregion


        #region Kontstruktoren
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="_path_id"></param>
        /// <param name="_unc_path_name"></param>
        /// <param name="_owner_sid"></param>
        /// <param name="_has_children"></param>
        /// <param name="_size"></param>
        /// <param name="_path_name"></param>
        /// <param name="_hidden"></param>
        public PermissionItem(int _path_id, string _unc_path_name, string _owner_sid, bool _has_children, long _size, 
            bool _hidden, List<ADElement> allGroups, DirectoryItemType itemType)
        {
            PathID = _path_id;
            UncPath = _unc_path_name;
            OwnerSid = _owner_sid;
            HasChildren = _has_children;
            Size = _size;
            IsHidden = _hidden;
            AllGroups = allGroups;
            ItemType = itemType;

            // Erstelle Commands
            ExpandCommand = new RelayCommand(Expand);
            ContextShowDetailsCommand = new RelayCommand(ContextShowDetails);


            Children = new ObservableCollection<PermissionItem>();
            if (HasChildren)
                Children.Add(null);

            directoryACEs = new List<DirectoryACE>();
        }

        /// <summary>
        /// Konstruktor für Server
        /// </summary>
        public PermissionItem(string serverName)
        {
            // Erstelle Commands
            ExpandCommand = new RelayCommand(Expand);
            ContextShowDetailsCommand = new RelayCommand(ContextShowDetails);

            // Setzt den UNCPfad auf den Servernamen
            UncPath = serverName;

            ItemType = DirectoryItemType.Server;

            Children = new ObservableCollection<PermissionItem>();
        }
        #endregion
    }
}
