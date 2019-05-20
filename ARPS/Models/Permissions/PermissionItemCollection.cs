using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ARPS
{
    public class PermissionItemCollection : BindableBase
    {
        /// <summary>
        /// Hällt die Items die angezeigt werden
        /// </summary>
        public ObservableCollection<PermissionItem> Items { get; set; }

        /// <summary>
        /// Hällt alle Gruppen zum selektierten User
        /// </summary>
        List<ADElement> AllGroups { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="userSid"></param>
        public PermissionItemCollection(string userSid)
        {
            // Liest alle Gruppen des Users aus
            AllGroups = new List<ADElement>(ADStructure.GetGroupsFromUser(userSid));
        }

        /// <summary>
        /// Füllt die Shares in das Property Items
        /// </summary>
        public void FillItemsWithShares()
        {
            Items = new ObservableCollection<PermissionItem>();

            // erstellt eine MSSQL Verbindung und öffnet Sie
            var mssql = new MsSql();
            mssql.Open();

            foreach (ADElement group in AllGroups)
            {
                // Der SQL Befehl um alle Ordner abzurufen die root sind
                string sql = $"SELECT ace.*, s.* " +
                    $"FROM [ARPS_Test].[fs].[aces] ace " +
                    $"LEFT JOIN ARPS_Test.fs.acls acl " +
                    $"ON acl._ace_id = ace._ace_id " +
                    $"JOIN ARPS_Test.fs.shares s " +
                    $"ON acl._path_id = s._path_id " +
                    $"WHERE acl._type = 1 " +
                    $"AND ace._sid = @Sid";

                // Sendet den SQL Befehl an den SQL Server
                SqlCommand cmd = new SqlCommand(sql, mssql.Con);

                //Parameter anhängen
                cmd.Parameters.AddWithValue("@Sid", group.SID);

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
                        string _unc_path_name = reader["_unc_path_name"].ToString();
                        string _owner_sid = reader["_owner_sid"].ToString();
                        bool _has_children = Convert.ToBoolean(reader["_has_children"]);
                        long _size = (long)reader["_size"];
                        string _path_name = reader["_path_name"].ToString();
                        string _display_name = reader["_display_name"].ToString();
                        string _remark = reader["_remark"].ToString();
                        string _share_type = reader["_share_type"].ToString();
                        bool _hidden = Convert.ToBoolean(reader["_hidden"]);

                        PermissionItem newPI = new PermissionItem(_sid, _rights, _type, _fsr, _is_inherited, _inheritance_flags, _propagation_flags);
                        newPI.PathID = _path_id;
                        newPI.UncPath = _unc_path_name;
                        newPI.OwnerSid = _owner_sid;
                        newPI.HasChildren = _has_children;
                        newPI.Size = _size;
                        newPI.LocalPath = _path_name;
                        //newPI.DisplayName = _display_name;
                        //newPI.Remark = _remark;
                        //newPI.ShareType = _share_type;
                        newPI.IsHidden = _hidden;

                        // Falls keine Rechte in diesem Datensatz vergeben werden oder wenn die Rechte nur auf Unterordner gelten
                        // wird der Datensatz nicht hinzugefügt
                        if (newPI.Rights <= 0 || !newPI.PropagationOnThisFolder)
                            continue;

                        Items.Add(newPI);
                    }
                }
            }

            // Schließt die MSSQL verbindung
            mssql.Close();
        }

        public void GroupItems()
        {
            List<PermissionItem> tempItems = new List<PermissionItem>();

            foreach (var item in Items)
            {
                // Prüft ob das Recht auf diesen Ordner gewährt wird
                if (item.InheritanceFlags == 0 && item.PropagationFlags == 0 ||
                    item.InheritanceFlags == 3 && item.PropagationFlags == 0 ||
                    item.InheritanceFlags == 1 && item.PropagationFlags == 0 ||
                    item.InheritanceFlags == 2 && item.PropagationFlags == 0)
                {
                    // TODO: Vergleich ob Item schon vorhanden ist und in der Liste tempItems zusammenführen
                }
            }
        }
    }
}
