namespace ArgPerm_LiveView
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.treeView_folder = new System.Windows.Forms.TreeView();
            this.imageList_icons = new System.Windows.Forms.ImageList(this.components);
            this.treeView_rights = new System.Windows.Forms.TreeView();
            this.imageList_rightsIcons = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // treeView_folder
            // 
            this.treeView_folder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeView_folder.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView_folder.FullRowSelect = true;
            this.treeView_folder.ImageIndex = 0;
            this.treeView_folder.ImageList = this.imageList_icons;
            this.treeView_folder.ItemHeight = 20;
            this.treeView_folder.Location = new System.Drawing.Point(0, 0);
            this.treeView_folder.Name = "treeView_folder";
            this.treeView_folder.SelectedImageIndex = 0;
            this.treeView_folder.ShowLines = false;
            this.treeView_folder.Size = new System.Drawing.Size(297, 694);
            this.treeView_folder.TabIndex = 0;
            this.treeView_folder.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
            this.treeView_folder.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // imageList_icons
            // 
            this.imageList_icons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_icons.ImageStream")));
            this.imageList_icons.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_icons.Images.SetKeyName(0, "folder.png");
            this.imageList_icons.Images.SetKeyName(1, "drive.png");
            this.imageList_icons.Images.SetKeyName(2, "folder_locked.png");
            // 
            // treeView_rights
            // 
            this.treeView_rights.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView_rights.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView_rights.Font = new System.Drawing.Font("Monospac821 BT", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView_rights.ImageIndex = 0;
            this.treeView_rights.ImageList = this.imageList_rightsIcons;
            this.treeView_rights.ItemHeight = 20;
            this.treeView_rights.Location = new System.Drawing.Point(303, 22);
            this.treeView_rights.Margin = new System.Windows.Forms.Padding(30);
            this.treeView_rights.Name = "treeView_rights";
            this.treeView_rights.SelectedImageIndex = 0;
            this.treeView_rights.ShowLines = false;
            this.treeView_rights.Size = new System.Drawing.Size(643, 672);
            this.treeView_rights.TabIndex = 1;
            // 
            // imageList_rightsIcons
            // 
            this.imageList_rightsIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_rightsIcons.ImageStream")));
            this.imageList_rightsIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_rightsIcons.Images.SetKeyName(0, "u_a.png");
            this.imageList_rightsIcons.Images.SetKeyName(1, "u_a_i.png");
            this.imageList_rightsIcons.Images.SetKeyName(2, "u_d.png");
            this.imageList_rightsIcons.Images.SetKeyName(3, "u_d_i.png");
            this.imageList_rightsIcons.Images.SetKeyName(4, "g_a.png");
            this.imageList_rightsIcons.Images.SetKeyName(5, "g_a_i.png");
            this.imageList_rightsIcons.Images.SetKeyName(6, "g_d.png");
            this.imageList_rightsIcons.Images.SetKeyName(7, "g_d_i.png");
            this.imageList_rightsIcons.Images.SetKeyName(8, "System.png");
            this.imageList_rightsIcons.Images.SetKeyName(9, "null.png");
            this.imageList_rightsIcons.Images.SetKeyName(10, "blank.png");
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(945, 693);
            this.Controls.Add(this.treeView_rights);
            this.Controls.Add(this.treeView_folder);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "ARG Perm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView_folder;
        private System.Windows.Forms.ImageList imageList_icons;
        private System.Windows.Forms.TreeView treeView_rights;
        private System.Windows.Forms.ImageList imageList_rightsIcons;
    }
}

