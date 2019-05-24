using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPSDeamon
{
    class Log
    {
        public static void writeLog(string logText)
        {
            string binaryPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            string path = binaryPath + @"\log";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string log = DateTime.Now.ToShortDateString() + "-" + DateTime.Now.ToLongTimeString() + " - " + logText;
            string filename = path + @"\" + DateTime.Now.ToString("yyyy-MM") + "_Log.txt";

            StreamWriter LogWriter = new StreamWriter(filename, true);
            LogWriter.WriteLine(log);
            LogWriter.Close();
        }
    }
}
