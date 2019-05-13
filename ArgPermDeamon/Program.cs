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
            XmlTextReader xtr = new XmlTextReader("config.xml");

            while (xtr.Read())
            {
                if (xtr.NodeType == XmlNodeType.Element && 
                    xtr.Name == "server" && 
                    xtr.AttributeCount == 3)
                {
                    string name = xtr.GetAttribute(0);
                    string displayname = xtr.GetAttribute(1);
                    int type = Convert.ToInt32(xtr.GetAttribute(2));


                    // Enumerate shares on a remote computer;
                    if (name != null && 
                        name.Trim().Length > 0 && 
                        type == 1)
                    {
                        ShareCollection shareColl = ShareCollection.GetShares(name);
                        if (shareColl != null)
                        {
                            foreach (Share share in shareColl)
                            {
                                Console.WriteLine("{0}: {1} [{2}] -- {3}", share.ShareType, share.ToString(), share.Path, share.Remark);


                                FSWorker.GetSharesSecurity(share, displayname);
                            }
                        }
                    }
                    
                }
            }


            //    List<string> paths = new List<string>();
            //paths.Add(@"\\apollon\Administration");
            //paths.Add(@"\\apollon\Appslab");
            //paths.Add(@"\\apollon\Archiv");
            //paths.Add(@"\\apollon\Contacts");
            //paths.Add(@"\\apollon\Documentation");
            //paths.Add(@"\\apollon\HardDev");
            //paths.Add(@"\\apollon\Install");
            //paths.Add(@"\\apollon\Marketing");
            //paths.Add(@"\\apollon\Mechanik");
            //paths.Add(@"\\apollon\Metallography");
            //paths.Add(@"\\apollon\PM");
            //paths.Add(@"\\apollon\Production");
            //paths.Add(@"\\apollon\Public");
            //paths.Add(@"\\apollon\Purchasing");
            //paths.Add(@"\\apollon\QMS");
            //paths.Add(@"\\apollon\REMINST");
            //paths.Add(@"\\apollon\Sales");
            //paths.Add(@"\\apollon\SoftDev");
            //paths.Add(@"\\apollon\User");
            //paths.Add(@"\\apollon\Vorlagen");



            //foreach (var path in paths)
            //{
                //DirectoryInfo dInfo = new DirectoryInfo(path);
                //FSWorker.GetDirectorySecurity(dInfo, -1);
            //}

            //ADWorker.ReadAD();


            Console.WriteLine("FERTIG");
            Console.ReadKey();
        }



    }
}