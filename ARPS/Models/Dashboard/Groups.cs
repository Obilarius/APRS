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
            Entrys.Add(new CountEnrty { Name = "Alle Gruppen", Count = GetCountGroups() });
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
            // Der SQL Befehl um AD Gruppen abzufragen
            string sql = $"SELECT count(*) FROM adgroups WHERE DistinguishedName IS NOT NULL";

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
