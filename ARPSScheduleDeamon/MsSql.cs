using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPSScheduleDeamon
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
            // Erstellt die Verbindung zum MsSQL Server
            //Con = new SqlConnection(@"Data Source=PC-W10-SW\MSSQLSERVER_DEV;Initial Catalog=ArgesPerm;Integrated Security=True;MultipleActiveResultSets=True");
            try
            {
                Con = new SqlConnection(@"Data Source=ARPS\SQLEXPRESS;Initial Catalog=ARPS;User Id=LokalArps;Password=nopasswd;MultipleActiveResultSets=True");
            }
            catch (Exception ex)
            {
                throw new Exception("Es konnte keine Vebindung zum MSSQL Server hergestellt werden! /n/r" + ex.Message);
            }
        }

        /// <summary>
        /// Öffnet die Verbindung zum MSSQL Server
        /// </summary>
        public void Open ()
        {
            Con.Open();
        }

        /// <summary>
        /// Schließt die Verbindung zum MSSQL Server
        /// </summary>
        public void Close ()
        {
            Con.Close();
        }
    }
}
