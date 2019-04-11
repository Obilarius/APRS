﻿
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ARPS
{
    public static class DirectoryStructure
    {
        #region Get Lists

        /// <summary>
        /// Gibt eine Liste mit allen angegebenen Servers zurück.
        /// </summary>
        /// <param name="serverNames"></param>
        /// <returns></returns>
        public static List<DirectoryItem> GetServers(List<string> serverNames)
        {
            // Erstellt eine leere Liste die später zurück gegeben werden kann
            List<DirectoryItem> retList = new List<DirectoryItem>();

            // Geht über alle übergebenen ServerNamen
            foreach (var server in serverNames)
            {
                // Fügt ein neues DirectoryItem hinzu
                retList.Add(new DirectoryItem
                {
                    Id = -1,
                    FullPath = $"\\\\{server.Substring(0,1).ToUpper() + server.Substring(1)}",
                    Owner = "no Owner",
                    Type = DirectoryItemType.Server
                });
            }

            // Gibt eine Liste mit DirectoryItems zurück die die Server beinhaltet
            return retList;
        }


        /// <summary>
        /// Gibt eine Liste mit allen Unterordnern des angegebenen Servers zurück die keine ParentId besitzen.
        /// </summary>
        /// <param name="serverName">Der Servername der abgefragt wird (zb "apollon")</param>
        /// <returns></returns>
        public static List<DirectoryItem> GetServerAndSharedFolders(string serverName)
        {
            // erstellt eine MSSQL Verbindung und öffnet Sie
            var mssql = new MsSql();
            mssql.Open();

            // Der SQL Befehl um alle Ordner abzurufen die mit dem "ServerName" beginnen aber keine ParentId besitzen
            string sql = $"SELECT ID, Directory, Owner " +
                $"FROM dirs " +
                $"WHERE ParentID IS NULL AND Directory LIKE @serverName " +
                $"ORDER BY Directory ";

            // Sendet den SQL Befehl an den SQL Server
            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            // Erstellt einen neuen Parameter mit dem Servernamen
            var serverNameParam = new SqlParameter("serverName", System.Data.SqlDbType.NVarChar)
            {
                Value = $"\\\\{serverName}\\%"
            };

            // Bindet den Parameter an die Abfrage
            cmd.Parameters.Add(serverNameParam);

            // Erstellt eine leere Liste die später zurück gegeben werden kann
            List<DirectoryItem> retList = new List<DirectoryItem>
            {
                // Erstellt das Item für den Server
                new DirectoryItem
                {
                    Id = -1,
                    FullPath = $"\\\\{serverName}",
                    Owner = "no Owner",
                    Type = DirectoryItemType.Server
                }
            };


            // Benutzt den SQL Reader um über alle Zeilen der Abfrage zu gehen
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Liest aus der jeweiligen Zeile die einzelnen Werte aus
                    int id = reader.GetInt32(0);
                    string dir = reader.GetString(1);
                    string owner = reader.GetString(2);

                    // Erstellt ein neues DirectoryItem und fügt es der Liste hinzu
                    retList.Add(new DirectoryItem
                    {
                        Id = id,
                        FullPath = dir,
                        Owner = owner,
                        Type = DirectoryItemType.SharedFolder
                    });

                }
            }

            // Schließt die MSSQL verbindung
            mssql.Close();

            // Gibt die Liste zurück
            return retList;
        }

        /// <summary>
        /// Gibt eine Liste mit allen Unterordnern die die übergebene ID als ParentID besitzen
        /// </summary>
        /// <param name="Id">Die ParentID</param>
        /// <returns></returns>
        public static List<DirectoryItem> GetChildren(int Id)
        {
            // erstellt eine MSSQL Verbindung und öffnet Sie
            var mssql = new MsSql();
            mssql.Open();

            // Der SQL Befehl um alle Ordner abzurufen die die übergebene ParentID besitzen
            string sql = $"SELECT ID, Directory, Owner, ParentID " +
                $"FROM dirs " +
                $"WHERE ParentID = @parentId " +
                $"ORDER BY Directory ";

            // Sendet den SQL Befehl an den SQL Server
            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            // Erstellt die ParentId als Parameter
            var parentIdParam = new SqlParameter("parentId", System.Data.SqlDbType.Int)
            {
                Value = Id
            };

            // Bindet den Parameter an die Abfrage
            cmd.Parameters.Add(parentIdParam);

            // Erstellt eine leere Liste die später zurück gegeben werden kann
            List<DirectoryItem> retList = new List<DirectoryItem>();

            // Benutzt den SQL Reader um über alle Zeilen der Abfrage zu gehen
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Liest aus der jeweiligen Zeile die einzelnen Werte aus
                    var id = reader.GetInt32(0);
                    var dir = reader.GetString(1);
                    var owner = reader.GetString(2);
                    var parentId = reader.GetInt32(3);

                    // Erstellt ein neues DirectoryItem und fügt es der Liste hinzu
                    retList.Add(new DirectoryItem
                    {
                        Id = id,
                        FullPath = dir,
                        Owner = owner,
                        Type = DirectoryItemType.Folder,
                        ParentID = parentId
                    });

                }
            }

            // Schließt die MSSQL verbindung
            mssql.Close();

            // Gibt die Liste zurück
            return retList;
        }

        /// <summary>
        /// Gibt eine Liste mit allen Unterordnern die die übergebene ID als ParentID besitzen
        /// </summary>
        /// <param name="fullPath">Der volle Pfad</param>
        /// <returns></returns>
        public static List<DirectoryItem> GetChildren(string fullPath)
        {
            // erstellt eine MSSQL Verbindung und öffnet Sie
            var mssql = new MsSql();
            mssql.Open();

            // Der SQL Befehl um alle Ordner abzurufen die mit dem Pfad beginnen
            string sql = $"SELECT ID, Directory, Owner " +
                $"FROM dirs " +
                $"WHERE ParentID IS NULL AND Directory LIKE @fullPath " +
                $"ORDER BY Directory";

            // Sendet den SQL Befehl an den SQL Server
            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            // Erstellt den vollen Pfad als Parameter
            var fullPathParam = new SqlParameter("fullPath", System.Data.SqlDbType.NVarChar)
            {
                Value = fullPath + "\\%"
            };

            // Bindet den Parameter an die Abfrage
            cmd.Parameters.Add(fullPathParam);

            // Erstellt eine leere Liste die später zurück gegeben werden kann
            List<DirectoryItem> retList = new List<DirectoryItem>();

            // Benutzt den SQL Reader um über alle Zeilen der Abfrage zu gehen
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Liest aus der jeweiligen Zeile die einzelnen Werte aus
                    int id = reader.GetInt32(0);
                    string dir = reader.GetString(1);
                    string owner = reader.GetString(2);
                    int parentId = -1;

                    // Erstellt ein neues DirectoryItem und fügt es der Liste hinzu
                    retList.Add(new DirectoryItem
                    {
                        Id = id,
                        FullPath = dir,
                        Owner = owner,
                        Type = DirectoryItemType.SharedFolder,
                        ParentID = parentId
                    });

                }
            }

            return retList;
        }

        #endregion

        #region Checks

        /// <summary>
        /// Prüft ob das übergebene Item Kinder (Unterordner) besitzt
        /// </summary>
        /// <param name="item">Das Elternelement das überprüft werden soll</param>
        /// <returns></returns>
        public static bool HasChild(int parentId)
        {
            // Wenn -1 als Id übergeben wird ist es ein Server der immer Kinder hat
            if(parentId == -1)
                return true;

            // erstellt eine MSSQL Verbindung und öffnet Sie
            var mssql = new MsSql();
            mssql.Open();

            // Der SQL Befehl um die Anzahl der Unterordner zu bekommen
            string sql = $"SELECT Count(*) FROM dirs WHERE ParentID = @parentId";

            // Sendet den SQL Befehl an den SQL Server
            SqlCommand cmd = new SqlCommand(sql, mssql.Con);

            // Erstellt die Id des Elternitems als Parameter
            var parentIdParam = new SqlParameter("parentId", System.Data.SqlDbType.Int)
            {
                Value = parentId
            };

            // Bindet den Parameter an die Abfrage
            cmd.Parameters.Add(parentIdParam);

            // Führt die Abfrage aus und speichert den Count der Unterordner
            int count = (int)cmd.ExecuteScalar();

            // Gibt true zurück wenn es mehr als 0 Kinder gibt, ansonsten false
            return (count > 0) ? true : false;
        }

        #endregion

        #region Helper

        /// <summary>
        /// Gibt nur den Ordnernamen eines vollen Pfades zurück
        /// </summary>
        /// <param name="path">Der volle Pfad</param>
        /// <returns></returns>
        public static string GetFolderName(string path)
        {
            // Wenn wir keinen Pfad bekommen gib empty zurück
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            // Findet das letzte Backslash im Pfad
            var lastIndex = path.LastIndexOf('\\');

            // Wenn wir kein Backslash finden, gib den ganzen Pfad zurück
            if (lastIndex <= 0)
                return path;

            // Return der Namen nach dem letzten Backslash
            return path.Substring(lastIndex + 1);
        }


        #endregion
    }
}