using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ARPS
{
    public class OU
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
        public OU()
        {
            // erstellt und öffnet eine neue MsSql Verbindung
            mssql = new MsSql();
            mssql.Open();

            // Erzeugt eine neue Liste mit Einträgen
            Entrys = new List<CountEnrty>();

            #region Computer
            Entrys.Add(new CountEnrty { Name = "Computer", Count = GetCountComputers().ToString() });
            #endregion

            #region Computer (deaktiviert)
            Entrys.Add(new CountEnrty { Name = "Computer (deaktiviert)", Count = GetCountDisabledComputers().ToString() });
            #endregion

            #region Benutzte OUs
            Entrys.Add(new CountEnrty { Name = "Benutzte OUs", Count = GetCountOU().ToString() });
            #endregion


            mssql.Close();
        }

        #endregion

        #region Get methods


        /// <summary>
        /// Gibt die Anzahl aller Computer im AD zurück
        /// </summary>
        /// <returns></returns>
        private int GetCountComputers()
        {
            return GetCount($"SELECT Count(*) FROM adcomputers");
        }

        /// <summary>
        /// Gibt die Anzahl aller deaktivierten Computer zurück
        /// </summary>
        /// <returns></returns>
        private int GetCountDisabledComputers()
        {
            return GetCount($"SELECT Count(*) FROM adcomputers c WHERE c.Enabled = 0");
        }

        /// <summary>
        /// Gibt die Anzahl aller benutzten OUs zurück
        /// </summary>
        /// <returns></returns>
        private int GetCountOU()
        {
            var sql = $"SELECT u.DistinguishedName " +
                $"FROM adusers u " +
                $"WHERE u.DistinguishedName IS NOT NULL " +
                $"UNION " +
                $"SELECT g.DistinguishedName " +
                $"FROM adgroups g " +
                $"WHERE g.DistinguishedName IS NOT NULL";

            // Sendet den SQL Befehl an den SQL Server
            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            List<string> ouList = new List<string>();

            // Benutzt den SQL Reader um über alle Zeilen der Abfrage zu gehen
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Liest den DistinguishedName aus
                    string disName = reader.GetString(0);

                    // Regex zum auslesen der einzelnen OUs aus dem DistinguishedName
                    string pattern = @"(?<=OU=)[\w -]+(?=,)";

                    // Geht über alle gefunden OUs
                    foreach (Match m in Regex.Matches(disName, pattern))
                    {
                        // Fügt jede OU der Liste hinzu
                        ouList.Add(m.Value);
                    }
                }
            }

            // Löscht Duplikate aus der Liste
            ouList = ouList.Distinct().ToList();

            return ouList.Count();
        }

        #endregion

        #region Helper Functions

 
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
