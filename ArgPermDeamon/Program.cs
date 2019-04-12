using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ArgPermDeamon
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();

            //FillParentID();

            List<string> paths = new List<string>();
            paths.Add(@"\\apollon\Administration");
            paths.Add(@"\\apollon\Appslab");
            paths.Add(@"\\apollon\Archiv");
            paths.Add(@"\\apollon\Contacts");
            paths.Add(@"\\apollon\Documentation");
            paths.Add(@"\\apollon\HardDev");
            paths.Add(@"\\apollon\Install");
            paths.Add(@"\\apollon\Marketing");
            paths.Add(@"\\apollon\Mechanik");
            paths.Add(@"\\apollon\Metallography");
            paths.Add(@"\\apollon\PM");
            paths.Add(@"\\apollon\Production");
            paths.Add(@"\\apollon\Public");
            paths.Add(@"\\apollon\Purchasing");
            paths.Add(@"\\apollon\QMS");
            paths.Add(@"\\apollon\REMINST");
            paths.Add(@"\\apollon\Sales");
            paths.Add(@"\\apollon\SoftDev");
            paths.Add(@"\\apollon\User");
            paths.Add(@"\\apollon\Vorlagen");
            //int levels = 999;

            //List<string> output = new List<string>();
            //foreach (var path in paths)
            //{
            //    sw.Reset();
            //    sw.Start();

            //    GetDirectorySecurity(path, levels);

            //    sw.Stop();
            //    output.Add(path + " -- " + sw.ElapsedMilliseconds);
            //}

            ADWorker.ReadAD();

            //foreach (var line in output)
            //    Console.WriteLine(line);

            Console.WriteLine("FERTIG");
            Console.ReadKey();
        }

        static void GetDirectorySecurity(string dir, int levels, int curLevel = 0)
        {
            SqlConnection con = new SqlConnection(@"Data Source=PC-W10-SW\MSSQLSERVER_DEV;Initial Catalog=ArgesPerm;Integrated Security=True");
            con.Open();
            string[] dirs = Directory.GetDirectories(@dir);

            if (curLevel == 0)
            {
                dirs = new string[] { dir };
            }

            foreach (string directory in dirs)
            {
                try
                {
                    if (curLevel < levels)
                        GetDirectorySecurity(@directory, levels, curLevel + 1);


                    DirectoryInfo dInfo = new DirectoryInfo(directory);
                    DirectorySecurity dSecurity = dInfo.GetAccessControl();

                    // Get the Info about the Folderowner
                    IdentityReference owner = dSecurity.GetOwner(typeof(SecurityIdentifier));  // FÜR SID
                    string ownerSID = owner.Value;

                    if (curLevel <= 2)
                    {
                        Console.WriteLine(directory);
                    }

                    // Write or Update MSSQL
                    string hash = Hash(directory);
                    var dirTemp = directory.Replace("'", "''");
                    string sql = $"IF NOT EXISTS (SELECT * FROM dirs WHERE Hash = '{hash}') " +
                                        $"INSERT INTO dirs(Directory, Owner, Hash) " +
                                        $"VALUES ('{dirTemp}', '{ownerSID}', '{hash}') " +
                                 $"ELSE " +
                                        $"UPDATE dirs " +
                                        $"SET Owner = '{ownerSID}' " +
                                        $"WHERE Hash = '{hash}'";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.ExecuteNonQuery();

                    // Get the ID from the Directory Row
                    sql = $"SELECT ID FROM dirs WHERE Hash = '{hash}'";
                    cmd = new SqlCommand(sql, con);
                    int dirId = -1;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            dirId = (int)reader["id"];
                    }

                    AuthorizationRuleCollection rules = dSecurity.GetAccessRules(true, true, typeof(SecurityIdentifier)); // FÜR SID
                    ////AuthorizationRuleCollection acl = dSecurity.GetAccessRules(true, true, typeof(NTAccount));        // FÜR NAMEN (ARGES/walzenbach)
                    foreach (FileSystemAccessRule rule in rules)
                    {
                        if (dirId != -1)
                        {
                            // Write or Update MSSQL
                            string rightHash = Hash(
                                dirId.ToString() + 
                                rule.IdentityReference + 
                                rule.InheritanceFlags);

                            int isInherited = rule.IsInherited ? 1 : 0;
                            int ACT = (rule.AccessControlType.ToString() == "Allow") ? 1 : 0;

                            string rightSql = $"IF NOT EXISTS (SELECT * FROM rights WHERE Hash = '{rightHash}') " +
                                                    $"INSERT INTO rights(DirID, Hash, IdentityReference, AccessControlType, FileSystemRights, IsInherited, InheritanceFlags) " +
                                                    $"VALUES ('{dirId}', '{rightHash}', '{rule.IdentityReference}', '{ACT}', '{rule.FileSystemRights}', '{isInherited}', '{rule.InheritanceFlags}') " +
                                             $"ELSE " +
                                                    $"UPDATE rights " +
                                                    $"SET IdentityReference = '{rule.IdentityReference}', " +
                                                        $"AccessControlType = '{ACT}', " +
                                                        $"FileSystemRights = '{rule.FileSystemRights}', " +
                                                        $"IsInherited = '{isInherited}', " +
                                                        $"InheritanceFlags = '{rule.InheritanceFlags}'" +
                                                    $"WHERE Hash = '{hash}'";

                            cmd = new SqlCommand(rightSql, con);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            con.Close();
        }

        static void FillParentID ()
        {
            SqlConnection con = new SqlConnection(@"Data Source=PC-W10-SW\MSSQLSERVER_DEV;Initial Catalog=ArgesPerm;Integrated Security=True");
            con.Open();

            string sql = "SELECT ID, Directory FROM dirs";
            SqlCommand cmd = new SqlCommand(sql, con);

            List<string[]> allList = new List<string[]>();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var dir = reader.GetString(1);

                    allList.Add(new[] { id.ToString(), dir });
                }
            }

            DateTime StartTime = DateTime.Now;
            Stopwatch swComp = new Stopwatch();
            swComp.Start();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            int index = 0;
            DateTime EndTime = DateTime.Now;
            foreach (var row in allList)
            {
                sw.Restart();

                var dir = row[1];
                var id = row[0];

                Regex pattern = new Regex(@"^.+\\");
                Match match = pattern.Match(dir);
                var parentDir = match.Value;
                parentDir = parentDir.Substring(0, parentDir.Length - 1);


                // Ohne Parameter
                //sql = $"UPDATE dirs SET ParentID = (SELECT TOP(1) ID FROM dirs WHERE Directory = '{parentDir}') WHERE ID = { id }";
                //cmd = new SqlCommand(sql, con);
                //cmd.ExecuteNonQuery();

                // Mit Parameter
                string sql2 = "UPDATE dirs SET ParentID = (SELECT TOP(1) ID FROM dirs WHERE Directory = @parentDir) WHERE ID = @id";
                cmd = new SqlCommand(sql2, con);

                var parentDirParam = new SqlParameter("parentDir", System.Data.SqlDbType.NVarChar);
                parentDirParam.Value = parentDir;

                var idParam = new SqlParameter("id", System.Data.SqlDbType.Int);
                idParam.Value = id;

                cmd.Parameters.Add(parentDirParam);
                cmd.Parameters.Add(idParam);
                cmd.ExecuteNonQuery();


                index++;
                float percent = ((float)index / (float)allList.Count) * 100f;
                if (index % 100 == 0)
                {
                    TimeSpan geschaetzteLaufzeit = TimeSpan.FromTicks((long)(swComp.Elapsed.Ticks * (100 / percent)));
                    EndTime = StartTime + geschaetzteLaufzeit;
                }
                
                Console.WriteLine($"{percent.ToString("0.0000")}% - ENDTIME: {EndTime.ToLongTimeString()} - EXECUTE: {sw.ElapsedMilliseconds} -- {dir}");
            }

            swComp.Stop();
            sw.Stop();
            con.Close();



            //using (SqlDataReader reader = cmd.ExecuteReader())
            //{
            //    while (reader.Read())
            //    {
            //        //Console.WriteLine("{0}\t{1}", reader.GetInt32(0), reader.GetString(1));
            //        var dir = reader.GetString(1);
            //        var id = reader.GetInt32(0);

            //        Regex pattern = new Regex(@"^[\w\\ ]+\\");
            //        Match match = pattern.Match(dir);
            //        var parentDir = match.Value;
            //        parentDir = parentDir.Substring(0, parentDir.Length - 1);


            //        using (SqlConnection con2 = new SqlConnection(@"Data Source=PC-W10-SW\MSSQLSERVER_DEV;Initial Catalog=ArgesPerm;Integrated Security=True"))
            //        { 
            //            con2.Open();

            //            string innersql = $"SELECT ID FROM dirs WHERE Directory = '{parentDir}'";
            //            SqlCommand innercmd = new SqlCommand(innersql, con2);

            //            SqlDataReader innerreader = innercmd.ExecuteReader();

            //            if (innerreader.HasRows)
            //            {
            //                while (innerreader.Read())
            //                {
            //                    var parentID = innerreader.GetInt32(0);

            //                    string updatesql = $"UPDATE dirs " +
            //                        $"SET ParentID = {parentID} " +
            //                        $"WHERE ID = {id}";

            //                    innerreader.Close();

            //                    SqlCommand updatecmd = new SqlCommand(updatesql, con2);
            //                    updatecmd.ExecuteNonQuery();

            //                    Console.WriteLine(dir);

            //                    break;
            //                }
            //            }

            //            con2.Close();
            //        }
            //    }
            //}

            //con.Close();
        }





        static long GetDirectorySize(string p)
        {
            // Get array of all file names.
            string[] a = Directory.GetFiles(p, "*.*", SearchOption.AllDirectories);

            // Calculate total bytes of all files in a loop.
            long b = 0;
            foreach (string name in a)
            {
                // Use FileInfo to get length of each file.
                try
                {
                    FileInfo info = new FileInfo(name);
                    b += info.Length;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            // Return total size
            return b;
        }

        // Returns the human-readable file size for an arbitrary, 64-bit file size 
        // The default format is "0.### XB", e.g. "4.2 KB" or "1.434 GB"
        static string GetBytesReadable(long i)
        {
            // Get absolute value
            long absolute_i = (i < 0 ? -i : i);
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (i >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (i >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (i >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = (readable / 1024);
            // Return formatted number with suffix
            return readable.ToString("0.### ") + suffix;
        }

        static string Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}