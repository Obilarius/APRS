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
            Log.writeLine("######################################");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            MsSql.DeleteTempTables();
            MsSql.CreateTempTables();
            Log.writeLine("Temp Datenbanken wurden erstellt");

            // Arbeitet das AD ab. Liest User, Gruppen und Computer ein
            Log.write("AD wird eingelesen...", true, false);
            WorkOnAD();
            Log.write("fertig", false, true);

            // Liest die Config ein
            Log.write("Config wird eingelesen...", true, false);
            Config config;
            try
            {
                config = new Config();
            }
            catch (Exception ex)
            {
                Log.writeLine("CONFIG: " + ex.Message);
                throw;
            }
            Log.write("fertig", false, true);

            // Läuft über jeden Server der Config
            foreach (var server in config.Servers)
            {
                Log.writeLine(server.Name + " wird gescannt...");
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

                Log.writeLine(server.Name + " wurde eingetragen");
            }

            MsSql.WriteTempToLive();

            sw.Stop();
            Log.writeLine("Deamon wird beendet");
            Log.writeLine("Laufzeit: " + (int)sw.Elapsed.TotalMinutes);
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

            int shareIndex = 1;
            // Schleife über jede ausgelesene Freigabe 
            foreach (Share share in shareColl)
            {
                Log.write(shareIndex + "/" + shareColl.Count + " - "  + share.NetName + " wird ausgelesen...", true, false);
                // Ruft alle Informationen ab und schreibt sie in die Datenbank
                FSWorker.GetSharesSecurity(share, server.DisplayName);

                Log.write(" fertig", false, true);
                shareIndex++;
            }
        }

    }

    
}