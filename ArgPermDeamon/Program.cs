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

namespace ARPSDeamon
{
    class Program
    {
        public static int Count { get; set; }
        public static int Index { get; set; }

        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

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


            #region Count Alle Dirs in DB
            //baut eine SQL Verbindung auf
            SqlConnection con = new SqlConnection(@"Data Source=8MAN\SQLEXPRESS;Initial Catalog=ARPS_Test;User Id=LokalArps;Password=nopasswd;MultipleActiveResultSets=True");
            // Der SQL Befehl zum überprüfen ob der jeweilige Eintrag schon vorhanden ist
            string sql = $"SELECT COUNT(*) FROM fs.dirs";
            SqlCommand cmd = new SqlCommand(sql, con);

            con.Open();
            Count = (int)cmd.ExecuteScalar();
            con.Close();
            #endregion


            foreach (var path in paths)
            {

                DirectoryInfo dInfo = new DirectoryInfo(path);
                GetDirectorySecurity(dInfo, -1);

                
            }

            //ADWorker.ReadAD();

            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalSeconds);

            Console.WriteLine("FERTIG");
            Console.ReadKey();
        }


        

        /// <summary>
        /// Liest den jeweiligen Ordner und rekursive alle Unterordner ab und speichert die Informationen in der Datenbank
        /// </summary>
        /// <param name="dInfo">Das DirectoryInfo Object des Ordners der verarbeitet werden soll</param>
        /// <param name="parentId">Die ID des übergeortneten Ordners</param>
        static void GetDirectorySecurity(DirectoryInfo dInfo, int parentId)
        {
            if (dInfo == null)
                return;

            if (dInfo.FullName.StartsWith($"\\\\apollon\\Administration\\ARGES_Intern\\"))
            {

            }

            //baut eine SQL Verbindung auf
            SqlConnection con = new SqlConnection(@"Data Source=8MAN\SQLEXPRESS;Initial Catalog=ARPS_Test;User Id=LokalArps;Password=nopasswd;MultipleActiveResultSets=True");

            // Der volle Pfad
            string _path_name = dInfo.FullName;
            // Die ID des übergeordneten Ordners
            int _parent_path_id = parentId;
            // Berechnet den Hash und speichert ihn
            string _path_hash = Hash(_path_name);
            // Ob der Ordner in der ersten Ebene ist oder nicht
            int _is_root = (_parent_path_id == -1) ? 1 : 0;
            
            // Die Ebene des Ordners
            int _scan_deepth = _path_name.TrimStart('\\').Split('\\').Count() - 1;


            // Ausgabe: Alle Pafde bis zur 3. Ebene werden ausgegeben
            Index++;
            if (_scan_deepth <= 5)
            {
                //Console.WriteLine(_path_name);
                float percent = (float)Index / (float)Count * 100f;
                Console.WriteLine(percent.ToString("n2") + $" % of {Count} -- {_path_name}");
            }



            #region Prüfung ob schon vorhanden ist
            // Der SQL Befehl zum überprüfen ob der jeweilige Eintrag schon vorhanden ist
            string sql = $"SELECT _path_id FROM fs.dirs WHERE _path_hash = @PathHash";
            SqlCommand cmd = new SqlCommand(sql, con);
            // Der Hash wird als Parameter an den SQL Befehl gehängt
            cmd.Parameters.AddWithValue("@PathHash", _path_hash);

            // Öffnet die SQL Verbindung, führt die Abfrage durch und schließt die Verbindung wieder
            con.Open();
            // Falls es den abgefragten Datensatz schon gibt, bekommt man in index die ID des Datensatzen, sonnst null
            var _path_id = cmd.ExecuteScalar();
            con.Close();
            #endregion


            #region Größenberechnung

            // Es gibt die Zeile
            long _size;
            if (_path_id != null)
            {
                sql = $"SELECT _size FROM fs.dirs WHERE _path_id = @PathId";
                cmd = new SqlCommand(sql, con);
                // Der Hash wird als Parameter an den SQL Befehl gehängt
                cmd.Parameters.AddWithValue("@PathId", _path_id);


                // Öffnet die SQL Verbindung, führt die Abfrage durch und schließt die Verbindung wieder
                con.Open();
                // Falls es den abgefragten Datensatz schon gibt, bekommt man in index die ID des Datensatzen, sonnst null
                var size = cmd.ExecuteScalar();
                con.Close();

                if ((long)size == 0 || size == null)
                    _size = DirSize(dInfo);
                else
                    _size = (long)size;
            }
            else
            {
                 _size = DirSize(dInfo);
            }
            #endregion


            // Liest alle Unterordner in ein Array
            DirectoryInfo[] childs;
            try
            {
                childs =  dInfo.GetDirectories();
            }
            catch (Exception)
            {
                childs = new DirectoryInfo[1];
            }


            // Liest die Infos über den Besitzer aus
            string _owner_sid = "0";
            try
            {
                DirectorySecurity dSecurity = dInfo.GetAccessControl();
                IdentityReference owner = dSecurity.GetOwner(typeof(SecurityIdentifier));  // FÜR SID
                _owner_sid = owner.Value;
            } catch (Exception) { }

            // Ob der Ordner Unterordner hat oder nicht
            int _has_children = (childs.Length > 0) ? 1 : 0;
            


            // Hash ist noch nicht vorhanden
            if (_path_id == null)
            {
                // Der SQL Befehl zum INSERT in die Datenbank
                sql = $"INSERT INTO fs.dirs(_path_name, _owner_sid, _path_hash, _parent_path_id, _is_root, _has_children, _scan_deepth, _size) " +
                      $"OUTPUT INSERTED._path_id " +
                      $"VALUES (@PathName, @OwnerSid, @PathHash, @ParentPathId, @IsRoot, @HasChildren, @ScanDeepth, @Size) ";

                cmd = new SqlCommand(sql, con);

                // Hängt die Parameter an
                cmd.Parameters.AddWithValue("@PathName", _path_name);
                cmd.Parameters.AddWithValue("@OwnerSid", _owner_sid);
                cmd.Parameters.AddWithValue("@PathHash", _path_hash);
                cmd.Parameters.AddWithValue("@ParentPathId", _parent_path_id);
                cmd.Parameters.AddWithValue("@IsRoot", _is_root);
                cmd.Parameters.AddWithValue("@HasChildren", _has_children);
                cmd.Parameters.AddWithValue("@ScanDeepth", _scan_deepth);
                cmd.Parameters.AddWithValue("@Size", _size);

                // Öffnet die SQL Verbindung
                con.Open();
                // Führt die Query aus
                _path_id = (int)cmd.ExecuteScalar();
                //Schließt die Verbindung
                con.Close();
            }
            // Hash ist noch nicht vorhanden
            else
            {
                // SQL Befehl zum Updaten des Eintrags
                sql = $"UPDATE fs.dirs " +
                      $"SET _path_name = @PathName, _owner_sid = @OwnerSid, _path_hash = @PathHash, _parent_path_id = @ParentPathId, " +
                      $"_is_root = @IsRoot, _has_children = @HasChildren, _scan_deepth = @ScanDeepth, _size = @Size " +
                      $"WHERE _path_id = @PathId";

                cmd = new SqlCommand(sql, con);

                // Hängt die Parameter an
                cmd.Parameters.AddWithValue("@PathName", _path_name);
                cmd.Parameters.AddWithValue("@OwnerSid", _owner_sid);
                cmd.Parameters.AddWithValue("@PathHash", _path_hash);
                cmd.Parameters.AddWithValue("@ParentPathId", _parent_path_id);
                cmd.Parameters.AddWithValue("@IsRoot", _is_root);
                cmd.Parameters.AddWithValue("@HasChildren", _has_children);
                cmd.Parameters.AddWithValue("@ScanDeepth", _scan_deepth);
                cmd.Parameters.AddWithValue("@Size", _size);
                cmd.Parameters.AddWithValue("@PathId", (int)_path_id);

                // Öffnet die SQL Verbindung
                con.Open();
                // Führt die Query aus
                cmd.ExecuteNonQuery();
                //Schließt die Verbindung
                con.Close();
            }

            // Geht über alle Unterordner (Kinder) und ruft die Funktion rekursiv auf
            foreach (DirectoryInfo child in childs)
            {
                GetDirectorySecurity(child, (int)_path_id);
            }

        }

        static void BACKUP_GetDirectorySecurity(string dir, int levels, int curLevel = 0)
        {
            //SqlConnection con = new SqlConnection(@"Data Source=8MAN\SQLEXPRESS;Initial Catalog=ARPS;User Id=LokalArps;Password=nopasswd;MultipleActiveResultSets=True");
            //con.Open();
            //string[] dirs = Directory.GetDirectories(@dir);

            //if (curLevel == 0)
            //{
            //    dirs = new string[] { dir };
            //}

            //foreach (string directory in dirs)
            //{
            //    try
            //    {
            //        if (curLevel < levels)
            //            GetDirectorySecurity(@directory, levels, curLevel + 1);


            //        DirectoryInfo dInfo = new DirectoryInfo(directory);
            //        DirectorySecurity dSecurity = dInfo.GetAccessControl();


            //        // Get the Info about the Folderowner
            //        IdentityReference owner = dSecurity.GetOwner(typeof(SecurityIdentifier));  // FÜR SID
            //        string ownerSID = owner.Value;

            //        if (curLevel <= 2)
            //        {
            //            Console.WriteLine(directory);
            //        }

            //        // Write or Update MSSQL
            //        string hash = Hash(directory);
            //        var dirTemp = directory.Replace("'", "''");
            //        string sql = $"IF NOT EXISTS (SELECT * FROM dirs WHERE Hash = '{hash}') " +
            //                            $"INSERT INTO dirs(Directory, Owner, Hash) " +
            //                            $"VALUES ('{dirTemp}', '{ownerSID}', '{hash}') " +
            //                     $"ELSE " +
            //                            $"UPDATE dirs " +
            //                            $"SET Owner = '{ownerSID}' " +
            //                            $"WHERE Hash = '{hash}'";

            //        SqlCommand cmd = new SqlCommand(sql, con);
            //        cmd.ExecuteNonQuery();

            //        // Get the ID from the Directory Row
            //        sql = $"SELECT ID FROM dirs WHERE Hash = '{hash}'";
            //        cmd = new SqlCommand(sql, con);
            //        int dirId = -1;
            //        using (SqlDataReader reader = cmd.ExecuteReader())
            //        {
            //            if (reader.Read())
            //                dirId = (int)reader["id"];
            //        }

            //        AuthorizationRuleCollection rules = dSecurity.GetAccessRules(true, true, typeof(SecurityIdentifier)); // FÜR SID
            //        ////AuthorizationRuleCollection acl = dSecurity.GetAccessRules(true, true, typeof(NTAccount));        // FÜR NAMEN (ARGES/walzenbach)
            //        foreach (FileSystemAccessRule rule in rules)
            //        {
            //            if (dirId != -1)
            //            {
            //                // Write or Update MSSQL
            //                string rightHash = Hash(
            //                    dirId.ToString() + 
            //                    rule.IdentityReference + 
            //                    rule.InheritanceFlags);

            //                int isInherited = rule.IsInherited ? 1 : 0;
            //                int ACT = (rule.AccessControlType.ToString() == "Allow") ? 1 : 0;

            //                string rightSql = $"IF NOT EXISTS (SELECT * FROM rights WHERE Hash = '{rightHash}') " +
            //                                        $"INSERT INTO rights(DirID, Hash, IdentityReference, AccessControlType, FileSystemRights, IsInherited, InheritanceFlags) " +
            //                                        $"VALUES ('{dirId}', '{rightHash}', '{rule.IdentityReference}', '{ACT}', '{rule.FileSystemRights}', '{isInherited}', '{rule.InheritanceFlags}') " +
            //                                 $"ELSE " +
            //                                        $"UPDATE rights " +
            //                                        $"SET IdentityReference = '{rule.IdentityReference}', " +
            //                                            $"AccessControlType = '{ACT}', " +
            //                                            $"FileSystemRights = '{rule.FileSystemRights}', " +
            //                                            $"IsInherited = '{isInherited}', " +
            //                                            $"InheritanceFlags = '{rule.InheritanceFlags}'" +
            //                                        $"WHERE Hash = '{hash}'";

            //                cmd = new SqlCommand(rightSql, con);
            //                cmd.ExecuteNonQuery();
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("Error: " + ex.Message);
            //    }
            //}

            //con.Close();
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


        /// <summary>
        /// Berechnet die Ordnergröße in Bytes
        /// </summary>
        /// <param name="d">DirectoryInfo Object</param>
        /// <returns>Größe in Bytes</returns>
        static long DirSize(DirectoryInfo d)
        {
            long size = 0;

            FileInfo[] fis;
            // Add file sizes.
            try
            {
                fis = d.GetFiles();
            }
            catch (Exception)
            {
                return size;
            }

            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size;
        }

        /// <summary>
        /// Returns the human-readable file size for an arbitrary, 64-bit file size 
        /// The default format is "0.### XB", e.g. "4.2 KB" or "1.434 GB"
        /// </summary>
        /// <param name="i">Wert in Bytes der Umgerechnet werden soll</param>
        /// <returns></returns>
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

        /// <summary>
        /// Berechnet einen SHA1 Hash aus dem Übergebenen String
        /// </summary>
        /// <param name="input">String der verhasht werden soll</param>
        /// <returns>SHA1 Hash</returns>
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