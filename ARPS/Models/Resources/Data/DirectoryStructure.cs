
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace ARPS
{
    public static class DirectoryStructure
    {
        #region Get Lists

        /// <summary>
        /// Gibt eine Liste mit allen angegebenen Servers zurück.
        /// </summary>
        /// <param name="serverNames"></param>
        /// <returns></returns>
        public static List<DirectoryItem> GetServers()
        {
            // erstellt eine MSSQL Verbindung und öffnet Sie
            var mssql = new MsSql();
            mssql.Open();

            // Der SQL Befehl um alle Ordner abzurufen die root sind
            string sql = $"SELECT _path_name FROM {mssql.TBL_FS_Dirs} WHERE _is_root = 1";

            // Sendet den SQL Befehl an den SQL Server
            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            List<string> ListOfServers = new List<string>();

            // Benutzt den SQL Reader um über alle Zeilen der Abfrage zu gehen
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Liest der Wert der SQL abfrage und gibt den Servernamen zurück
                    string serverName = GetServerName(reader.GetString(0));

                    if (!ListOfServers.Contains(serverName))
                        ListOfServers.Add(serverName);

                    

                }
            }

            // Schließt die MSSQL verbindung
            mssql.Close();

            List<DirectoryItem> retList = new List<DirectoryItem>();

            foreach (var serverName in ListOfServers)
            {
                //Erstellt ein neues DirectoryItem und fügt es der Liste hinzu
                retList.Add(new DirectoryItem
                {
                    Id = -1,
                    FullPath = $"\\\\{serverName}",
                    Owner = "Kein Besitzer gefunden",
                    Type = DirectoryItemType.Server,
                    HasChildren = true
                });
            }

            // Gibt eine Liste mit DirectoryItems zurück die die Server beinhaltet
            return retList;
        }


        /// <summary>
        /// Gibt eine Liste mit allen Unterordnern des angegebenen Servers zurück die keine ParentId besitzen.
        /// </summary>
        /// <param name="serverName">Der Servername der abgefragt wird (zb "apollon")</param>
        /// <returns></returns>
        public static List<DirectoryItem> GetServerAndSharedFolders(string serverName)
        {
            // erstellt eine MSSQL Verbindung und öffnet Sie
            var mssql = new MsSql();
            mssql.Open();

            // Der SQL Befehl um alle Ordner abzurufen die mit dem "ServerName" beginnen aber keine ParentId besitzen
            string sql = $"SELECT ID, Directory, Owner " +
                $"FROM dirs " +
                $"WHERE ParentID IS NULL AND Directory LIKE @serverName " +
                $"ORDER BY Directory ";

            // Sendet den SQL Befehl an den SQL Server
            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            // Erstellt einen neuen Parameter mit dem Servernamen
            var serverNameParam = new SqlParameter("serverName", System.Data.SqlDbType.NVarChar)
            {
                Value = $"\\\\{serverName}\\%"
            };

            // Bindet den Parameter an die Abfrage
            cmd.Parameters.Add(serverNameParam);

            // Erstellt eine leere Liste die später zurück gegeben werden kann
            List<DirectoryItem> retList = new List<DirectoryItem>
            {
                // Erstellt das Item für den Server
                new DirectoryItem
                {
                    Id = -1,
                    FullPath = $"\\\\{serverName}",
                    Owner = "no Owner",
                    Type = DirectoryItemType.Server
                }
            };


            // Benutzt den SQL Reader um über alle Zeilen der Abfrage zu gehen
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Liest aus der jeweiligen Zeile die einzelnen Werte aus
                    int id = reader.GetInt32(0);
                    string dir = reader.GetString(1);
                    string owner = reader.GetString(2);

                    // Erstellt ein neues DirectoryItem und fügt es der Liste hinzu
                    retList.Add(new DirectoryItem
                    {
                        Id = id,
                        FullPath = dir,
                        Owner = owner,
                        Type = DirectoryItemType.SharedFolder
                    });

                }
            }

            // Schließt die MSSQL verbindung
            mssql.Close();

            // Gibt die Liste zurück
            return retList;
        }

        /// <summary>
        /// Gibt eine Liste mit allen Unterordnern die die übergebene ID als ParentID besitzen
        /// </summary>
        /// <param name="Id">Die ParentID</param>
        /// <returns></returns>
        public static List<DirectoryItem> GetChildren(int Id)
        {
            // erstellt eine MSSQL Verbindung und öffnet Sie
            var mssql = new MsSql();
            mssql.Open();

            // Der SQL Befehl um alle Ordner abzurufen die die übergebene ParentID besitzen
            string sql = $"SELECT * " +
                $"FROM {mssql.TBL_FS_Dirs} " +
                $"WHERE _parent_path_id = @parentId " +
                $"ORDER BY _path_name";

            // Sendet den SQL Befehl an den SQL Server
            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            // Erstellt die ParentId als Parameter
            cmd.Parameters.AddWithValue("parentId", Id);

            // Erstellt eine leere Liste die später zurück gegeben werden kann
            List<DirectoryItem> retList = new List<DirectoryItem>();

            // Benutzt den SQL Reader um über alle Zeilen der Abfrage zu gehen
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Erstellt ein neues DirectoryItem und fügt es der Liste hinzu
                    retList.Add(new DirectoryItem
                    {
                        Id = (int)reader["_path_id"],
                        FullPath = reader["_path_name"].ToString(),
                        Owner = reader["_owner_sid"].ToString(),
                        Type = DirectoryItemType.Folder,
                        ParentID = (int)reader["_parent_path_id"],
                        IsRoot = (bool)reader["_is_root"],
                        HasChildren = (bool)reader["_has_children"],
                        ScanDeepth = (int)reader["_scan_deepth"],
                        Size = (long)reader["_size"]
                    });

                }
            }

            // Schließt die MSSQL verbindung
            mssql.Close();

            // Gibt die Liste zurück
            return retList;
        }

        /// <summary>
        /// Gibt eine Liste mit allen Unterordnern des übergebenen Pfades zurück
        /// </summary>
        /// <param name="fullPath">Der volle Pfad</param>
        /// <returns></returns>
        public static List<DirectoryItem> GetChildren(string fullPath)
        {
            // erstellt eine MSSQL Verbindung und öffnet Sie
            var mssql = new MsSql();
            mssql.Open();

            // Der SQL Befehl um alle Ordner abzurufen die mit dem Pfad beginnen
            string sql = $"SELECT * " +
                $"FROM {mssql.TBL_FS_Dirs} " +
                $"WHERE _is_root = 1 AND _path_name LIKE @fullPath " +
                $"ORDER BY _path_name";

            // Sendet den SQL Befehl an den SQL Server
            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            // Erstellt den vollen Pfad als Parameter
            cmd.Parameters.AddWithValue("fullPath", fullPath + "\\%");

            // Erstellt eine leere Liste die später zurück gegeben werden kann
            List<DirectoryItem> retList = new List<DirectoryItem>();

            // Benutzt den SQL Reader um über alle Zeilen der Abfrage zu gehen
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Erstellt ein neues DirectoryItem und fügt es der Liste hinzu
                    retList.Add(new DirectoryItem
                    {
                        Id = (int)reader["_path_id"],
                        FullPath = reader["_path_name"].ToString(),
                        Owner = reader["_owner_sid"].ToString(),
                        Type = DirectoryItemType.SharedFolder,
                        ParentID = (int)reader["_parent_path_id"],
                        IsRoot = (bool)reader["_is_root"],
                        HasChildren = (bool)reader["_has_children"],
                        ScanDeepth = (int)reader["_scan_deepth"],
                        Size = (long)reader["_size"]
                    });

                }
            }

            return retList;
        }

        #endregion


        #region Helper

        /// <summary>
        /// Gibt nur den Ordnernamen eines vollen Pfades zurück
        /// </summary>
        /// <param name="path">Der volle Pfad</param>
        /// <returns></returns>
        public static string GetFolderName(string path)
        {
            // Wenn wir keinen Pfad bekommen gib empty zurück
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            // Findet das letzte Backslash im Pfad
            var lastIndex = path.LastIndexOf('\\');

            // Wenn wir kein Backslash finden, gib den ganzen Pfad zurück
            if (lastIndex <= 0)
                return path;

            // Return der Namen nach dem letzten Backslash
            return path.Substring(lastIndex + 1);
        }

        /// <summary>
        /// Gibt nur den Servernamen eines vollen Pfades zurück
        /// </summary>
        /// <param name="path">Der volle Pfad</param>
        /// <returns></returns>
        public static string GetServerName(string path)
        {
            // Wenn wir keinen Pfad bekommen gib empty zurück
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            // Schneider die führenden \ ab
            path = path.TrimStart('\\');

            // Findet das erste Backslash im Pfad
            var index = path.IndexOf('\\');

            // Wenn wir kein Backslash finden, gib den ganzen Pfad zurück
            if (index <= 0)
                return path;

            // Return der Namen nach dem letzten Backslash
            return path.Substring(0, 1).ToUpper() + path.Substring(1, index -1);
        }

        /// <summary>
        /// Returns the human-readable file size for an arbitrary, 64-bit file size 
        /// The default format is "0.### XB", e.g. "4.2 KB" or "1.434 GB"
        /// </summary>
        /// <param name="i">Wert in Bytes der Umgerechnet werden soll</param>
        /// <returns></returns>
        public static string GetBytesReadable(long i)
        {
            // Get absolute value
            long absolute_i = (i < 0 ? -i : i);
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (i >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (i >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (i >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = (readable / 1024);
            // Return formatted number with suffix
            return readable.ToString("0.## ") + suffix;
        }

        /// <summary>
        /// Gibt den Usernamen und in Klammern dahinter den Pricipal zu eine übergebenen SID zurück
        /// </summary>
        /// <param name="sid">Die SID des Benutzers</param>
        /// <returns></returns>
        public static string GetUserNameAndPricipalName(string sid)
        {
            // erstellt eine MSSQL Verbindung und öffnet Sie
            var mssql = new MsSql();
            mssql.Open();

            // Der SQL Befehl um alle Ordner abzurufen die root sind
            string sql = $"SELECT * FROM (" +
                $"SELECT SID, DisplayName name, UserPrincipalName secName FROM {mssql.TBL_AD_Users} " +
                $"UNION ALL " +
                $"SELECT SID, Name name, SamAccountName secName " +
                $"FROM {mssql.TBL_AD_Groups}) as UsersAndGroups " +
                $"WHERE SID = @Sid";

            // Sendet den SQL Befehl an den SQL Server
            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            //Parameter anhängen
            cmd.Parameters.AddWithValue("Sid", sid);

            

            // Benutzt den SQL Reader um über alle Zeilen der Abfrage zu gehen
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string name = reader["name"].ToString();
                    string secName = reader["secName"].ToString();

                    // Schließt die MSSQL verbindung
                    mssql.Close();

                    return $"{name} ({secName})";
                }
            }

            // Schließt die MSSQL verbindung
            mssql.Close();

            return "Keinen Besitzer gefunden";
        }

        /// <summary>
        /// Gibt alle direkten ACEs des Ordners zurück
        /// </summary>
        /// <param name="_path_id">Die PfadId des Ordners</param>
        /// <returns></returns>
        public static List<DirectoryACE> GetACEs(int _path_id)
        {
            // erstellt eine MSSQL Verbindung und öffnet Sie
            var mssql = new MsSql();
            mssql.Open();

            // Der SQL Befehl um alle Ordner abzurufen die root sind
            string sql = $"SELECT " +
                    $"ug._is_group, ug._identity_name, ug._distinguished_name " +
                    $", aces._ace_id, aces._sid, aces._rights, aces._type, aces._fsr, aces._is_inherited, aces._inheritance_flags, aces._propagation_flags " +
                $"FROM {mssql.TBL_FS_Dirs} dirs " +
                $"JOIN {mssql.TBL_FS_ACLs} acls " +
                $"ON dirs._path_id = acls._path_id " +
                $"JOIN {mssql.TBL_FS_ACEs} aces " +
                $"ON aces._ace_id = acls._ace_id " +
                $"JOIN( " +
                    $"SELECT 0 as _is_group, SID _sid, DisplayName _identity_name, DistinguishedName _distinguished_name " +
                    $"FROM {mssql.TBL_AD_Users} " +
                    $"UNION ALL " +
                    $"SELECT 1, SID, Name, DistinguishedName " +
                    $"FROM {mssql.TBL_AD_Groups}) ug " +
                $"ON aces._sid = ug._sid " +
                $"WHERE dirs._path_id = @PathId";

            // Sendet den SQL Befehl an den SQL Server
            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            //Parameter anhängen
            cmd.Parameters.AddWithValue("@PathId", _path_id);

            //Erstellt Liste die zurückgegeben wird
            List<DirectoryACE> aceList = new List<DirectoryACE>();

            // Benutzt den SQL Reader um über alle Zeilen der Abfrage zu gehen
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    bool _is_group = Convert.ToBoolean(reader["_is_group"]);
                    string _identity_name = reader["_identity_name"].ToString();
                    string _distinguished_name = reader["_distinguished_name"].ToString();
                    int _ace_id = (int)reader["_ace_id"];
                    string _sid = reader["_sid"].ToString();
                    int _rights = (int)reader["_rights"];
                    bool _type = Convert.ToBoolean(reader["_type"]);
                    string _fsr = reader["_fsr"].ToString();
                    bool _is_inherited = Convert.ToBoolean(reader["_is_inherited"]);
                    int _inheritance_flags = (int)reader["_inheritance_flags"];
                    int _propagation_flags = (int)reader["_propagation_flags"];

                    aceList.Add(
                        new DirectoryACE(_is_group, _identity_name, _distinguished_name, _ace_id, _sid, _rights, _type, _fsr, _is_inherited, _inheritance_flags, _propagation_flags));

                }
            }

            // Schließt die MSSQL verbindung
            mssql.Close();

            return aceList;
        }

        /// <summary>
        /// Get die direkten ACEs durch und gibt alle User zurück die dadurch berechtigt sind. Gruppen werden zu Usern aufgelöst.
        /// </summary>
        /// <param name="aces">Die Liste der direkten ACEs</param>
        /// <returns></returns>
        public static List<DirectoryACE> GetAllAuthorizedUser(List<DirectoryACE> aces)
        {
            List<DirectoryACE> retACEs = new List<DirectoryACE>();

            foreach (DirectoryACE ace in aces)
            {
                // Der ACE gilt nur für Unterordner und zählt hier also nicht als Recht auf diesen Ordner
                if (ace.PropagationFlags == 2)
                    continue;

                // Wenn User
                if (ace.IsGroup == false)
                {
                    Debug.WriteLine(ace.IdentityName);
                    retACEs.Add(ace);
                }
                // Wenn Gruppe dann wird die Funktion GetMemberInGroup aufgerufen die alle User die über diese Gruppe berechtigt sind abgerufen
                else
                {
                    retACEs.AddRange(GetMemberInGroup(ace));
                }
            }

            return retACEs;
        }

        /// <summary>
        /// Hilfsfunktion die alle Member einer Gruppe ausliest. Falls Gruppen in Gruppen sind wir diese Funktion auch rekursiv aufgerufen
        /// </summary>
        /// <param name="groupAce">Das ACE der Gruppe die ausgelesen werden soll</param>
        /// <returns></returns>
        private static List<DirectoryACE> GetMemberInGroup(DirectoryACE groupAce)
        {

            List<DirectoryACE> retList = new List<DirectoryACE>();

            // erstellt eine MSSQL Verbindung und öffnet Sie
            var mssql = new MsSql();
            mssql.Open();

            // Der SQL Befehl um alle Ordner abzurufen die root sind
            string sql = $"SELECT gu.userSID, CASE WHEN u.SID IS NULL THEN 1 ELSE 0 END as _is_group, u.* " +
                $"FROM ARPS_Test.dbo.adgroups g " +
                $"JOIN ARPS_Test.dbo.grp_user gu " +
                $"ON g.SID = gu.grpSID " +
                $"LEFT JOIN ARPS_Test.dbo.adusers u " +
                $"ON u.SID = gu.userSID " +
                $"WHERE g.SID = @GroupSid ";

            // Sendet den SQL Befehl an den SQL Server
            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            //Parameter anhängen
            cmd.Parameters.AddWithValue("@GroupSid", groupAce.SID);

            // Benutzt den SQL Reader um über alle Zeilen der Abfrage zu gehen
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    bool isGroup = Convert.ToBoolean(reader["_is_group"]);
                    string sid = reader["userSID"].ToString();

                    // Wenn die aktuelle Zeile eine Gruppe ist wird die Funktion Rekursiv aufgerufen
                    if (isGroup)
                    {
                        // Fügt die Rückgabewert der rekursiven Funktion an die Liste an
                        retList.AddRange(GetMemberInGroup(new DirectoryACE(
                            sid, 
                            groupAce.Rights, 
                            groupAce.Type, 
                            groupAce.FileSystemRight, 
                            groupAce.IsInherited, 
                            groupAce.InheritanceFlags, 
                            groupAce.PropagationFlags)));
                    }
                    else
                    // Falls die Zeile ein User ist wir aus dem User ein PseudoACE erstellt und an die Liste angehängt
                    {
                        // Speichert die Daten des Readers in einzelne Variablen
                        string displayName = reader["DisplayName"].ToString();
                        string samAccountName = reader["SamAccountName"].ToString();
                        string distinguishedName = reader["DistinguishedName"].ToString();
                        string userPrincipalName = reader["UserPrincipalName"].ToString();
                        bool enabled = Convert.ToBoolean(reader["Enabled"]);

                        // erstellt den User falls er aktiv ist
                        if (enabled)
                        {
                            retList.Add(new DirectoryACE(
                                false,
                                displayName + " (" + userPrincipalName + ")",
                                userPrincipalName,
                                -1,
                                sid,
                                groupAce.Rights,
                                groupAce.Type,
                                groupAce.FileSystemRight,
                                groupAce.IsInherited,
                                groupAce.InheritanceFlags,
                                groupAce.PropagationFlags));
                        }

                    }
                }
            }

            // Schließt die MSSQL verbindung
            mssql.Close();

            return retList;
        }

        #endregion
    }
}
