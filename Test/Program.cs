using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Trinet.Networking;

namespace Test
{
    class Program
    {
        static MsSql mssql;

        static void Main(string[] args)
        {
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
                    if (found.SamAccountName == "walzenbach")
                    {

                    }
                }
            }


            Console.WriteLine("Belibige Taste Drücken zum beenden...");
            Console.ReadKey();
        }

    }

}
