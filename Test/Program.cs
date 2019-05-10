using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            //mssql = new MsSql();
            //mssql.Open();

            //for (int i = 22; i > 1; i--)
            //{
            //    Console.WriteLine(i);

            //    string sql = $"SELECT _path_id, _path_name, _parent_path_id " +
            //        $"FROM dirs " +
            //        $"WHERE _scan_deepth = {i} AND _has_children = 0";

            //    SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            //    using (SqlDataReader reader = cmd.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {
            //            int id = reader.GetInt32(0);
            //            string path = reader.GetString(1);
            //            int parentId = reader.GetInt32(2);

            //            string herkunft = id.ToString() + " -> " + parentId.ToString();
            //        }
            //    }
            //}

            //mssql.Close();


            //Console.WriteLine("Fertig");
            //Console.ReadKey();



            TestShares("filer", "filer");

            Console.ReadKey();
        }


        static void TestShares(string server, string displayname)
        {
            // Enumerate shares on a remote computer;
            if (server != null && server.Trim().Length > 0)
            {
                ShareCollection shi = ShareCollection.GetShares(server);
                if (shi != null)
                {
                    foreach (Share si in shi)
                    {
                        Console.WriteLine("{0}: {1} [{2}] -- {3}", si.ShareType, si, si.Path, si.Remark);
                    }
                }
            }


            Console.ReadLine();
        }

    }

}
