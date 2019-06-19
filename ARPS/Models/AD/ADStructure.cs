using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPS
{
    public static class ADStructure
    {
        /// <summary>
        /// Fragt die Datenbank nach der übergebenen sid ab und gibt ein ADElement zurück
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
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
                    ADElementType type = (isAdmin) ? ADElementType.Administrator : ADElementType.User;

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
        /// Gibt alle AD User zurück die in der Datenbank stehen
        /// </summary>
        /// <returns></returns>
        public static List<ADElement> GetAllADUsers(bool onlyEnabled = false)
        {
            // Erstellt leere Liste die mit den Rückgabewerten befüllt wird
            List<ADElement> retList = new List<ADElement>();

            // erstellt eine MSSQL Verbindung und öffnet Sie
            var mssql = new MsSql();
            mssql.Open();

            // Der SQL Befehl um alle User abzurufen
            string sql = $"SELECT * " +
                $"FROM {mssql.TBL_AD_Users}";

            if (onlyEnabled)
                sql += $" WHERE Enabled = 1";

            // Sendet den SQL Befehl an den SQL Server
            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            // Benutzt den SQL Reader um über alle Zeilen der Abfrage zu gehen
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Speichert die Daten des Readers in einzelne Variablen
                    string sid = reader["SID"].ToString();
                    string displayName = reader["DisplayName"].ToString();
                    string samAccountName = reader["SamAccountName"].ToString();
                    string distinguishedName = reader["DistinguishedName"].ToString();
                    string userPrincipalName = reader["UserPrincipalName"].ToString();
                    bool enabled = Convert.ToBoolean(reader["Enabled"]);

                    bool isAdmin = IsUserAdmin(sid);
                    ADElementType type = (isAdmin) ? ADElementType.Administrator : ADElementType.User;

                    retList.Add(new ADElement(sid, displayName, samAccountName, distinguishedName, userPrincipalName, enabled, type));
                }
            }

            // Schließt die MSSQL verbindung
            mssql.Close();

            return retList;
        }

        /// <summary>
        /// Fragt die Datenbank nach der übergebenen sid ab und gibt ein ADElement zurück
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static ADElement GetADGroup(string sid)
        {
            // erstellt eine MSSQL Verbindung und öffnet Sie
            var mssql = new MsSql();
            mssql.Open();

            // Der SQL Befehl um alle Ordner abzurufen die root sind
            string sql = $"SELECT * " +
                $"FROM {mssql.TBL_AD_Groups} " +
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
                    string name = reader["Name"].ToString();
                    string samAccountName = reader["SamAccountName"].ToString();
                    string distinguishedName = reader["DistinguishedName"].ToString();
                    string description = reader["Description"].ToString();
                    bool isSecurityGroup = Convert.ToBoolean(reader["IsSecurityGroup"]);
                    string groupScopeString = reader["GroupScope"].ToString();
                    GroupScope groupScope;

                    switch (groupScopeString)
                    {
                        case "Global":
                            groupScope = GroupScope.Global;
                            break;
                        case "Local":
                            groupScope = GroupScope.Local;
                            break;
                        default:
                            groupScope = GroupScope.Universal;
                            break;
                    }

                    // Schließt die MSSQL verbindung
                    mssql.Close();

                    return new ADElement(sid, name, samAccountName, distinguishedName, description, isSecurityGroup, groupScope);
                }
            }

            // Schließt die MSSQL verbindung
            mssql.Close();

            return null;
        }

        /// <summary>
        /// Fragt die Datenbank nach der übergebenen sid ab und gibt ein ADElement zurück
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static ADElement GetADComputer(string sid)
        {
            // erstellt eine MSSQL Verbindung und öffnet Sie
            var mssql = new MsSql();
            mssql.Open();

            // Der SQL Befehl um alle Ordner abzurufen die root sind
            string sql = $"SELECT * " +
                $"FROM {mssql.TBL_AD_Computers} " +
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
                    string name = reader["Name"].ToString();
                    string samAccountName = reader["SamAccountName"].ToString();
                    string distinguishedName = reader["DistinguishedName"].ToString();
                    string description = reader["Description"].ToString();
                    bool enabled = Convert.ToBoolean(reader["Enabled"]);
                    DateTime lastLogon = Convert.ToDateTime(reader["LastLogon"]);
                    DateTime lastPasswordSet = Convert.ToDateTime(reader["LastPasswordSet"]);

                    // Schließt die MSSQL verbindung
                    mssql.Close();

                    return new ADElement(sid, name, samAccountName, distinguishedName, description, enabled, lastLogon, lastPasswordSet);
                }
            }

            // Schließt die MSSQL verbindung
            mssql.Close();

            return null;
        }

        /// <summary>
        /// Gibt ein ADElement der angefragten SID zurück.
        /// Überprüft alle User, Gruppen und Computer
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static ADElement GetADElement(string sid)
        {

            // Überprüft ob die übergebene SID ein User ist
            ADElement returnElement = GetADUser(sid);

            // Falls das Element gefunden wurde wird hier beendet und das Element zurückgegeben
            if (returnElement != null)
            {
                bool isUserAdmin = IsUserAdmin(sid);

                return returnElement;
            }

            // Überprüft ob die übergebene SID eine Gruppe ist
            returnElement = GetADGroup(sid);

            // Falls das Element gefunden wurde wird hier beendet und das Element zurückgegeben
            if (returnElement != null)
                return returnElement;

            // Überprüft ob die übergebene SID ein Computer ist
            returnElement = GetADComputer(sid);

            // Falls das Element gefunden wurde wird hier beendet und das Element zurückgegeben
            if (returnElement != null)
                return returnElement;

            // Falls kein Element gefunden wurde wird
            return null;
        }

        /// <summary>
        /// Überprüft ob ein User Administrator ist
        /// </summary>
        /// <param name="sid">Die SID des Benutzers der überprüft werden soll</param>
        /// <returns></returns>
        public static bool IsUserAdmin(string sid)
        {
            // erstellt leere Liste in die alle Administratoren kommen
            List<ADElement> allAdmins = new List<ADElement>();

            allAdmins.AddRange(GetUserInGroup("S-1-5-32-544"));
            allAdmins.AddRange(GetUserInGroup("S-1-5-21-3723797570-695524079-4101376058-512"));

            foreach (var admin in allAdmins)
            {
                if (admin.SID == sid)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gibt alle User einer Gruppe zurück. Geht auch rekursiv in alle Gruppen bis alle User aufgelöst sind
        /// </summary>
        /// <param name="grpSID">Die Gruppen SID von der man die User haben möchte</param>
        public static List<ADElement> GetUserInGroup(string grpSID)
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

        /// <summary>
        /// Gibt alle Gruppen eines Users zurück. Geht auch rekursiv in alle Gruppen in Gruppen.
        /// </summary>
        /// <param name="userSID"></param>
        /// <returns></returns>
        public static List<ADElement> GetGroupsFromUser(string userSID)
        {
            List<ADElement> retList = new List<ADElement>();

            // erstellt eine MSSQL Verbindung und öffnet Sie
            var mssql = new MsSql();
            mssql.Open();

            // Der SQL Befehl um alle Gruppen eines 
            string sql = $"SELECT g.* " +
                $"FROM ARPS_Test.dbo.grp_user gu " +
                $"LEFT JOIN ARPS_Test.dbo.adgroups g " +
                $"ON gu.grpSID = g.SID " +
                $"WHERE gu.userSID = @UserSID";

            // Sendet den SQL Befehl an den SQL Server
            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            //Parameter anhängen
            cmd.Parameters.AddWithValue("@UserSID", userSID);

            // Benutzt den SQL Reader um über alle Zeilen der Abfrage zu gehen
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Speichert die Daten des Readers in einzelne Variablen
                    string sid = reader["SID"].ToString();
                    string samAccountName = reader["SamAccountName"].ToString();
                    string distinguishedName = reader["DistinguishedName"].ToString();
                    string name = reader["Name"].ToString();
                    string description = reader["Description"].ToString();
                    bool isSecurityGroup = Convert.ToBoolean(reader["IsSecurityGroup"]);
                    string gS = reader["GroupScope"].ToString();

                    GroupScope groupScope;

                    switch (gS)
                    {
                        case "Global":
                            groupScope = GroupScope.Global;
                            break;
                        case "Local":
                            groupScope = GroupScope.Local;
                            break;
                        default:
                            groupScope = GroupScope.Universal;
                            break;
                    }

                    // Fügt die Gruppe zur Liste hinzu
                    retList.Add(new ADElement(sid, name, samAccountName, distinguishedName, description, isSecurityGroup, groupScope));

                    // Ruft diese Funktion rekursiv auf und überprüft somit auch alle Gruppen in Gruppen
                    retList.AddRange(GetGroupsFromUser(sid));
                }
            }

            // Schließt die MSSQL verbindung
            mssql.Close();

            return retList;
        }
    }
}
