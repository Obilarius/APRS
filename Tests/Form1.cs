using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Windows.Forms;
using Trinet.Networking;

namespace ArgPerm_LiveView
{
    public partial class Form1 : Form
    {
        public int counter = 0;

        public Form1()
        {
            InitializeComponent();

            List<string> servers = new List<string>();
            servers.Add("SAP");
            servers.Add("Apollon");
            servers.Add("Hades");
            servers.Sort();

            foreach (var server in servers)
            {
                ShareCollection shi = ShareCollection.GetShares(server);
                if (shi != null)
                {
                    // Erstelle Hauptnode für Server
                    TreeNode node = new TreeNode(server);

                    // Schleife über alle Freigaben des Servers
                    foreach (Share si in shi)
                    {
                        var last = si.ToString().Substring(si.ToString().Length - 1);
                        if (si.ShareType == ShareType.Disk && last != "$")
                        {
                            TreeNode subNode = new TreeNode(si.NetName);
                            node.SelectedImageIndex = 0;
                            node.ImageIndex = 0;
                            DirectoryInfo dInfo = new DirectoryInfo(si.ToString());
                            subNode.Tag = dInfo;
                            subNode.Nodes.Add("...");

                            node.Nodes.Add(subNode);
                        }

                    }

                    node.SelectedImageIndex = 1;
                    node.ImageIndex = 1;
                    treeView_folder.Nodes.Add(node);
                }
                else
                {
                    Console.WriteLine("Unable to enumerate the shares on {0}.\n"
                        + "Make sure the machine exists, and that you have permission to access it.",
                        server);
                }
            }
        }
      
        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {

            if (e.Node.Nodes.Count > 0)
            {
                if (e.Node.Nodes[0].Text == "..." && e.Node.Nodes[0].Tag == null)
                {
                    e.Node.Nodes.Clear();

                    //get the list of sub direcotires
                    string[] dirs = Directory.GetDirectories(e.Node.Tag.ToString());

                    //add files of rootdirectory
                    //DirectoryInfo rootDir = new DirectoryInfo(e.Node.Tag.ToString());
                    //foreach (var file in rootDir.GetFiles())
                    //{
                    //    TreeNode n = new TreeNode(file.Name, 13, 13);
                    //    n.ImageIndex = 2;
                    //    e.Node.Nodes.Add(n);
                    //}

                    foreach (string dir in dirs)
                    {
                        DirectoryInfo di = new DirectoryInfo(dir);
                        TreeNode node = new TreeNode(di.Name, 0, 1);

                        try
                        {
                            //keep the directory's full path in the tag for use later
                            node.Tag = dir;

                            //if the directory has sub directories add the place holder
                            if (di.GetDirectories().Count() > 0)
                                node.Nodes.Add(null, "...", 0, 0);

                            //foreach (var file in di.GetFiles())
                            //{
                            //    TreeNode n = new TreeNode(file.Name, 13, 13);
                            //    n.ImageIndex = 2;
                            //    node.Nodes.Add(n);
                            //}

                            node.SelectedImageIndex = 0;
                            node.ImageIndex = 0;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            //display a locked folder icon
                            node.ImageIndex = 2;
                            node.SelectedImageIndex = 2;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "DirectoryLister",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            e.Node.Nodes.Add(node);
                        }
                    }
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode CurrentNode = e.Node;
            string fullpath = @"\\" + CurrentNode.FullPath;
            //Debug.WriteLine(fullpath);
            GetDirectorySecurity(fullpath);
        }

        private void GetDirectorySecurity(string dir)
        {
            // Set cursor as hourglass
            Cursor.Current = Cursors.WaitCursor;

            //var repl = dir.Replace("\\\\", "");
            //var split = repl.Split('\\');
            if (dir.Replace("\\\\", "").Split('\\').Length == 1)
                return;

            DirectoryInfo dInfo = new DirectoryInfo(dir);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            AuthorizationRuleCollection acl = dSecurity.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier)); // FÜR SID
            //AuthorizationRuleCollection acl = dSecurity.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));          // FÜR NAMEN (ARGES/walzenbach)

            treeView_rights.Nodes.Clear();

            foreach (FileSystemAccessRule ace in acl)
            {
                var sid = ace.IdentityReference.Value;

                var context = new PrincipalContext(ContextType.Domain, "arges");
                Principal identity = Principal.FindByIdentity(context, sid);

                TreeNode node = (identity == null) ? new TreeNode(sid) : newNode(identity, ace);

                if (node.Text != "")
                {
                    treeView_rights.Nodes.Add(node);
                }
                
                //treeView_rights.ExpandAll();
            }

            // Set cursor as default arrow
            Cursor.Current = Cursors.Default;
        }

        private TreeNode newNode (Principal principal, FileSystemAccessRule ace = null, int fatherImageIndex = -1)
        {
            string sid = principal.Sid.ToString();

            TreeNode node = new TreeNode();

            if (sid == "S-1-5-18")
            {
                var str = "Lokales System";
                str += (ace != null) ? " - " + ace.FileSystemRights.ToString() : "";

                node = new TreeNode(str);
                node.ImageIndex = 8;
                node.SelectedImageIndex = node.ImageIndex;
            }
            else if (principal.StructuralObjectClass == "user")
            {
                var str = (principal.Name == "" || principal.Name == null) ? "Name unbekannt (" + sid + ")" : principal.Name;
                str += (ace != null) ? " - " + ace.FileSystemRights.ToString() : "";
                node = new TreeNode(str);

                if (ace != null)
                {
                    // User Bild für die erste Ebene
                    node.ImageIndex = (ace.AccessControlType == AccessControlType.Allow) ? 0 : 2;
                    node.ImageIndex += (ace.IsInherited) ? 1 : 0;
                    node.SelectedImageIndex = node.ImageIndex;
                }
                else
                {
                    // User Bild ab der zweiten Ebene
                    node.ImageIndex = (fatherImageIndex - 4 < 0 && fatherImageIndex < 8) ? fatherImageIndex : fatherImageIndex - 4;
                    node.SelectedImageIndex = node.ImageIndex;
                }
            }
            else if (principal.StructuralObjectClass == "group")
            {
                var str = (principal.Name == "" || principal.Name == null) ? "Name unbekannt (" + sid + ")" : principal.Name;
                str += (ace != null) ? " - " + ace.FileSystemRights.ToString() : "";
                node = new TreeNode(str);

                if (ace != null)
                {
                    // Gruppen Bild für die erste Ebene
                    node.ImageIndex = (ace.AccessControlType == AccessControlType.Allow) ? 4 : 6;
                    node.ImageIndex += (ace.IsInherited) ? 1 : 0;
                    node.SelectedImageIndex = node.ImageIndex;
                }
                else
                {
                    // Gruppen Bild ab der zweiten Ebene
                    node.ImageIndex = (fatherImageIndex + 4 < 8) ? fatherImageIndex + 4 : fatherImageIndex;
                    node.SelectedImageIndex = node.ImageIndex;
                }

                // Geht die Gruppe Rekursiv durch und gibt die enthaltenen Member aus.
                var group = principal as GroupPrincipal;
                var usersInGroup = group.GetMembers();
                foreach (Principal user in usersInGroup)
                {
                    TreeNode subnode = newNode(user, null, node.ImageIndex);
                    if (subnode.Text != "")
                    {
                        node.Nodes.Add(subnode);
                    }
                }

            }

            return node;
        }
    }
}
