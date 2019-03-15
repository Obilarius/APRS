using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArgPerm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //MSSQL sql = new MSSQL();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //SqlConnection con = new SqlConnection(
            //    @"Server=PC-W10-SW\MSSQLSERVER_DEV;Database=ArgesPerm;Trusted_Connection=True;"
            //);
            //SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM argarm.adgroups", con);
            //DataSet ds = new DataSet();
            //sda.Fill(ds, "argarm.adgroups");

            ////var result = from dirs in ds.Tables["argarm.dirs"].AsEnumerable()
            ////             where dirs.Field<string>("Directory").Substring(2).Split('\\').Length <= 2
            ////             orderby dirs.Field<string>("Directory")
            ////             group dirs by dirs.Field<string>("Directory") into gdirs
            ////             select new
            ////             {
            ////                 dir = gdirs.Key,
            ////             };

            //var result = from grps in ds.Tables["argarm.adgroups"].AsEnumerable()
            //             //where dirs.Field<string>("Directory").Substring(2).Split('\\').Length <= 2
            //             orderby grps.Field<string>("SamAccountName")
            //             group grps by grps.Field<string>("SamAccountName") into ggrps
            //             select new
            //             {
            //                 grps = ggrps.Key,
            //             };



            //foreach (var row in result)
            //{
            //    listBox_test.Items.Add(row.grps);
            //}

            GetAllDirs();

            sw.Stop();
            lbl_time.Content = sw.ElapsedMilliseconds;
        }

        

        void GetAllDirs()
        {
            DataClassesDataContext db = new DataClassesDataContext();
            var result = from dir in db.dirs
                         where dir.Directory.Substring(2).Length - dir.Directory.Substring(2).Replace("\\","").Length < 3   //.Substring(2).Split('\\').Length <= 2
                         group dir by dir.Directory into gdirs
                         orderby gdirs.Key
                         select new
                         {
                             dir = gdirs.Key,
                             //level = gdirs.Key.Replace('\\', '').Length - gdirs.Key.Length
                         };

            foreach (var row in result)
            {
                listBox_test.Items.Add(row.dir);
            }

        }

        
    }
}
