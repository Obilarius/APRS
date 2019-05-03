using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static MsSql mssql;

        static void Main(string[] args)
        {
            mssql = new MsSql();
            mssql.Open();

            for (int i = 22; i > 1; i--)
            {
                Console.WriteLine(i);

                string sql = $"SELECT _path_id, _path_name, _parent_path_id " +
                    $"FROM dirs " +
                    $"WHERE _scan_deepth = {i} AND _has_children = 0";

                SqlCommand cmd = new SqlCommand(sql, mssql.Con);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string path = reader.GetString(1);
                        int parentId = reader.GetInt32(2);

                        string herkunft = id.ToString() + " -> " + parentId.ToString();
                        fillParent(parentId, herkunft);
                    }
                }
            }

            mssql.Close();


            Console.WriteLine("Fertig");
            Console.ReadKey();
        }


        static void fillParent(int id, string herkunft)
        {
            if (id == 90224)
                return;

            var sql = $"UPDATE dirs SET _has_children = 1 WHERE _path_id = {id}";

            //Console.WriteLine("ID: " + id);
            //Console.WriteLine(sql);

            var cmd = new SqlCommand(sql, mssql.Con);
            cmd.CommandTimeout = 0;
            cmd.ExecuteNonQuery();


            sql = $"SELECT _parent_path_id, _path_name " +
                    $"FROM dirs " +
                    $"WHERE _path_id = {id}";

            //Console.WriteLine(sql);

            cmd = new SqlCommand(sql, mssql.Con);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int _parentId = -10;
                    _parentId = reader.GetInt32(0);
                    var path = reader.GetString(1);
                    //Console.WriteLine(path);

                    if (_parentId != -10)
                    {
                        herkunft += " -> " + _parentId.ToString();
                        Console.WriteLine(herkunft);
                        fillParent(_parentId, herkunft);
                    }
                }
            }
        }


        void fillChilds ()
        {
            string sql = $"SELECT _path_id, _path_name " +
                $"FROM dirs " +
                $"WHERE _has_children = 0";

            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            var list = new List<string[]>();
            var index = 0;

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string path = reader.GetString(1);

                    index++;

                    list.Add(new[] { id.ToString(), path });
                }
            }

            Console.WriteLine("Select durchgeführt");

            var count = index;
            index = 0;

            sql = string.Empty;

            foreach (var item in list)
            {
                //if (item[1].ToLower().StartsWith("\\\\apollon\\user\\walzenbach"))
                //{
                var deepth = item[1].TrimStart('\\').Split('\\').Count();

                sql = $"SELECT TOP(1) _path_id FROM dirs WHERE _scan_deepth = {deepth} AND _path_name LIKE '{item[1]}\\%'";

                cmd = new SqlCommand(sql, mssql.Con);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    Console.WriteLine(item[1]);

                    while (reader.Read())
                    {
                        int id = -10;
                        id = reader.GetInt32(0);

                        if (id != -10)
                        {
                            sql = $"UPDATE dirs SET _has_children = 1 WHERE _path_id = '{item[0]}'";

                            cmd = new SqlCommand(sql, mssql.Con);
                            cmd.ExecuteNonQuery();
                        }


                        index++;

                        if (index % 50 == 0)
                        {
                            float percent = (float)index / (float)count * 100.0f;
                            Console.WriteLine(index + ": " + percent + " %");
                        }
                    }
                }
                //}

            }

        }
    }

}
