using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPS
{
    public class MsSql
    {
        public string TBL_AD_Users { get { return "dbo.adusers"; } }
        public string TBL_AD_Groups { get { return "dbo.adgroups"; } }
        public string TBL_AD_Computers { get { return "dbo.adcomputers"; } }

        public string TBL_FS_Dirs { get { return "fs.dirs"; } }
        public string TBL_FS_Shares { get { return "fs.shares"; } }
        public string TBL_FS_ACLs { get { return "fs.acls"; } }
        public string TBL_FS_ACEs { get { return "fs.aces"; } }

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
                Con = new SqlConnection(GetConString());
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

        /// <summary>
        /// Liest die Datei "mysql_config.txt" ein und baut daraus den Connection String zusammen
        /// </summary>
        /// <returns>Den Connection String für die MySQL Verbindung</returns>
        private string GetConString()
        {
            // Liest en Pfad der exe aus und baut den Pfad zur Config Datei zusammen
            string path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\mysql_config.txt";
            // Liest alle Zeilen der Config Datei in ein Array
            string[] configLines = File.ReadAllLines(path);
            // Verbindet die einzelnen Zeilen zu einem String mit ; getrennt
            string conString = string.Join(";", configLines);

            return conString;
        }
    }

}
