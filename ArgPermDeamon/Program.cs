using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Trinet.Networking;

namespace ARPSDeamon
{
    class Program
    {
        static void Main(string[] args)
        {
            // Liest die Config ein
            Config config = new Config();

            // Läuft über jeden Server der Config
            foreach (var server in config.Servers)
            {
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
            }

            // Arbeitet das AD ab. Liest User, Gruppen und Computer ein
            WorkOnAD();
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