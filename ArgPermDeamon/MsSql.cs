using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPSMSSQL
{
    public class MsSql
    {
        #region Tabellennamen
        public string TBL_AD_Users { get { return "dbo.adusers"; } }
        public string TBL_AD_Groups { get { return "dbo.adgroups"; } }
        public string TBL_AD_Computers { get { return "dbo.adcomputers"; } }

        public string TBL_FS_Dirs { get { return "fs.dirs"; } }
        public string TBL_FS_Shares { get { return "fs.shares"; } }
        public string TBL_FS_ACLs { get { return "fs.acls"; } }
        public string TBL_FS_ACEs { get { return "fs.aces"; } }
        #endregion

        /// <summary>
        /// Connection String
        /// </summary>
        private string conString = @"Data Source=8MAN\SQLEXPRESS;Initial Catalog=ARPS;User Id=LokalArps;Password=nopasswd;MultipleActiveResultSets=True";

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
                Con = new SqlConnection(conString);
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
            if (Con.State == System.Data.ConnectionState.Open)
                return;
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
