using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace ArgPermDeamon
{
    class ADWorker
    {
        public static void ReadAD()
        {
            //GetAllADUsers();
            //GetAllADGroups();

            GetAllADComputer();
        }

        static void GetAllADUsers()
        {
            SqlConnection con = new SqlConnection(@"Data Source=PC-W10-SW\MSSQLSERVER_DEV;Initial Catalog=ArgesPerm;Integrated Security=True");
            con.Open();

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
                    string sql = $"IF NOT EXISTS (SELECT * FROM adusers WHERE SID = '{user.Sid}') " +
                                        $"INSERT INTO adusers(SID, DisplayName, SamAccountName, DistinguishedName, UserPrincipalName, Enabled) " +
                                        $"VALUES ('{user.Sid}', '{user.DisplayName}', '{user.SamAccountName}', '{user.DistinguishedName}', '{user.UserPrincipalName}', '{enabled}') " +
                                 $"ELSE " +
                                        $"UPDATE adusers " +
                                        $"SET DisplayName = '{user.DisplayName}'," +
                                            $"SamAccountName = '{user.SamAccountName}'," +
                                            $"DistinguishedName = '{user.DistinguishedName}'," +
                                            $"UserPrincipalName = '{user.UserPrincipalName}'," +
                                            $"Enabled = '{enabled}'" +
                                        $"WHERE SID = '{user.Sid}'";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.ExecuteNonQuery();
                }
            }

            con.Close();
        }

        static void GetAllADGroups()
        {
            SqlConnection con = new SqlConnection(@"Data Source=PC-W10-SW\MSSQLSERVER_DEV;Initial Catalog=ArgesPerm;Integrated Security=True");
            con.Open();

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

                    string sql = $"IF NOT EXISTS (SELECT * FROM adgroups WHERE SID = '{grp.Sid}') " +
                                        $"INSERT INTO adgroups(SID, SamAccountName, DistinguishedName, Name, Description, IsSecurityGroup, GroupScope) " +
                                        $"VALUES ('{grp.Sid}', " +
                                            $"'{grp.SamAccountName}', " +
                                            $"'{grp.DistinguishedName}', " +
                                            $"'{grp.Name}', " +
                                            $"'{grp.Description}', " +
                                            $"'{isSecurityGroup}', " +
                                            $"'{grp.GroupScope}') " +
                                 $"ELSE " +
                                        $"UPDATE adgroups " +
                                        $"SET SamAccountName = '{grp.SamAccountName}', " +
                                            $"DistinguishedName = '{grp.DistinguishedName}', " +
                                            $"Name = '{grp.Name}', " +
                                            $"Description = '{grp.Description}', " +
                                            $"IsSecurityGroup = '{isSecurityGroup}', " +
                                            $"GroupScope = '{grp.GroupScope}'" +
                                        $"WHERE SID = '{grp.Sid}'";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.ExecuteNonQuery();

                    // fill the Table grp_user with the users in this group
                    GetAllUserInGroups(con, grp.Sid, grp.Members);
                }
            }

            con.Close();
        }

        static void GetAllUserInGroups(SqlConnection con, SecurityIdentifier grpSid, PrincipalCollection members)
        {
            foreach (var user in members)
            {
                string sql = $"IF NOT EXISTS (SELECT * FROM grp_user WHERE userSID = '{user.Sid}' AND grpSID = '{grpSid}') " +
                                $"INSERT INTO grp_user(userSID, grpSID) " +
                                $"VALUES ('{user.Sid}', '{grpSid}')";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
            }
        }

        static void GetAllADComputer()
        {
            SqlConnection con = new SqlConnection(@"Data Source=PC-W10-SW\MSSQLSERVER_DEV;Initial Catalog=ArgesPerm;Integrated Security=True");
            con.Open();

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
                    string sql = $"IF NOT EXISTS (SELECT * FROM adcomputers WHERE SID = '{computer.Sid}') " +
                                        $"INSERT INTO adcomputers(SID, SamAccountName, Name, DistinguishedName, DisplayName, Description, Enabled, LastLogon, LastPasswordSet) " +
                                        $"VALUES ('{computer.Sid}', '{computer.SamAccountName}', '{computer.Name}', '{computer.DistinguishedName}', '{computer.DisplayName}', " +
                                        $"'{computer.Description}', '{enabled}', '{computer.LastLogon}', '{computer.LastPasswordSet}') " +
                                 $"ELSE " +
                                        $"UPDATE adcomputers " +
                                        $"SET SamAccountName = '{computer.SamAccountName}'," +
                                            $"Name = '{computer.Name}'," +
                                            $"DistinguishedName = '{computer.DistinguishedName}'," +
                                            $"DisplayName = '{computer.DisplayName}'," +
                                            $"Description = '{computer.Description}'," +
                                            $"Enabled = '{enabled}'," +
                                            $"LastLogon = '{computer.LastLogon}'," +
                                            $"LastPasswordSet = '{computer.LastPasswordSet}'" +
                                        $"WHERE SID = '{computer.Sid}'";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.ExecuteNonQuery();
                }
            }

            con.Close();
        }
    }
}
