using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace ARPSScheduleDeamon
{
    class Program
    {
        static void Main(string[] args)
        {
            WorkScheduleFromDB();
        }

        /// <summary>
        /// Arbeitet alle noch nicht fetiggestellten Mitgliedschaften ab
        /// </summary>
        private static void WorkScheduleFromDB()
        {
            // Erstellt und öffnet die SQL Verbindung
            MsSql mssql = new MsSql();
            mssql.Open();

            // Der SQL Befehl der alle nicht abgeschlossenen Zeilen abfrägt
            string sql = $"SELECT * FROM schedule WHERE Completed = 0";

            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Die einzelnen Spalten des Eintrags
                    int ID = (int)reader["ID"];
                    string Username = reader["Username"].ToString();
                    string UserSid = reader["UserSid"].ToString();
                    string Groupname = reader["Groupname"].ToString();
                    string GroupSid = reader["GroupSid"].ToString();
                    DateTime StartDate = (DateTime)reader["StartDate"];
                    DateTime EndDate = (DateTime)reader["EndDate"];

                    DateTime EndDatePlus1 = (EndDate != DateTime.MaxValue) ? EndDate : EndDate.AddDays(1);

                    string Status = reader["Status"].ToString();
                    string Creator = reader["Creator"].ToString();
                    string Comment = reader["Comment"].ToString();

                    // Das heutige Datum
                    DateTime now = DateTime.Today;

                    // Falls das Startdatum in der Vergangenheit liegt, das Enddatum in der Zukunft und der Status "planned" ist
                    // Plan startet
                    if (StartDate <= now && now <= EndDatePlus1 && Status == ConstStatus.Planned)
                    {
                        AddUserToGroup(UserSid, GroupSid);
                        WriteNoteToUser(UserSid, DateTime.Now.ToShortDateString() + " - " + Creator + " - Grp: " + Groupname + " hinzugefügt");
                        SetNewStatus(ID, ConstStatus.Set, false);
                    }
                    
                    // Falls der folge Tag des Enddatums in der Vergangenheit liegt
                    // Plan ist abgelaufen
                    if (EndDatePlus1 <= now)
                    {
                        RemoveUserFromGroup(UserSid, GroupSid);
                        WriteNoteToUser(UserSid, DateTime.Now.ToShortDateString() + " - " + Creator + " - Grp: " + Groupname + " entfernt");
                        SetNewStatus(ID, ConstStatus.Terminate, true);
                    }

                    // Überprüft die gelöschten Einträge die aber noch nicht abgearbeitet worden sind
                    if (Status == "deleted")
                    {
                        // StartDate liegt in der Vergangenheit
                        // Die Gruppe wurde also schon gesetzt und muss wieder gelöscht werden
                        if (now <= StartDate)
                        {
                            RemoveUserFromGroup(UserSid, GroupSid);
                            WriteNoteToUser(UserSid, DateTime.Now.ToShortDateString() + " - " + Creator + " - Grp: " + Groupname + " entfernt");
                        }

                        // Setzt die Zeile auf Complete true
                        SetNewStatus(ID, Status, true);
                    }
                }
            }

            mssql.Close();
        }

        /// <summary>
        /// Führt ein Update auf der Datenbank aus und ändert den Status und den Completed Wert
        /// </summary>
        /// <param name="id">Die ID des Datensatzes der geändert werden soll</param>
        /// <param name="status">Der neue Status der gesetzt wird</param>
        /// <param name="completed">Ob der Plan abgeschlossen ist oder nicht</param>
        private static void SetNewStatus(int id, string status, bool completed)
        {
            // Erstellt eine Datenbankverbindung
            var mssql = new MsSql();
            // Öffnet die Datenbankverbindung
            mssql.Open();

            // Der Update SQL Befehl
            string sql = $"UPDATE schedule SET Status = @status, Completed = @completed WHERE ID = @id";

            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            // Hängt die Parameter an
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@completed", (completed) ? 1 : 0 );
            cmd.Parameters.AddWithValue("@status", status);

            // Führt die Query aus
            cmd.ExecuteNonQuery();

            // Schliest die Datenbankverbindung
            mssql.Close();
        }

        /// <summary>
        /// Schreibt eine Notiz in den Info Bereich des Users im AD
        /// </summary>
        /// <param name="userSid">Die SID des Users bei dem die Notiz angehängt werden soll</param>
        /// <param name="note">Die Notiz die angehängt werden soll</param>
        private static void WriteNoteToUser(string userSid, string note)
        {
            try
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain))
                {
                    // Sucht den User im AD anhand der userSid
                    UserPrincipal user = UserPrincipal.FindByIdentity(pc, IdentityType.Sid, userSid);
                    DirectoryEntry udo = user.GetUnderlyingObject() as DirectoryEntry;

                    // Liest den InfoText des Users aus
                    string _note = (udo.Properties["info"].Value == null) ? "" : udo.Properties["info"].Value.ToString();

                    // Hängt die neue Notiz an die Alte und schreibt diese wieder in den ausgelesenen User
                    udo.Properties["info"].Value = _note + "\r\n" + note;
                    // Speichert den bearbeiteten User im AD
                    user.Save();
                }
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException E)
            {
                //doSomething with E.Message.ToString(); 

            }
        }

        /// <summary>
        /// Fügt einen User einer Gruppe hinzu
        /// </summary>
        /// <param name="userSid">Der User der der Gruppe hinzugefügt werden soll</param>
        /// <param name="groupSid">Die Gruppe der der User hinzugefügt werden soll</param>
        private static void AddUserToGroup(string userSid, string groupSid)
        {
            try
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain))
                {
                    // Sucht die Gruppe anhand der SID
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(pc, IdentityType.Sid, groupSid);
                    // Fügt den User anhand der Sid zur Gruppe hinzu
                    group.Members.Add(pc, IdentityType.Sid, userSid);
                    // Speichert die Gruppe wieder
                    group.Save();
                }
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException E)
            {
                //doSomething with E.Message.ToString(); 

            }
        }

        /// <summary>
        /// Löscht einen User aus einer Gruppe
        /// </summary>
        /// <param name="userSid">Die SID des Users der aus der Gruppe entfernt werden soll</param>
        /// <param name="groupSid">Die SID der Gruppe aus der der User gelöscht werden soll</param>
        private static void RemoveUserFromGroup(string userSid, string groupSid)
        {
            try
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain))
                {
                    // Sucht die Gruppe anhand der SID
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(pc, IdentityType.Sid, groupSid);
                    // Löscht den User anhand der SID aus der Gruppe
                    group.Members.Remove(pc, IdentityType.Sid, userSid);
                    // Speichert die Gruppe wieder
                    group.Save();
                }
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException E)
            {
                //doSomething with E.Message.ToString(); 

            }
        }
    }
}
