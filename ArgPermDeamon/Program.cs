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
            // Prüft ob ein Name für den Server übergeben wurde
            if (name != null && name.Trim().Length > 0)
            {
                // Switch über den Type um jeden Type anders zu behandeln
                switch (type)
                {
                    // Type = FileServer
                    case ConfigType.FileServer:
                        WorkFileServer(name, displayname, type);
                        break;
                    // Default
                    default:
                        break;
                }

            }



            //foreach (var path in paths)
            //{
            //DirectoryInfo dInfo = new DirectoryInfo(path);
            //FSWorker.GetDirectorySecurity(dInfo, -1);
            //}

            //ADWorker.ReadAD();


            Console.WriteLine("FERTIG");
            Console.ReadKey();
        }


        

        static void WorkFileServer(string name, string displayname, ConfigType type)
        {
            ShareCollection shareColl = ShareCollection.GetShares(name);
            if (shareColl != null)
            {
                foreach (Share share in shareColl)
                {
                    FSWorker.GetSharesSecurity(share, displayname);
                }
            }
        }

    }

    
}