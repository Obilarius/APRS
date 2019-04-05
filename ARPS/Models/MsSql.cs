using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPS.Classes
{
    public class MsSql
    {
        public MsSql()
        {
            SqlConnection con = new SqlConnection(@"Data Source=PC-W10-SW\MSSQLSERVER_DEV;Initial Catalog=ArgesPerm;Integrated Security=True");


            //con.Open();

            //string sql = "SELECT ID, Directory FROM dirs";
            //SqlCommand cmd = new SqlCommand(sql, con);

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
    }
}
