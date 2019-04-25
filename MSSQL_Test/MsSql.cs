using System;
using System.Data.SqlClient;

namespace MSSQL_Test
{
    public class MsSql
    {
        /// <summary>
        /// Hält die MSSQL Vebindung
        /// </summary>
        public SqlConnection Con { get; private set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public MsSql()
        {
            Console.WriteLine("Versuche MSSQL Vebindung aufzubauen");
            // Erstellt die Verbindung zum MsSQL Server
            //Con = new SqlConnection(@"Data Source=PC-W10-SW\MSSQLSERVER_DEV;Initial Catalog=ArgesPerm;Integrated Security=True;MultipleActiveResultSets=True");
            try
            {
                Con = new SqlConnection(@"Data Source=8MAN\SQLEXPRESS;Initial Catalog=ARPS;User Id=LokalArps;Password=nopasswd;MultipleActiveResultSets=True");
                Console.WriteLine("Verbindung aufgebaut");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Es konnte keine Vebindung zum MSSQL Server hergestellt werden! /n/r" + ex.Message);
            }
        }

        /// <summary>
        /// Öffnet die Verbindung zum MSSQL Server
        /// </summary>
        public void Open ()
        {
            Console.WriteLine("Versuche MSSQL Vebindung zu öffnen");

            try
            {
                Con.Open();
                Console.WriteLine("Verbindung offen");

            }
            catch (Exception)
            {

                Console.WriteLine("Fehler");
            }

        }

        /// <summary>
        /// Schließt die Verbindung zum MSSQL Server
        /// </summary>
        public void Close ()
        {
            Con.Close();
            Console.WriteLine("Verbindung wurde geschlossen");
        }
    }
}
