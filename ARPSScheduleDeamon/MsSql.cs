using System.Data.SqlClient;

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
            Con = new SqlConnection(@"Data Source=PC-W10-SW\MSSQLSERVER_DEV;Initial Catalog=ArgesPerm;Integrated Security=True;MultipleActiveResultSets=True");
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
