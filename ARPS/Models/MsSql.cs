using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPS
{
    public class MsSql
    {
        public SqlConnection Con { get; private set; }

        public MsSql()
        {
            Con = new SqlConnection(@"Data Source=PC-W10-SW\MSSQLSERVER_DEV;Initial Catalog=ArgesPerm;Integrated Security=True");


            //Con.Open();

            //string sql = "SELECT ID, Directory FROM dirs";
            //SqlCommand cmd = new SqlCommand(sql, Con);

            //using (SqlDataReader reader = cmd.ExecuteReader())
            //{
            //    while (reader.Read())
            //    {
            //        var id = reader.GetInt32(0);
            //        var dir = reader.GetString(1);

            //        allList.Add(new[] { id.ToString(), dir });
            //    }
            //}
        }

        public void Open ()
        {
            Con.Open();
        }

        public void Close ()
        {
            Con.Close();
        }
    }
}
