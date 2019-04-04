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
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Text.RegularExpressions;

namespace ArgPerm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public readonly static Uri iconUri_folder = new Uri("pack://siteoforigin:,,,/Resources/win/Folder_16x.png");
        public readonly static Uri iconUri_folderShared = new Uri("pack://siteoforigin:,,,/Resources/win/FolderShared_16x.png");
        public readonly static Uri iconUri_server = new Uri("pack://siteoforigin:,,,/Resources/win/LocalServer_16x.png");

        public readonly static Uri iconUri_user = new Uri("pack://siteoforigin:,,,/Resources/User_16x.png");
        public readonly static Uri iconUri_group = new Uri("pack://siteoforigin:,,,/Resources/Group_16x.png");

        public readonly static Uri iconUri_warning = new Uri("pack://siteoforigin:,,,/Resources/Warning_16x.png");
        public readonly static Uri iconUri_lockOpen = new Uri("pack://siteoforigin:,,,/Resources/LockOpen_16x.png");
        public readonly static Uri iconUri_lockClosed = new Uri("pack://siteoforigin:,,,/Resources/LockClosed_16x.png");

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            MyTreeViewItem rootNode = GetRootAndFirstLevelNodes("Apollon");
            //MyTreeViewItem rootNode = GetRootNode("Apollon");
            treeView_Directorys.Items.Add(rootNode);
            Mouse.OverrideCursor = null;
        }



        #region Directory TreeView
        //private MyTreeViewItem GetRootNode(string Servername)
        //{
        //    // Bildet die root Node für den Server (Level 0)
        //    MyTreeViewItem root = new MyTreeViewItem
        //    {
        //        HeaderText = Servername,
        //        Tag = "\\\\" + Servername.ToLower(),
        //        Icon = new BitmapImage(iconUri_server)
        //    };
        //    root.IsSelected = true;
        //    root.Expanding += new RoutedEventHandler(TreeViewBeforeExpand);

        //    MyTreeViewItem subitem = new MyTreeViewItem
        //    {
        //        Header = "..."
        //    };
        //    root.Items.Add(subitem);

        //    return root;
        //}

        private MyTreeViewItem GetRootAndFirstLevelNodes(string Servername)
        {
            // Bildet die root Node für den Server (Level 0)
            MyTreeViewItem root = new MyTreeViewItem
            {
                HeaderText = Servername,
                Tag = new[] { null, "\\\\" + Servername.ToLower() },
                Icon = new BitmapImage(iconUri_server)
            };
            root.IsSelected = true;

            // Sucht alle Unterordner des Servers und bildet die Nodes dafür (Level 1)
            DataClassesDataContext db = new DataClassesDataContext();

            var result = from dir in db.dirs
                         where
                             dir.ParentID == null && 
                             dir.Directory.ToLower().StartsWith("\\\\" + Servername.ToLower() + "\\")
                         orderby dir.Directory
                         select new
                         {
                             dir.ID,
                             dir.Directory
                         };

            foreach (var row in result)
            {
                MyTreeViewItem node = new MyTreeViewItem
                {
                    HeaderText = Regex.Match(row.Directory, @"\w+$").Value,
                    Tag = new [] { row.ID.ToString(), row.Directory },
                    Icon = new BitmapImage(iconUri_folderShared)
                };
                node.Expanding += new RoutedEventHandler(TreeViewBeforeExpand);

                var subdirs = (from dirs in db.dirs
                               where dirs.ParentID == row.ID
                               select new
                               {
                                   dirs.ID
                               }).Take(1);

                if (subdirs.Count() > 0)
                {
                    MyTreeViewItem subnode = new MyTreeViewItem
                    {
                        Header = "..."
                    };
                    node.Items.Add(subnode);
                }

                root.Items.Add(node);
            }

            return root;
        }

        //private List<DirWithSubcount> GetDirectories(string v)
        //{
        //    int level = v.Substring(2).Split('\\').Count();
        //    DataClassesDataContext db = new DataClassesDataContext();

        //    var result = from dir in db.dirs
        //                 where
        //                   dir.Directory.ToLower().StartsWith(v.ToLower() + "\\") &&
        //                   dir.Directory.Substring(2).Length - dir.Directory.Substring(2).Replace("\\", "").Length == level
        //                 group dir by dir.Directory into gdirs
        //                 orderby gdirs.Key
        //                 select new
        //                 {
        //                     dir = gdirs.Key
        //                 };

        //    List < DirWithSubcount > listOfDirs = new List<DirWithSubcount>();
        //    foreach (var row in result)
        //    {
        //        var subdirs = (from dirs in db.dirs
        //                       where dirs.Directory.StartsWith(row.dir + "\\")
        //                       select new
        //                       {
        //                           dirs.ID
        //                       }).Take(1);


        //        DirWithSubcount entry = new DirWithSubcount(row.dir, subdirs.Count());
        //        listOfDirs.Add(entry);
        //    }
        //    return listOfDirs;
        //}

        private List<DirWithSubcount> GetDirectoriesByParentID(int parentID)
        {
            DataClassesDataContext db = new DataClassesDataContext();
            
            var result = from dir in db.dirs
                        where
                            dir.ParentID == parentID
                        orderby dir.Directory
                        select new
                        {
                            dir.Directory,
                            dir.ID
                        };

            List<DirWithSubcount> listOfDirs = new List<DirWithSubcount>();
            foreach (var row in result)
            {
                var subdirs = (from dirs in db.dirs
                               where dirs.ParentID == row.ID
                               select new
                               {
                                   dirs.ID
                               }).Take(1);


                DirWithSubcount entry = new DirWithSubcount(row.ID, row.Directory, subdirs.Count());
                listOfDirs.Add(entry);
            }
            return listOfDirs;
        }

        private void TreeViewBeforeExpand(object sender, RoutedEventArgs args)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            MyTreeViewItem selectedTVI = (MyTreeViewItem)args.Source;

            if (selectedTVI.Items.Count > 0)
            {
                MyTreeViewItem node0 = (MyTreeViewItem)selectedTVI.Items[0];
                if (node0.Header.ToString() == "..." && node0.Tag == null)
                {
                    selectedTVI.Items.Clear();

                    //get the list of sub directories
                    int parentID = Convert.ToInt32(((string[])selectedTVI.Tag)[0]);
                    List<DirWithSubcount> dirs = GetDirectoriesByParentID(parentID);

                    foreach (DirWithSubcount dir in dirs)
                    {
                        MyTreeViewItem node = new MyTreeViewItem
                        {
                            HeaderText = Regex.Match(dir.Dir, @"([^\\]+)(?=[^\\]*$)").Value,
                            //keep the directory's full path in the tag for use later
                            Tag = new[] { dir.ID.ToString(), dir.Dir },
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

                        selectedTVI.Items.Add(node);
                    }
                }
            }
            Mouse.OverrideCursor = null;
        }

        private void TreeViewSelectedItemChange(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MyTreeViewItem selectedItem = treeView_Directorys.SelectedItem as MyTreeViewItem;
            string fullPath = ((string[])selectedItem.Tag)[1];
            var folderName = fullPath.Split('\\').Last();

            lbl_FolderName.Content = folderName;
            lbl_FolderPath.Content = fullPath;

            DataClassesDataContext db = new DataClassesDataContext();

            #region FolderOwner
            var owner = from d in db.dirs
                         join u in db.adusers on new { d.Owner } equals new { Owner = u.SID } into u_join
                         from u in u_join.DefaultIfEmpty()
                         join g in db.adgroups on new { d.Owner } equals new { Owner = g.SID } into g_join
                         from g in g_join.DefaultIfEmpty()
                         where
                           d.Directory == fullPath
                         select new
                         {
                             Name = (u.DisplayName ?? g.Name),
                             SamAccountName = (u.SamAccountName ?? g.SamAccountName),
                             g.SID
                         };

            if (owner.Any())
            {
                lbl_OwnerName.Content = owner.First().Name;
                img_OwnerIcon.Source = (owner.First().SID != null) ? new BitmapImage(iconUri_group) : new BitmapImage(iconUri_user);
            }
            else
            {
                lbl_OwnerName.Content = "Keinen Besitzer gefunden";
            }
            #endregion

            listView_AccountWithPermissions.Items.Clear();
            var result = from d in db.dirs
                         join r in db.rights on new { d.ID } equals new { ID = r.DirID }
                         join u in db.adusers on new { r.IdentityReference } equals new { IdentityReference = u.SID } into u_join
                         from u in u_join.DefaultIfEmpty()
                         join g in db.adgroups on new { r.IdentityReference } equals new { IdentityReference = g.SID } into g_join
                         from g in g_join.DefaultIfEmpty()
                         where
                           d.Directory == fullPath &&
                           r.InheritanceFlags != "None"
                         select new
                         {
                             r.AccessControlType,
                             r.FileSystemRights,
                             r.IsInherited,
                             r.InheritanceFlags,
                             SID = (u.SID ?? g.SID),
                             SamAccountName = (u.SamAccountName ?? g.SamAccountName),
                             User = u.DisplayName,
                             Group = g.Name
                         };

            List<User> UserList = new List<User>();
            foreach (var row in result)
            {
                // Wenn User dann einfach anzeigen
                if (row.User != null)
                {
                    var name = row.User;
                    name += (row.SamAccountName != null) ? $" ({row.SamAccountName})" : "";

                    UserList.Add(new User(row.AccessControlType, row.FileSystemRights, row.IsInherited, row.InheritanceFlags, row.SID, name, null));
                }
                // Wenn Gruppe dann wird die Funktion GetMemberInGroup aufgerufen die alle User die über diese Gruppe berechtigt sind abgerufen
                else
                {
                    var name = row.Group;
                    name += (row.SamAccountName != null) ? $" ({row.SamAccountName})" : "";

                    List<User> retList = new List<User>();
                    GetMemberInGroup(row.AccessControlType, row.FileSystemRights, row.IsInherited, row.InheritanceFlags, row.SID, name, retList);
                    UserList.AddRange(retList);
                }
            }

            FillAccountWithPermissonsSection(UserList);
        }

        private void GetMemberInGroup(int AccessControlType, string FileSystemRights, int IsInherited, string InheritanceFlags, string SID, string grpName, List<User> list)
        {
            DataClassesDataContext db = new DataClassesDataContext();
            var result = from gu in db.grp_users
                         join u in db.adusers on new { UserSID = gu.userSID } equals new { UserSID = u.SID } into u_join
                         from u in u_join.DefaultIfEmpty()
                         join g in db.adgroups on new { UserSID = gu.userSID } equals new { UserSID = g.SID } into g_join
                         from g in g_join.DefaultIfEmpty()
                         where
                           gu.grpSID == SID
                         select new
                         {
                             SID = (u.SID ?? g.SID),
                             SamAccountName = (u.SamAccountName ?? g.SamAccountName),
                             User = u.DisplayName,
                             Group = g.Name
                         };

            if (!result.Any())
                return;
            
            foreach (var row in result)
            {
                // Wenn User dann einfach zur Liste hinzufügen
                if (row.User != null)
                {
                    string userName = row.User;
                    userName += (row.SamAccountName != null) ? $" ({row.SamAccountName})" : "";

                    list.Add(new User(AccessControlType, FileSystemRights, IsInherited, InheritanceFlags, row.SID, userName, grpName));
                }
                // Falls Gruppe dann Rekursiv die Funktion aufrufen
                else if (row.Group != null)
                {
                    var _grpName = row.Group;
                    _grpName += (row.SamAccountName != null) ? $" ({row.SamAccountName})" : "";

                    GetMemberInGroup(AccessControlType, FileSystemRights, IsInherited, InheritanceFlags, row.SID, _grpName, list);
                }
            }
        }
        #endregion Directory TreeView


        #region Account with Permissions
        void FillAccountWithPermissonsSection(List<User> UserList)
        {
            var ret = from ul in UserList
                      group ul by new
                      {
                          ul.Name,
                          ul.SID
                      } into gul
                      orderby gul.Key.Name
                      select new
                      {
                          gul.Key.SID,
                          gul.Key.Name,
                          Count = gul.Count(),
                          InheritedCount = gul.Sum(p => (p.IsInherited) ? 1 : 0)
                      };

            foreach (var user in ret)
            {
                List<string> groups = (from ul in UserList
                                     where ul.SID == user.SID
                                     select ul.Group
                                     ).ToList();

                var item = new AccountWithPermissions(iconUri_user, user.Name, user.Count, user.InheritedCount, user.SID, groups);
                listView_AccountWithPermissions.Items.Add(item);
            }
        }

        // TODO
        private void ShowAllDirectoriesWithPermissions_Click(object sender, RoutedEventArgs e)
        {
            var user = listView_AccountWithPermissions.SelectedItem as AccountWithPermissions;
            var SID = user.SID;

            DataClassesDataContext db = new DataClassesDataContext();

            var result = from r in db.rights
                         join d in db.dirs on new { ID = r.DirID } equals new { d.ID }
                         where
                               ((from gu in db.grp_users
                                 where gu.userSID == SID
                                 select new
                                 {
                                     SID = gu.grpSID
                                 }).Concat(
                                 from u in db.adusers
                                 where u.SID == SID
                                 select new
                                 {
                                     u.SID
                                 })).Contains(new { SID = r.IdentityReference })
                         orderby
                           d.Directory
                         select new
                         {
                             d.Directory,
                             r.AccessControlType,
                             r.FileSystemRights,
                             r.InheritanceFlags,
                             r.IsInherited
                         };

            var count = result.Count();




            foreach (var item in result)
            {
                //output += String.Format("0, -150", item.Directory) + $" - {item.AccessControlType} - {item.FileSystemRights} - {item.InheritanceFlags} - {item.IsInherited} \n";
            }

            

        }

        private void ListViewItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (!(listView_AccountWithPermissions.SelectedItem is AccountWithPermissions selUser))
            {
                return;
            }

            #region Lösche Gruppen unter dem User
            List<int> toRemove = new List<int>();
            for (int i = listView_AccountWithPermissions.SelectedIndex + 1; i < listView_AccountWithPermissions.Items.Count; i++)
            {
                var checkItem = listView_AccountWithPermissions.Items.GetItemAt(i);
                AccountWithPermissions _item = checkItem as AccountWithPermissions;

                if (_item.IconGroup == null)
                {
                    break;
                }
                else
                {
                    toRemove.Add(i);
                    //listView_AccountWithPermissions.Items.RemoveAt(i);
                }
            }

            bool ifRemoved = false;
            if (toRemove.Count > 0)
            {
                for (int i = toRemove.Last(); i >= toRemove.First(); i--)
                {
                    listView_AccountWithPermissions.Items.RemoveAt(i);
                    ifRemoved = true;
                }
            }
            #endregion

            #region Fügt die Gruppen unter dem User ein
            if (!ifRemoved)
            {
                foreach (var group in selUser.Groups)
                {
                    AccountWithPermissions groupItem = new AccountWithPermissions(iconUri_group, group);
                    listView_AccountWithPermissions.Items.Insert(listView_AccountWithPermissions.SelectedIndex + 1, groupItem);
                }
            }
            #endregion
        }
        #endregion

    }

    class DirWithSubcount
    {
        public int ID { get; set; }
        public string Dir { get; set; }
        public int Subcount { get; set; }

        public DirWithSubcount(int id, string dir, int subcount)
        {
            this.ID = id;
            this.Dir = dir;
            this.Subcount = subcount;
        }
    }

    class AccountWithPermissions
    {
        public Uri IconPath { get; set; }

        public string Name { get; set; }
        public string SID { get; set; }
        public int HowOftenGranted { get; set; }
        public Uri IconWarning { get; }
        public string Inheritance { get; set; }
        public Uri IconInheritance { get; }
        public List<string> Groups { get; set; }
        public Uri IconGroup { get; set; }
        public SolidColorBrush BackgroudColor { get; set; }

        public AccountWithPermissions(Uri iconPath, string name, int howOftenGranted, int inheritance, string sid, List<string> groups)
        {
            this.IconPath = iconPath;
            this.Name = name;
            this.SID = sid;
            this.HowOftenGranted = howOftenGranted;
            this.Groups = groups;

            if (this.HowOftenGranted > 1)
                this.IconWarning = MainWindow.iconUri_warning;
            else
                this.IconWarning = null;

            if (inheritance <= 1)
                this.Inheritance = null;
            else
                this.Inheritance = inheritance.ToString() + "x";

            this.IconInheritance = MainWindow.iconUri_lockOpen;
        }

        public AccountWithPermissions(Uri iconPath, string name)
        {
            this.IconGroup = iconPath;
            this.Name = name;
            this.BackgroudColor = new SolidColorBrush(Colors.AliceBlue);
        }
    }

    class User
    {
        public bool AccessControlType { get; set; }
        public string FileSystemRights { get; set; }
        public bool IsInherited { get; set; }
        public string InheritanceFlags { get; set; }
        public string SID { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }

        public User(int act, string fsr, int inherited, string inheritanceFlags, string sid, string name, string group)
        {
            this.AccessControlType = (act == 1) ? true : false;
            this.FileSystemRights = fsr;
            this.IsInherited = (inherited == 1) ? true : false;
            this.InheritanceFlags = inheritanceFlags;
            this.SID = sid;
            this.Name = name;
            this.Group = group;
        }

        public User()
        {

        }
    }
}
