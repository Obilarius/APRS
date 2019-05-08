using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPS
{
    public static class ADStructure
    {
        public static ADElement GetADUser(string sid)
        {
            // erstellt eine MSSQL Verbindung und öffnet Sie
            var mssql = new MsSql();
            mssql.Open();

            // Der SQL Befehl um alle Ordner abzurufen die root sind
            string sql = $"SELECT * " +
                $"FROM {mssql.TBL_AD_Users} " +
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
                    // Speichert die Daten des Readers in einzelne Variablen
                    string displayName = reader["DisplayName"].ToString();
                    string samAccountName = reader["SamAccountName"].ToString();
                    string distinguishedName = reader["DistinguishedName"].ToString();
                    string userPrincipalName = reader["UserPrincipalName"].ToString();
                    bool enabled = Convert.ToBoolean(reader["Enabled"]);

                    bool isAdmin = IsUserAdmin(sid);
                    UserType type = (isAdmin) ? UserType.Administrator : UserType.User;

                    // Schließt die MSSQL verbindung
                    mssql.Close();

                    return new ADElement(sid, displayName, samAccountName, distinguishedName, userPrincipalName, enabled, type);
                }
            }

            // Schließt die MSSQL verbindung
            mssql.Close();

            return null;
        }

        /// <summary>
        /// Überprüft ob ein User Administrator ist
        /// </summary>
        /// <param name="sid">Die SID des Benutzers der überprüft werden soll</param>
        /// <returns></returns>
        private static bool IsUserAdmin(string sid)
        {
            return false;
        }

        /// <summary>
        /// Gibt alle User einer Gruppe zurück. Geht auch rekursiv in alle Gruppen bis alle User aufgelöst sind
        /// </summary>
        /// <param name="grpSID">Die Gruppen SID von der man die User haben möchte</param>
        private static List<ADElement> GetUserInGroup(string grpSID)
        {
            List<ADElement> retList = new List<ADElement>();

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
            cmd.Parameters.AddWithValue("@GroupSid", grpSID);

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
                        retList.AddRange(GetUserInGroup(sid));
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
                            retList.Add(new ADElement(sid, displayName, samAccountName, distinguishedName, userPrincipalName, enabled));
                        }

                    }
                }
            }

            // Schließt die MSSQL verbindung
            mssql.Close();

            return retList;
        }
    }
}
