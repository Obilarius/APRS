using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPS
{
    public class Groups
    {
        #region Properties
        /// <summary>
        /// Hält die MsSql Vebindung damit alle Methoden darauf zugreifen können
        /// </summary>
        private MsSql mssql;

        /// <summary>
        /// Die Liste enthält alle Zeilen der Kategorie
        /// </summary>
        public List<CountEnrty> Entrys { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Standard Konstruktor
        /// </summary>
        public Groups()
        {
            // erstellt und öffnet eine neue MsSql Verbindung
            mssql = new MsSql();
            mssql.Open();

            // Erzeugt eine neue Liste mit Einträgen
            Entrys = new List<CountEnrty>();

            #region Alle Gruppen
            // Erstelle Eintrag für Gruppen
            Entrys.Add(new CountEnrty { Name = "Alle Gruppen", Count = GetCountGroups().ToString() });
            #endregion

            #region Gruppen mit Mitgliedern (ohne Rekursionsgruppen)
            // Erstelle Eintrag für Gruppen mit Mitgliedern (ohne Rekursionsgruppen)
            Entrys.Add(new CountEnrty { Name = "Gruppen mit Mitgliedern (ohne Rekursionsgruppen)", Count = GetCountGroupsWithMembers().ToString() });
            #endregion

            #region Leere Gruppen
            // Erstelle Eintrag für Gruppen die keine Mitglieder haben
            Entrys.Add(new CountEnrty { Name = "Leere Gruppen", Count = GetCountEmptyGroups().ToString() });
            #endregion

            #region Gruppen in Rekursionen
            // Erstelle Eintrag für Gruppen die in Rekursion sind
            Entrys.Add(new CountEnrty { Name = "Gruppen in Rekursionen", Count = GetCountGroupsInRecursion().ToString() });
            #endregion

            #region Die mitgliedstärksten Gruppen
            // Erstelle die Einträge für die drei mitgliedstärksten Gruppen
            List<string> biggestGroups = GetBiggestGroups(); 
            Entrys.Add(new CountEnrty { Name = "Die mitgliedstärksten Gruppen", Count = biggestGroups.First() });
            Entrys.Add(new CountEnrty { Name = "", Count = biggestGroups[1] });
            Entrys.Add(new CountEnrty { Name = "", Count = biggestGroups[2] });
            #endregion

            #region Globale Sicherheitsgruppen
            int globSecCount = GetCount($"SELECT count(*) " +
                $"FROM adgroups g " +
                $"WHERE g.IsSecurityGroup = 1 AND g.GroupScope = 'Global'");
            Entrys.Add(new CountEnrty { Name = "Globale Sicherheitsgruppen", Count = globSecCount.ToString() });
            #endregion

            #region Universelle Sicherheitsgruppen
            int univSecCount = GetCount($"SELECT count(*) " +
                $"FROM adgroups g " +
                $"WHERE g.IsSecurityGroup = 1 AND g.GroupScope = 'Universal'");
            Entrys.Add(new CountEnrty { Name = "Universelle Sicherheitsgruppen", Count = univSecCount.ToString() });
            #endregion

            #region Lokale Sicherheitsgruppen
            int localSecCount = GetCount($"SELECT count(*) " +
                $"FROM adgroups g " +
                $"WHERE g.IsSecurityGroup = 1 AND g.GroupScope = 'Local'");
            Entrys.Add(new CountEnrty { Name = "Lokale Sicherheitsgruppen", Count = localSecCount.ToString() });
            #endregion

            #region Globale Verteilergruppen
            int globVerCount = GetCount($"SELECT count(*) " +
                $"FROM adgroups g " +
                $"WHERE g.IsSecurityGroup = 0 AND g.GroupScope = 'Global'");
            Entrys.Add(new CountEnrty { Name = "Globale Verteilergruppen", Count = globVerCount.ToString() });
            #endregion

            #region Universelle Verteilergruppen
            int univVerCount = GetCount($"SELECT count(*) " +
                $"FROM adgroups g " +
                $"WHERE g.IsSecurityGroup = 0 AND g.GroupScope = 'Universal'");
            Entrys.Add(new CountEnrty { Name = "Universelle Verteilergruppen", Count = univVerCount.ToString() });
            #endregion

            #region Lokale Verteilergruppen
            int localVerCount = GetCount($"SELECT count(*) " +
                $"FROM adgroups g " +
                $"WHERE g.IsSecurityGroup = 0 AND g.GroupScope = 'Local'");
            Entrys.Add(new CountEnrty { Name = "Lokale Verteilergruppen", Count = localVerCount.ToString() });
            #endregion

            mssql.Close();
        }

        #endregion

        #region Get methods

        /// <summary>
        /// Gibt die Anzahl der Gruppen im AD zurück
        /// </summary>
        /// <returns></returns>
        private int GetCountGroups()
        {
            // Übergibt den SQL Befehl an die HelperFunction
            return GetCount($"SELECT count(*) FROM adgroups WHERE DistinguishedName IS NOT NULL");
        }


        /// <summary>
        /// Gibt die Anzahl der Gruppen im AD zurück die mindestens einen Benutzer beinhalten
        /// </summary>
        /// <returns></returns>
        private int GetCountGroupsWithMembers()
        {
            // Übergibt den SQL Befehl an die HelperFunction
            return GetCount($"SELECT Count(*) " +
                $"FROM( " +
                    $"SELECT Count(*) OVER() AS Total " +
                    $"FROM grp_user gu " +
                    $"LEFT JOIN adusers u ON gu.userSID = u.SID " +
                    $"WHERE u.DisplayName IS NOT NULL " +
                    $"GROUP BY gu.grpSID) s " +
                $"GROUP BY s.Total");
        }


        /// <summary>
        /// Gibt die Anzahl der leeren Gruppen im AD zurück
        /// </summary>
        /// <returns></returns>
        private int GetCountEmptyGroups()
        {
            // Übergibt den SQL Befehl an die HelperFunction
            return GetCount($"SELECT Count(*) " +
                $"FROM( " +
                    $"SELECT gu.grpSID " +
                    $"FROM grp_user gu " +
                    $"LEFT JOIN adusers u ON gu.userSID = u.SID " +
                    $"LEFT JOIN adgroups g ON gu.userSID = g.SID " +
                    $"WHERE COALESCE(u.SID, g.SID) IS NULL " +
                    $"GROUP BY gu.grpSID) s");

        }

        /// <summary>
        /// Gibt die Anzahl der Gruppen in Rekursion zurück
        /// </summary>
        /// <returns></returns>
        private int GetCountGroupsInRecursion()
        {
            // Übergibt den SQL Befehl an die HelperFunction
            return GetCount($"SELECT Count(*) " +
                $"FROM grp_user gu " +
                $"LEFT JOIN adusers u ON gu.userSID = u.SID " +
                $"LEFT JOIN adgroups g ON gu.userSID = g.SID " +
                $"WHERE g.SID IS NOT NULL");

        }

        /// <summary>
        /// Gibt die drei Gruppen aus die die meisten Mitglieder haben
        /// </summary>
        /// <returns></returns>
        private List<string> GetBiggestGroups()
        {
            var sql = $"SELECT TOP(3) s.count, g.Name, g.SamAccountName " +
                $"FROM( " +
                    $"SELECT gu.grpSID, Count(*) as 'count' " +
                    $"FROM grp_user gu " +
                    $"GROUP BY gu.grpSID ) s " +
                $"LEFT JOIN adgroups g ON g.SID = s.grpSID " +
                $"ORDER BY s.count DESC";

            // Sendet den SQL Befehl an den SQL Server
            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            // Erstelle eine leere Liste die zurückgegeben werden soll
            var retList = new List<string>();

            // Benutzt den SQL Reader um über alle Zeilen der Abfrage zu gehen
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Liest den Count aus
                    int count = reader.GetInt32(0);
                    // Liest den Gruppennamen aus
                    string name = reader.GetString(1);
                    // Liest den SamAccountName aus
                    string sam = reader.GetString(2);

                    // Fügt die Gruppe mit Count zur Liste hinzu
                    retList.Add($"{count} - {name} ({sam})");
                }
            }

            return retList;
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Schreibt alle UserSID in die Übergebene Liste. Geht auch rekursiv in alle Gruppen in Gruppen bis alle User aufgelöst sind
        /// </summary>
        /// <param name="grpSID">Die Gruppen SID von der man die User haben möchte</param>
        /// <param name="list">Eine leere Liste die per ref übergeben werden muss</param>
        private void GetUserInGroup(string grpSID, ref List<string> list)
        {
            // Der SQL Befehl 
            string sql = $"SELECT u.SID as 'User', g.SID as 'Group' " +
                $"FROM grp_user gu " +
                $"LEFT JOIN adusers u ON gu.userSID = u.SID " +
                $"LEFT JOIN adgroups g ON gu.userSID = g.SID " +
                $"WHERE gu.grpSID = @grpsid";

            // Sendet den SQL Befehl an den SQL Server
            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            // Erstellt einen neuen Parameter mit der SID
            var serverNameParam = new SqlParameter("grpsid", System.Data.SqlDbType.NVarChar)
            {
                Value = grpSID
            };

            // Bindet den Parameter an die Abfrage
            cmd.Parameters.Add(serverNameParam);

            // Benutzt den SQL Reader um über alle Zeilen der Abfrage zu gehen
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Liest den Count aus
                    string user = (reader[0] != null && reader[0] != DBNull.Value) ? reader.GetString(0) : null;
                    string group = (reader[1] != null && reader[1] != DBNull.Value) ? reader.GetString(1) : null;

                    // Ein User
                    if (user != null && group == null)
                        list.Add(user);
                    else if (user == null && group != null)
                        GetUserInGroup(group, ref list);
                }
            }
        }


        /// <summary>
        /// Führt einen SQL Command aus und gibt einen Count zurück
        /// </summary>
        /// <param name="sql">SQL Befehl mit Count</param>
        /// <returns></returns>
        private int GetCount(string sql)
        {
            // Sendet den SQL Befehl an den SQL Server
            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            // Benutzt den SQL Reader um über alle Zeilen der Abfrage zu gehen
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Liest den Count aus
                    int count = reader.GetInt32(0);
                    return count;
                }
            }
            return -1;
        }

        #endregion
    }
}
