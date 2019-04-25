using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;

namespace MSSQL_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var mssql = new MsSql();

            mssql.Open();


            // Der SQL Befehl um AD User abzufragen
            string sql = $"SELECT TOP 10 DisplayName FROM adusers";

            // Sendet den SQL Befehl an den SQL Server
            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            Console.WriteLine();
            // Benutzt den SQL Reader um über alle Zeilen der Abfrage zu gehen
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Liest den Count aus
                    string name = reader.GetString(0);
                    Console.WriteLine(name);
                }
            }

            Console.WriteLine();



            mssql.Close();



            // create your domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
            // define a "query-by-example" principal - here, we search for a UserPrincipal 
            UserPrincipal qbeUser = new UserPrincipal(ctx);

            // Erstellt den Eintrag der während des Ladens angezeigt wird
            UserPrincipal loading = new UserPrincipal(ctx)
            {
                Name = "loading ..."
            };

            // Fügt den Lade User der Liste hinzu die angezeigt wird
            var UsersFiltered = new ObservableCollection<UserPrincipal>
            {
                loading
            };

            // create your principal searcher passing in the QBE principal    
            PrincipalSearcher srch = new PrincipalSearcher(qbeUser);

            List<UserPrincipal> lst = new List<UserPrincipal>();
            // find all matches
            foreach (var found in srch.FindAll())
            {
                if (found is UserPrincipal user)
                {
                    if (user.Enabled.Value)
                        Console.WriteLine(user.DisplayName);
                }
            }


            Console.ReadKey();
        }
    }
}
