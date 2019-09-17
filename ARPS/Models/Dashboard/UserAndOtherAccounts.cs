using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPS
{
    public class UserAndOtherAccounts
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
        public UserAndOtherAccounts()
        {
            // erstellt und öffnet eine neue MsSql Verbindung
            mssql = new MsSql();
            mssql.Open();

            // Erzeugt eine neue Liste mit Einträgen
            Entrys = new List<CountEnrty>();

            #region Benutzer
            // Erstelle Eintrag für Benutzer
            Entrys.Add(new CountEnrty { Name = "Benutzer", Count = GetCountUser().ToString() });
            #endregion

            #region Benutzer (deaktiviert)
            // Erstelle Eintrag für deaktivierte Benutzer
            Entrys.Add(new CountEnrty { Name = "Benutzer (deaktiviert)", Count = GetCountDisabledUser().ToString() });
            #endregion

            #region Administratoren
            // Erstellt die leere Liste in die die UserSID eingetragen werden sollen
            var admins = new List<string>();
            // Schreibt alle User der Gruppe "Administratoren" in die Liste
            GetUserInGroup("S-1-5-32-544", ref admins);
            // Schreibt alle User der Gruppe "Domain-Admins" in die Liste
            GetUserInGroup("S-1-5-21-3723797570-695524079-4101376058-512", ref admins);

            // Löscht alle Duplikate aus der Liste
            admins = admins.Distinct().ToList();

            // Erstelle Eintrag für Administratoren
            Entrys.Add(new CountEnrty { Name = "Admininstratoren", Count = admins.Count.ToString() });
            #endregion

            #region Administratoren (deaktiviert) 
            Entrys.Add(new CountEnrty { Name = "Admininstratoren (deaktiviert)", Count = GetCountDisabledAdmins(admins).ToString() });
            #endregion

            // Schließt die MsSql Verbindung
            mssql.Close();
        }

        #endregion

        #region Get methods

        /// <summary>
        /// Gibt die Anzahl der Accounts im AD zurück
        /// </summary>
        /// <returns></returns>
        private int GetCountUser()
        {
            // Der SQL Befehl um AD User abzufragen
            string sql = $"SELECT count(*) FROM adusers WHERE DistinguishedName IS NOT NULL";

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

        /// <summary>
        /// Gibt die Anzahl der deaktivierten User im AD zurück
        /// </summary>
        /// <returns></returns>
        private int GetCountDisabledUser()
        {
            // Der SQL Befehl um AD User abzufragen
            string sql = $"SELECT count(*) FROM adusers WHERE DistinguishedName IS NOT NULL AND Enabled = 0";

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

        /// <summary>
        /// Gibt die Anzahl der deaktivierten Adminaccounts zurück
        /// </summary>
        /// <param name="ListOfAdmins">Die Liste mit SIDs der Administratoren</param>
        /// <returns></returns>
        private int GetCountDisabledAdmins(List<string> ListOfAdmins)
        {
            // Der SQL Befehl um AD User abzufragen
            string sql = $"SELECT count(*) FROM (SELECT * FROM adusers WHERE ";

            // Fügt für jeden Admin in der Liste eine OR Abfrage hinzu
            for (int i = 0; i < ListOfAdmins.Count(); i++)
            {
                sql += $"SID = '{ListOfAdmins[i]}'";

                if (i < ListOfAdmins.Count() - 1)
                    sql += " OR ";
            }

            sql += $") a WHERE Enabled = 0";
                

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

        #endregion
    }
}
