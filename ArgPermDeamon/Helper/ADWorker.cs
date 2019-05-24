using ARPSMSSQL;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace ARPSDeamon
{
    /// <summary>
    /// Bearbeitet alle Anfragen die gegen das AD gehen
    /// </summary>
    public static class ADWorker
    {
        /// <summary>
        /// Funktion liest alle User, Gruppen und Computer aus dem AD aus und speichert sie in der Datenbank
        /// </summary>
        public static void ReadCompleteAD()
        {
            GetAllADUsers();
            GetAllADGroups();
            GetAllADComputer();
        }

        /// <summary>
        /// Liest alle User aus dem Ad und speicht sie in der Datenbank
        /// </summary>
        static void GetAllADUsers()
        {
            MsSql mssql = new MsSql();
            mssql.Open();

            // create your domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            // define a "query-by-example" principal - here, we search for a UserPrincipal 
            UserPrincipal qbeUser = new UserPrincipal(ctx);

            // create your principal searcher passing in the QBE principal    
            PrincipalSearcher srch = new PrincipalSearcher(qbeUser);

            // find all matches
            foreach (var found in srch.FindAll())
            {
                if (found is UserPrincipal user)
                {
                    int enabled = (bool)user.Enabled ? 1 : 0;
                    string sql = $"IF NOT EXISTS (SELECT * FROM {MsSql.TBL_tmp_AD_Users} WHERE SID = '{user.Sid}') " +
                                        $"INSERT INTO {MsSql.TBL_tmp_AD_Users}(SID, DisplayName, SamAccountName, DistinguishedName, UserPrincipalName, Enabled) " +
                                        $"VALUES ('{user.Sid}', '{user.DisplayName}', '{user.SamAccountName}', '{user.DistinguishedName}', '{user.UserPrincipalName}', '{enabled}') " +
                                 $"ELSE " +
                                        $"UPDATE {MsSql.TBL_tmp_AD_Users} " +
                                        $"SET DisplayName = '{user.DisplayName}'," +
                                            $"SamAccountName = '{user.SamAccountName}'," +
                                            $"DistinguishedName = '{user.DistinguishedName}'," +
                                            $"UserPrincipalName = '{user.UserPrincipalName}'," +
                                            $"Enabled = '{enabled}'" +
                                        $"WHERE SID = '{user.Sid}'";

                    SqlCommand cmd = new SqlCommand(sql, mssql.Con);
                    cmd.ExecuteNonQuery();
                }
            }

            mssql.Close();
        }

        /// <summary>
        /// Liest alle Gruppen aus der Datenbank aus und speichert sie in der Datenbank
        /// </summary>
        static void GetAllADGroups()
        {
            MsSql mssql = new MsSql();
            mssql.Open();

            // create your domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            GroupPrincipal qbeGrp = new GroupPrincipal(ctx);

            // create your principal searcher passing in the QBE principal    
            PrincipalSearcher srch = new PrincipalSearcher(qbeGrp);

            // find all matches
            foreach (var found in srch.FindAll())
            {
                if (found is GroupPrincipal grp)
                {
                    int isSecurityGroup = (bool)grp.IsSecurityGroup ? 1 : 0;

                    string sql = $"IF NOT EXISTS (SELECT * FROM {MsSql.TBL_tmp_AD_Groups} WHERE SID = '{grp.Sid}') " +
                                        $"INSERT INTO {MsSql.TBL_tmp_AD_Groups}(SID, SamAccountName, DistinguishedName, Name, Description, IsSecurityGroup, GroupScope) " +
                                        $"VALUES ('{grp.Sid}', " +
                                            $"'{grp.SamAccountName}', " +
                                            $"'{grp.DistinguishedName}', " +
                                            $"'{grp.Name}', " +
                                            $"'{grp.Description}', " +
                                            $"'{isSecurityGroup}', " +
                                            $"'{grp.GroupScope}') " +
                                 $"ELSE " +
                                        $"UPDATE {MsSql.TBL_tmp_AD_Groups} " +
                                        $"SET SamAccountName = '{grp.SamAccountName}', " +
                                            $"DistinguishedName = '{grp.DistinguishedName}', " +
                                            $"Name = '{grp.Name}', " +
                                            $"Description = '{grp.Description}', " +
                                            $"IsSecurityGroup = '{isSecurityGroup}', " +
                                            $"GroupScope = '{grp.GroupScope}'" +
                                        $"WHERE SID = '{grp.Sid}'";

                    SqlCommand cmd = new SqlCommand(sql, mssql.Con);
                    cmd.ExecuteNonQuery();

                    // fill the Table grp_user with the users in this group
                    GetAllUserInGroups(mssql.Con, grp);
                }
            }

            mssql.Close();
        }

        /// <summary>
        /// Liest aus alle User einer Gruppe aus und speichert das Matching in der Datenbank
        /// </summary>
        /// <param name="con">Bekommt eine bestehende SQL Verbindung übergeben</param>
        /// <param name="grp">Das Pricipal der Gruppe</param>
        static void GetAllUserInGroups(SqlConnection con, GroupPrincipal grp)
        {
            foreach (var user in grp.Members)
            {
                string sql = $"IF NOT EXISTS (SELECT * FROM {MsSql.TBL_tmp_AD_UserInGroup} WHERE userSID = '{user.Sid}' AND grpSID = '{grp.Sid}') " +
                                $"INSERT INTO {MsSql.TBL_tmp_AD_UserInGroup}(userSID, grpSID) " +
                                $"VALUES ('{user.Sid}', '{grp.Sid}')";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Liest alle Computer aus dem Ad uns spiecht diese in der Datenbank
        /// </summary>
        static void GetAllADComputer()
        {
            MsSql mssql = new MsSql();
            mssql.Open();

            // create your domain context
            ComputerPrincipal ctx = new ComputerPrincipal(new PrincipalContext(ContextType.Domain));

            // create your principal searcher passing in the QBE principal    
            PrincipalSearcher srch = new PrincipalSearcher(ctx);

            // find all matches
            foreach (var found in srch.FindAll())
            {
                if (found is ComputerPrincipal computer)
                {
                    int enabled = (bool)computer.Enabled ? 1 : 0;
                    string sql = $"IF NOT EXISTS (SELECT * FROM {MsSql.TBL_tmp_AD_Computers} WHERE SID = '{computer.Sid}') " +
                                        $"INSERT INTO {MsSql.TBL_tmp_AD_Computers}(SID, SamAccountName, Name, DistinguishedName, DisplayName, Description, Enabled, LastLogon, LastPasswordSet) " +
                                        $"VALUES ('{computer.Sid}', '{computer.SamAccountName}', '{computer.Name}', '{computer.DistinguishedName}', '{computer.DisplayName}', " +
                                        $"'{computer.Description}', '{enabled}', '{computer.LastLogon}', '{computer.LastPasswordSet}') " +
                                 $"ELSE " +
                                        $"UPDATE {MsSql.TBL_tmp_AD_Computers} " +
                                        $"SET SamAccountName = '{computer.SamAccountName}'," +
                                            $"Name = '{computer.Name}'," +
                                            $"DistinguishedName = '{computer.DistinguishedName}'," +
                                            $"DisplayName = '{computer.DisplayName}'," +
                                            $"Description = '{computer.Description}'," +
                                            $"Enabled = '{enabled}'," +
                                            $"LastLogon = '{computer.LastLogon}'," +
                                            $"LastPasswordSet = '{computer.LastPasswordSet}'" +
                                        $"WHERE SID = '{computer.Sid}'";

                    SqlCommand cmd = new SqlCommand(sql, mssql.Con);
                    cmd.ExecuteNonQuery();
                }
            }

            mssql.Close();
        }
    }
}
