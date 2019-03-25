using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
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
using MahApps.Metro.Controls;
using System.Windows.Shapes;

namespace ArgPerm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        Uri iconUri_folder = new Uri("pack://siteoforigin:,,,/Resources/win/Folder_16x.png");
        Uri iconUri_folderShared = new Uri("pack://siteoforigin:,,,/Resources/win/FolderShared_16x.png");
        Uri iconUri_server = new Uri("pack://siteoforigin:,,,/Resources/win/LocalServer_16x.png");

        Uri iconUri_user = new Uri("pack://siteoforigin:,,,/Resources/User_16x.png");
        Uri iconUri_group = new Uri("pack://siteoforigin:,,,/Resources/Group_16x.png");

        Uri iconUri_warning = new Uri("pack://siteoforigin:,,,/Resources/Warning_16x.png");
        Uri iconUri_lockOpen = new Uri("pack://siteoforigin:,,,/Resources/LockOpen_16x.png");
        Uri iconUri_lockClosed = new Uri("pack://siteoforigin:,,,/Resources/LockClosed_16x.png");

        public MainWindow()
        {
            InitializeComponent();


            MyTreeViewItem rootNode = GetRootNodeWithThreeLevels("Apollon");
            treeView_Directorys.Items.Add(rootNode);

            var item = new AccountWithPermissions(iconUri_user, "Sascha Walzenbach (walzenbach@arges.local)", 2, 2);
            listView_AccountWithPermissions.Items.Add(item);
        }



        #region Directory TreeView
        private MyTreeViewItem GetRootNodeWithThreeLevels(string Servername)
        {
            MyTreeViewItem root = new MyTreeViewItem
            {
                HeaderText = Servername,
                Tag = "\\\\" + Servername.ToLower(),
                Icon = new BitmapImage(iconUri_server)
            };

            List<DirWithSubcount> listOfDirs = GetDirectories(root.Tag.ToString());

            foreach (DirWithSubcount row in listOfDirs)
            {
                MyTreeViewItem item = new MyTreeViewItem
                {
                    HeaderText = row.Dir.Substring(root.Tag.ToString().Length + 1),
                    Tag = row.Dir,
                    Icon = new BitmapImage(iconUri_folderShared)
                };

                List<DirWithSubcount> listOfSubDirs = GetDirectories(row.Dir);

                foreach (var SubRow in listOfSubDirs)
                {
                    MyTreeViewItem subitem = new MyTreeViewItem
                    {
                        HeaderText = SubRow.Dir.Substring(item.Tag.ToString().Length + 1),
                        Tag = SubRow.Dir,
                        Icon = new BitmapImage(iconUri_folder)
                    };
                    subitem.Expanding += new RoutedEventHandler(TreeViewBeforeExpand);

                    if (SubRow.Subcount > 0)
                    {
                        MyTreeViewItem subsubitem = new MyTreeViewItem
                        {
                            Header = "..."
                        };
                        subitem.Items.Add(subsubitem);
                    }

                    item.Items.Add(subitem);
                }

                root.Items.Add(item);
            }
            return root;
        }

        private List<DirWithSubcount> GetDirectories(string v)
        {
            int level = v.Substring(2).Split('\\').Count();
            DataClassesDataContext db = new DataClassesDataContext();
            var result = from dir in db.dirs
                         where
                           dir.Directory.ToLower().StartsWith(v.ToLower()) &&
                           dir.Directory.Substring(2).Length - dir.Directory.Substring(2).Replace("\\", "").Length == level
                         group dir by dir.Directory into gdirs
                         orderby gdirs.Key
                         select new
                         {
                             dir = gdirs.Key
                         };


            List<DirWithSubcount> listOfDirs = new List<DirWithSubcount>();
            foreach (var row in result)
            {
                var subdirs = from Argarm_Dirs in
                                (from Argarm_Dirs in db.dirs
                                 where Argarm_Dirs.Directory.StartsWith(row.dir + "\\")
                                 select new
                                 {
                                     Dummy = "x"
                                 })
                              group Argarm_Dirs by new { Argarm_Dirs.Dummy } into g
                              select new
                              {
                                  Column1 = g.Count()
                              };

                DirWithSubcount entry = new DirWithSubcount(row.dir, subdirs.Count());
                listOfDirs.Add(entry);
            }
            return listOfDirs;
        }

        private void TreeViewBeforeExpand(object sender, RoutedEventArgs args)
        {
            MyTreeViewItem e = (MyTreeViewItem)args.Source;

            if (e.Items.Count > 0)
            {
                MyTreeViewItem node0 = (MyTreeViewItem)e.Items[0];
                if (node0.Header.ToString() == "..." && node0.Tag == null)
                {
                    e.Items.Clear();

                    //get the list of sub directories
                    List<DirWithSubcount> dirs = GetDirectories(e.Tag.ToString());

                    foreach (DirWithSubcount dir in dirs)
                    {
                        MyTreeViewItem node = new MyTreeViewItem
                        {
                            HeaderText = dir.Dir.Substring(e.Tag.ToString().Length + 1),
                            //keep the directory's full path in the tag for use later
                            Tag = dir.Dir,
                            Icon = new BitmapImage(iconUri_folder)
                        };

                        node.Expanding += new RoutedEventHandler(TreeViewBeforeExpand);

                        //if (getDirectories(dir).Count() > 0)
                        if (dir.Subcount > 0)
                        {
                            MyTreeViewItem subnode = new MyTreeViewItem
                            {
                                Header = "..."
                            };
                            node.Items.Add(subnode);
                        }

                        e.Items.Add(node);
                    }
                }
            }
        }
        #endregion Directory TreeView

    }

    class DirWithSubcount
    {
        public string Dir { get; set; }
        public int Subcount { get; set; }

        public DirWithSubcount(string dir, int subcount)
        {
            this.Dir = dir;
            this.Subcount = subcount;
        }
    }

    class AccountWithPermissions
    {
        public Uri IconPath { get; set; }
        public string Name { get; set; }
        public int HowOftenGranted { get; set; }
        public int Inheritance { get; set; }

        public AccountWithPermissions(Uri iconPath, string name, int howOftenGranted, int inheritance)
        {
            this.IconPath = iconPath;
            this.Name = name;
            this.HowOftenGranted = howOftenGranted;
            this.Inheritance = inheritance;
        }
    }
}
