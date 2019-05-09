using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

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

            FileSystemRights _r = (FileSystemRights)1179785;

            Console.WriteLine("Überprüfe FileSystemRights von: " + _r);
            Console.WriteLine();

            //Console.WriteLine(_r.HasFlag(FileSystemRights.Read));

            foreach (FileSystemRights f in (FileSystemRights[])Enum.GetValues(typeof(FileSystemRights)))
            {
                Console.WriteLine(f.ToString().PadLeft(28) + "  --  " + _r.HasFlag(f));
            }

            Console.ReadKey();
        }


    }

}
