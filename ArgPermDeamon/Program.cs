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
using System.Xml;

namespace ARPSDeamon
{
    class Program
    {
        static void Main(string[] args)
        {

            using (XmlReader reader = XmlReader.Create(@"config.xml"))
            {
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "server"))
                    {
                        if (reader.HasAttributes)
                            Console.WriteLine(reader.GetAttribute("name") + " - " + reader.GetAttribute("displayname") + " - " + reader.GetAttribute("type"));
                    }


                    //if (reader.IsStartElement())
                    //{
                    //    //return only when you have START tag  
                    //    switch (reader.Name.ToString())
                    //    {
                    //        case "name":
                    //            Console.WriteLine("Name of the Element is : " + reader.ReadString());
                    //            break;
                    //        case "displayname":
                    //            Console.WriteLine("The displayname is : " + reader.ReadString());
                    //            break;
                    //        case "type":
                    //            Console.WriteLine("The Type is : " + reader.ReadString());
                    //            break;
                    //    }
                    //}
                }
            }


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



            foreach (var path in paths)
            {
                //DirectoryInfo dInfo = new DirectoryInfo(path);
                //FSWorker.GetDirectorySecurity(dInfo, -1);
            }

            //ADWorker.ReadAD();


            Console.WriteLine("FERTIG");
            Console.ReadKey();
        }



    }
}