using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Trinet.Networking;

namespace Test
{
    class Program
    {

        static void Main(string[] args)
        {
            ShareSecurity("Hades");

            Console.ReadKey();
        }



        private static void ShareSecurity(string ServerName)
        {
            // MSSQL VERBINDUNG
            var mssql = new MsSql();

            ConnectionOptions myConnectionOptions = new ConnectionOptions();

            myConnectionOptions.Impersonation = ImpersonationLevel.Impersonate;
            myConnectionOptions.Authentication = AuthenticationLevel.Packet;

            ManagementScope myManagementScope =
                new ManagementScope(@"\\" + ServerName + @"\root\cimv2", myConnectionOptions);

            myManagementScope.Connect();

            if (!myManagementScope.IsConnected)
                Console.WriteLine("could not connect");
            else
            {
                ManagementObjectSearcher myObjectSearcher =
                    new ManagementObjectSearcher(myManagementScope.Path.ToString(), "SELECT * FROM Win32_LogicalShareSecuritySetting");

                var shareList = myObjectSearcher.Get();

                foreach (ManagementObject share in shareList)
                {
                    Console.WriteLine("--------------------------------");
                    Console.WriteLine(share["Name"] as string);
                    InvokeMethodOptions options = new InvokeMethodOptions();
                    ManagementBaseObject outParamsMthd = share.InvokeMethod("GetSecurityDescriptor", null, options);
                    ManagementBaseObject descriptor = outParamsMthd["Descriptor"] as ManagementBaseObject;
                    ManagementBaseObject[] dacl = descriptor["DACL"] as ManagementBaseObject[];


                    #region Holt die PathID der Freigabe
                    string sql = "SELECT _path_id FROM fs.shares WHERE _unc_path_name = '\\\\" + ServerName + "\\" + share["Name"] + "'";
                    SqlCommand cmd = new SqlCommand(sql, mssql.Con);

                    // Öffnet die SQL Verbindung, führt die Abfrage durch und schließt die Verbindung wieder
                    mssql.Open();
                    var _path_id = cmd.ExecuteScalar();
                    
                    #endregion

                    foreach (ManagementBaseObject ace in dacl)
                    {
                        try
                        {
                            ManagementBaseObject trustee = ace["Trustee"] as ManagementBaseObject;

                            Console.WriteLine(
                                trustee["Domain"] as string + @"\" + trustee["Name"] as string + " (" + trustee["SIDString"] + ") : " +
                                ace["AccessMask"] as string + " " + ace["AceType"] as string + " -- " + ace["AceFlags"]
                            );


                            // Der SQL Befehl zum INSERT in die Datenbank
                            sql = $"INSERT INTO fs.share_aces (_sid, _rights, _type, _flags, _path_id) " +
                                  $"OUTPUT INSERTED._ace_id " +
                                  $"VALUES (@Sid, @Rights, @Type, @Flags, @PathId) ";

                            cmd = new SqlCommand(sql, mssql.Con);

                            // Hängt die Parameter an
                            cmd.Parameters.AddWithValue("@Sid", trustee["SIDString"] as string);
                            cmd.Parameters.AddWithValue("@Rights", Convert.ToInt32(ace["AccessMask"]));
                            cmd.Parameters.AddWithValue("@Type", Convert.ToInt32(ace["AceType"]));
                            cmd.Parameters.AddWithValue("@Flags", Convert.ToInt32(ace["AceFlags"]));
                            cmd.Parameters.AddWithValue("@PathId", (int)_path_id);

                            // Öffnet die SQL Verbindung
                            //mssql.Open();
                            // Führt die Query aus
                            _path_id = (int)cmd.ExecuteScalar();
                            //Schließt die Verbindung
                            //mssql.Close();
                        }
                        catch (Exception error)
                        {
                            Console.WriteLine("Error: " + error.ToString());
                        }
                    }

                    mssql.Close();
                }
            }
        }

    }

}
