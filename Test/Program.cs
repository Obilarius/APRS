using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
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

        static void Main(string[] args)
        {
            #region Username
            Console.WriteLine("Username:");
            string username = Console.ReadLine();
            #endregion

            #region Password
            Console.WriteLine("Password:");

            string pass = "";
            do
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Substring(0, (pass.Length - 1));
                        Console.Write("\b \b");
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }
            } while (true);
            #endregion

            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain))
            {
                // validate the credentials
                bool isValid = pc.ValidateCredentials(username, pass);
            }


            Console.WriteLine("Belibige Taste Drücken zum beenden...");
            Console.ReadKey();
        }

    }

}
