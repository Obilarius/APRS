using ARPSMSSQL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Trinet.Networking;

namespace ARPSDeamon
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.writeLog("######################################");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            MsSql.DeleteTempTables();
            MsSql.CreateTempTables();
            Log.writeLog("Temp Datenbanken wurden erstellt");

            // Arbeitet das AD ab. Liest User, Gruppen und Computer ein
            WorkOnAD();
            Log.writeLog("AD wurde eingelesen");

            // Liest die Config ein
            Config config = new Config();

            // Läuft über jeden Server der Config
            foreach (var server in config.Servers)
            {
                Log.writeLog(server.Name + " wird gescannt...");
                // Switch über den Type um jeden Type anders zu behandeln
                switch (server.Type)
                {
                    // Type = FileServer
                    case ConfigType.FileServer:
                        WorkOnFileServer(server);
                        break;
                    // Default
                    default:
                        break;
                }

                Log.writeLog(server.Name + " wurde eingetragen");
            }

            sw.Stop();
            Log.writeLog("Deamon wird beendet");
            Log.writeLog("Laufzeit: " + sw.Elapsed.TotalMinutes);
        }


        /// <summary>
        /// Arbeitet das Active Directory ab
        /// </summary>
        static void WorkOnAD()
        {
            ADWorker.ReadCompleteAD();
        }
        
        /// <summary>
        /// Arbeitet einen FileServer ab
        /// </summary>
        /// <param name="server"></param>
        static void WorkOnFileServer(ConfigServer server)
        {
            // Liest die Shares des Servers aus
            ShareCollection shareColl = ShareCollection.GetShares(server.Name);

            // Wenn keine Shares gelesen werden konnten
            if (shareColl == null)
                return;

            // Schleife über jede ausgelesene Freigabe 
            foreach (Share share in shareColl)
            {
                // Ruft alle Informationen ab und schreibt sie in die Datenbank
                FSWorker.GetSharesSecurity(share, server.DisplayName);
            }
        }

    }

    
}