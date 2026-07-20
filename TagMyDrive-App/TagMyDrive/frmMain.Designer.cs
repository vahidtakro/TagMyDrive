namespace TagMyDrive
{
    partial class frmMain
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelMembership = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.disksMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDiskManager = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRefreshDisks = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuGoogleDrive = new System.Windows.Forms.ToolStripMenuItem();
            this.accountMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMembership = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLogout = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtLinkRoot = new System.Windows.Forms.TextBox();
            this.chkLinkFiles = new System.Windows.Forms.CheckBox();
            this.chkHidden = new System.Windows.Forms.CheckBox();
            this.chkSystem = new System.Windows.Forms.CheckBox();
            this.cmdCreate = new System.Windows.Forms.Button();
            this.txtRoot = new System.Windows.Forms.TextBox();
            this.labelRootFolder = new System.Windows.Forms.Label();
            this.cmdBrowse = new System.Windows.Forms.Button();
            this.tabPageDisks = new System.Windows.Forms.TabPage();
            this.lstDisks = new System.Windows.Forms.ListBox();
            this.btnAddDisk = new System.Windows.Forms.Button();
            this.btnDeleteDisk = new System.Windows.Forms.Button();
            this.btnManageQRCodes = new System.Windows.Forms.Button();
            this.btnSnapshotDisk = new System.Windows.Forms.Button();
            this.labelDisksTitle = new System.Windows.Forms.Label();
            this.picDiskQR = new System.Windows.Forms.PictureBox();
            this.lblDiskQRLabel = new System.Windows.Forms.Label();
            this.lblDiskGDriveLabel = new System.Windows.Forms.Label();
            this.lnkDiskGDrive = new System.Windows.Forms.LinkLabel();
            this.lblDiskInfo = new System.Windows.Forms.Label();
            this.tabPageAbout = new System.Windows.Forms.TabPage();
            this.labelAboutSoftware = new System.Windows.Forms.Label();
            this.labelAboutVersion = new System.Windows.Forms.Label();
            this.labelAboutTitle = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPageDisks.SuspendLayout();
            this.tabPageAbout.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabelMembership});
            this.statusStrip1.Location = new System.Drawing.Point(0, 431);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(534, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 3;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(162, 17);
            this.toolStripStatusLabel1.Text = "Select a root folder to begin...";
            // 
            // toolStripStatusLabelMembership
            // 
            this.toolStripStatusLabelMembership.Name = "toolStripStatusLabelMembership";
            this.toolStripStatusLabelMembership.Size = new System.Drawing.Size(90, 17);
            this.toolStripStatusLabelMembership.Text = "Membership: Free";
            this.toolStripStatusLabelMembership.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.disksMenu,
            this.toolsMenu,
            this.accountMenu,
            this.helpMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(534, 24);
            this.menuStrip1.TabIndex = 4;
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuExit});
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(37, 20);
            this.fileMenu.Text = "&File";
            // 
            // menuExit
            // 
            this.menuExit.Name = "menuExit";
            this.menuExit.Size = new System.Drawing.Size(93, 22);
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // disksMenu
            // 
            this.disksMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuDiskManager,
            this.menuRefreshDisks});
            this.disksMenu.Name = "disksMenu";
            this.disksMenu.Size = new System.Drawing.Size(47, 20);
            this.disksMenu.Text = "&Disks";
            // 
            // menuDiskManager
            // 
            this.menuDiskManager.Name = "menuDiskManager";
            this.menuDiskManager.Size = new System.Drawing.Size(180, 22);
            this.menuDiskManager.Text = "&Open Disk Manager";
            this.menuDiskManager.Click += new System.EventHandler(this.menuDiskManager_Click);
            // 
            // menuRefreshDisks
            // 
            this.menuRefreshDisks.Name = "menuRefreshDisks";
            this.menuRefreshDisks.Size = new System.Drawing.Size(180, 22);
            this.menuRefreshDisks.Text = "&Refresh Disk List";
            this.menuRefreshDisks.Click += new System.EventHandler(this.menuRefreshDisks_Click);
            // 
            // toolsMenu
            // 
            this.toolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuGoogleDrive});
            this.toolsMenu.Name = "toolsMenu";
            this.toolsMenu.Size = new System.Drawing.Size(46, 20);
            this.toolsMenu.Text = "&Tools";
            // 
            // menuGoogleDrive
            // 
            this.menuGoogleDrive.Name = "menuGoogleDrive";
            this.menuGoogleDrive.Size = new System.Drawing.Size(180, 22);
            this.menuGoogleDrive.Text = "&Upload to Google Drive";
            this.menuGoogleDrive.Click += new System.EventHandler(this.menuGoogleDrive_Click);
            // 
            // accountMenu
            // 
            this.accountMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMembership,
            this.menuLogout});
            this.accountMenu.Name = "accountMenu";
            this.accountMenu.Size = new System.Drawing.Size(67, 20);
            this.accountMenu.Text = "&Account";
            // 
            // menuMembership
            // 
            this.menuMembership.Name = "menuMembership";
            this.menuMembership.Size = new System.Drawing.Size(180, 22);
            this.menuMembership.Text = "&Membership Info";
            this.menuMembership.Click += new System.EventHandler(this.menuMembership_Click);
            // 
            // menuLogout
            // 
            this.menuLogout.Name = "menuLogout";
            this.menuLogout.Size = new System.Drawing.Size(180, 22);
            this.menuLogout.Text = "&Logout";
            this.menuLogout.Click += new System.EventHandler(this.menuLogout_Click);
            // 
            // helpMenu
            // 
            this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAbout});
            this.helpMenu.Name = "helpMenu";
            this.helpMenu.Size = new System.Drawing.Size(44, 20);
            this.helpMenu.Text = "&Help";
            // 
            // menuAbout
            // 
            this.menuAbout.Name = "menuAbout";
            this.menuAbout.Size = new System.Drawing.Size(107, 22);
            this.menuAbout.Text = "&About";
            this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPageDisks);
            this.tabControl1.Controls.Add(this.tabPageAbout);
            this.tabControl1.Location = new System.Drawing.Point(8, 30);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(518, 395);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.txtTitle);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.txtLinkRoot);
            this.tabPage1.Controls.Add(this.chkLinkFiles);
            this.tabPage1.Controls.Add(this.chkHidden);
            this.tabPage1.Controls.Add(this.chkSystem);
            this.tabPage1.Controls.Add(this.cmdCreate);
            this.tabPage1.Controls.Add(this.txtRoot);
            this.tabPage1.Controls.Add(this.labelRootFolder);
            this.tabPage1.Controls.Add(this.cmdBrowse);
            this.tabPage1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(510, 369);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Snapshot";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Page title:";
            // 
            // txtTitle
            // 
            this.txtTitle.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TagMyDrive.Properties.Settings.Default, "txtTitle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtTitle.Location = new System.Drawing.Point(20, 126);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(470, 20);
            this.txtTitle.TabIndex = 4;
            this.txtTitle.Text = global::TagMyDrive.Properties.Settings.Default.txtTitle;
            this.toolTip1.SetToolTip(this.txtTitle, "Set the html page title");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 159);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Link files:";
            // 
            // txtLinkRoot
            // 
            this.txtLinkRoot.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TagMyDrive.Properties.Settings.Default, "txtLinkRoot", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtLinkRoot.Location = new System.Drawing.Point(20, 198);
            this.txtLinkRoot.Name = "txtLinkRoot";
            this.txtLinkRoot.Size = new System.Drawing.Size(470, 20);
            this.txtLinkRoot.TabIndex = 6;
            this.txtLinkRoot.Text = global::TagMyDrive.Properties.Settings.Default.txtLinkRoot;
            this.toolTip1.SetToolTip(this.txtLinkRoot, "This is the target files will be linked to.");
            // 
            // chkLinkFiles
            // 
            this.chkLinkFiles.AutoSize = true;
            this.chkLinkFiles.Checked = global::TagMyDrive.Properties.Settings.Default.chkLinkFiles;
            this.chkLinkFiles.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::TagMyDrive.Properties.Settings.Default, "chkLinkFiles", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkLinkFiles.Location = new System.Drawing.Point(20, 177);
            this.chkLinkFiles.Name = "chkLinkFiles";
            this.chkLinkFiles.Size = new System.Drawing.Size(59, 17);
            this.chkLinkFiles.TabIndex = 5;
            this.chkLinkFiles.Text = "Enable";
            this.toolTip1.SetToolTip(this.chkLinkFiles, "Files can be linked so you can open them from within the html document");
            this.chkLinkFiles.UseVisualStyleBackColor = true;
            this.chkLinkFiles.CheckedChanged += new System.EventHandler(this.chkLinkFiles_CheckedChanged);
            // 
            // chkHidden
            // 
            this.chkHidden.AutoSize = true;
            this.chkHidden.Checked = global::TagMyDrive.Properties.Settings.Default.chkHidden;
            this.chkHidden.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::TagMyDrive.Properties.Settings.Default, "chkHidden", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkHidden.Location = new System.Drawing.Point(20, 56);
            this.chkHidden.Name = "chkHidden";
            this.chkHidden.Size = new System.Drawing.Size(123, 17);
            this.chkHidden.TabIndex = 2;
            this.chkHidden.Text = "Include hidden items";
            this.toolTip1.SetToolTip(this.chkHidden, "This applies to both files and folders");
            this.chkHidden.UseVisualStyleBackColor = true;
            // 
            // chkSystem
            // 
            this.chkSystem.AutoSize = true;
            this.chkSystem.Checked = global::TagMyDrive.Properties.Settings.Default.chkSystem;
            this.chkSystem.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::TagMyDrive.Properties.Settings.Default, "chkSystem", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkSystem.Location = new System.Drawing.Point(20, 79);
            this.chkSystem.Name = "chkSystem";
            this.chkSystem.Size = new System.Drawing.Size(123, 17);
            this.chkSystem.TabIndex = 3;
            this.chkSystem.Text = "Include system items";
            this.toolTip1.SetToolTip(this.chkSystem, "This applies to both files and folders");
            this.chkSystem.UseVisualStyleBackColor = true;
            // 
            // cmdCreate
            // 
            this.cmdCreate.Enabled = false;
            this.cmdCreate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cmdCreate.Image = ((System.Drawing.Image)(resources.GetObject("cmdCreate.Image")));
            this.cmdCreate.Location = new System.Drawing.Point(120, 280);
            this.cmdCreate.Name = "cmdCreate";
            this.cmdCreate.Size = new System.Drawing.Size(170, 40);
            this.cmdCreate.TabIndex = 7;
            this.cmdCreate.Text = " Create Snapshot";
            this.cmdCreate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdCreate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdCreate.UseVisualStyleBackColor = true;
            this.cmdCreate.Click += new System.EventHandler(this.cmdCreate_Click);
            // 
            // txtRoot
            // 
            this.txtRoot.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TagMyDrive.Properties.Settings.Default, "txtRoot", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtRoot.Location = new System.Drawing.Point(20, 30);
            this.txtRoot.Name = "txtRoot";
            this.txtRoot.Size = new System.Drawing.Size(440, 20);
            this.txtRoot.TabIndex = 0;
            this.txtRoot.Text = global::TagMyDrive.Properties.Settings.Default.txtRoot;
            this.txtRoot.Leave += new System.EventHandler(this.txtRoot_Leave);
            // 
            // labelRootFolder
            // 
            this.labelRootFolder.AutoSize = true;
            this.labelRootFolder.Location = new System.Drawing.Point(6, 12);
            this.labelRootFolder.Name = "labelRootFolder";
            this.labelRootFolder.Size = new System.Drawing.Size(62, 13);
            this.labelRootFolder.TabIndex = 1;
            this.labelRootFolder.Text = "Root folder:";
            // 
            // cmdBrowse
            // 
            this.cmdBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cmdBrowse.Image = ((System.Drawing.Image)(resources.GetObject("cmdBrowse.Image")));
            this.cmdBrowse.Location = new System.Drawing.Point(466, 25);
            this.cmdBrowse.Name = "cmdBrowse";
            this.cmdBrowse.Padding = new System.Windows.Forms.Padding(2);
            this.cmdBrowse.Size = new System.Drawing.Size(28, 28);
            this.cmdBrowse.TabIndex = 1;
            this.toolTip1.SetToolTip(this.cmdBrowse, "Browse for root folder");
            this.cmdBrowse.UseVisualStyleBackColor = true;
            this.cmdBrowse.Click += new System.EventHandler(this.cmdBrowse_Click);
            // 
            // tabPageDisks
            // 
            this.tabPageDisks.Controls.Add(this.lstDisks);
            this.tabPageDisks.Controls.Add(this.btnAddDisk);
            this.tabPageDisks.Controls.Add(this.btnDeleteDisk);
            this.tabPageDisks.Controls.Add(this.btnManageQRCodes);
            this.tabPageDisks.Controls.Add(this.btnSnapshotDisk);
            this.tabPageDisks.Controls.Add(this.labelDisksTitle);
            this.tabPageDisks.Controls.Add(this.picDiskQR);
            this.tabPageDisks.Controls.Add(this.lblDiskQRLabel);
            this.tabPageDisks.Controls.Add(this.lblDiskGDriveLabel);
            this.tabPageDisks.Controls.Add(this.lnkDiskGDrive);
            this.tabPageDisks.Controls.Add(this.lblDiskInfo);
            this.tabPageDisks.Location = new System.Drawing.Point(4, 22);
            this.tabPageDisks.Name = "tabPageDisks";
            this.tabPageDisks.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDisks.Size = new System.Drawing.Size(510, 369);
            this.tabPageDisks.TabIndex = 1;
            this.tabPageDisks.Text = "Disks";
            this.tabPageDisks.UseVisualStyleBackColor = true;
            // 
            // labelDisksTitle
            // 
            this.labelDisksTitle.AutoSize = true;
            this.labelDisksTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.labelDisksTitle.Location = new System.Drawing.Point(10, 10);
            this.labelDisksTitle.Name = "labelDisksTitle";
            this.labelDisksTitle.Size = new System.Drawing.Size(150, 16);
            this.labelDisksTitle.TabIndex = 0;
            this.labelDisksTitle.Text = "Your Registered Disks";
            // 
            // lstDisks
            // 
            this.lstDisks.FormattingEnabled = true;
            this.lstDisks.Location = new System.Drawing.Point(10, 35);
            this.lstDisks.Name = "lstDisks";
            this.lstDisks.Size = new System.Drawing.Size(250, 264);
            this.lstDisks.TabIndex = 1;
            this.lstDisks.SelectedIndexChanged += new System.EventHandler(this.lstDisks_SelectedIndexChanged);
            this.lstDisks.DoubleClick += new System.EventHandler(this.lstDisks_DoubleClick);
            // 
            // btnAddDisk
            // 
            this.btnAddDisk.Location = new System.Drawing.Point(10, 315);
            this.btnAddDisk.Name = "btnAddDisk";
            this.btnAddDisk.Size = new System.Drawing.Size(100, 35);
            this.btnAddDisk.TabIndex = 2;
            this.btnAddDisk.Text = "Add Disk";
            this.btnAddDisk.UseVisualStyleBackColor = true;
            this.btnAddDisk.Click += new System.EventHandler(this.btnAddDisk_Click);
            // 
            // btnDeleteDisk
            // 
            this.btnDeleteDisk.Location = new System.Drawing.Point(120, 315);
            this.btnDeleteDisk.Name = "btnDeleteDisk";
            this.btnDeleteDisk.Size = new System.Drawing.Size(100, 35);
            this.btnDeleteDisk.TabIndex = 3;
            this.btnDeleteDisk.Text = "Delete Disk";
            this.btnDeleteDisk.UseVisualStyleBackColor = true;
            this.btnDeleteDisk.Click += new System.EventHandler(this.btnDeleteDisk_Click);
            // 
            // btnManageQRCodes
            // 
            this.btnManageQRCodes.Location = new System.Drawing.Point(230, 315);
            this.btnManageQRCodes.Name = "btnManageQRCodes";
            this.btnManageQRCodes.Size = new System.Drawing.Size(120, 35);
            this.btnManageQRCodes.TabIndex = 4;
            this.btnManageQRCodes.Text = "QR Codes";
            this.btnManageQRCodes.UseVisualStyleBackColor = true;
            this.btnManageQRCodes.Click += new System.EventHandler(this.btnManageQRCodes_Click);
            // 
            // btnSnapshotDisk
            // 
            this.btnSnapshotDisk.Location = new System.Drawing.Point(360, 315);
            this.btnSnapshotDisk.Name = "btnSnapshotDisk";
            this.btnSnapshotDisk.Size = new System.Drawing.Size(140, 35);
            this.btnSnapshotDisk.TabIndex = 5;
            this.btnSnapshotDisk.Text = "Snapshot Disk Path";
            this.btnSnapshotDisk.UseVisualStyleBackColor = true;
            this.btnSnapshotDisk.Click += new System.EventHandler(this.btnSnapshotDisk_Click);
            // 
            // picDiskQR
            // 
            this.picDiskQR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picDiskQR.Location = new System.Drawing.Point(275, 35);
            this.picDiskQR.Name = "picDiskQR";
            this.picDiskQR.Size = new System.Drawing.Size(130, 130);
            this.picDiskQR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picDiskQR.TabStop = false;
            // 
            // lblDiskQRLabel
            // 
            this.lblDiskQRLabel.AutoSize = true;
            this.lblDiskQRLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblDiskQRLabel.Location = new System.Drawing.Point(415, 35);
            this.lblDiskQRLabel.Name = "lblDiskQRLabel";
            this.lblDiskQRLabel.Size = new System.Drawing.Size(80, 13);
            this.lblDiskQRLabel.Text = "QR Code";
            // 
            // lblDiskInfo
            // 
            this.lblDiskInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblDiskInfo.ForeColor = System.Drawing.Color.Gray;
            this.lblDiskInfo.Location = new System.Drawing.Point(415, 55);
            this.lblDiskInfo.Name = "lblDiskInfo";
            this.lblDiskInfo.Size = new System.Drawing.Size(85, 60);
            this.lblDiskInfo.Text = "";
            // 
            // lblDiskGDriveLabel
            // 
            this.lblDiskGDriveLabel.AutoSize = true;
            this.lblDiskGDriveLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblDiskGDriveLabel.Location = new System.Drawing.Point(275, 175);
            this.lblDiskGDriveLabel.Name = "lblDiskGDriveLabel";
            this.lblDiskGDriveLabel.Size = new System.Drawing.Size(120, 13);
            this.lblDiskGDriveLabel.Text = "Google Drive Link:";
            // 
            // lnkDiskGDrive
            // 
            this.lnkDiskGDrive.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lnkDiskGDrive.Location = new System.Drawing.Point(275, 195);
            this.lnkDiskGDrive.Name = "lnkDiskGDrive";
            this.lnkDiskGDrive.Size = new System.Drawing.Size(225, 30);
            this.lnkDiskGDrive.Text = "(Select a disk)";
            this.lnkDiskGDrive.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkDiskGDrive_LinkClicked);
            // 
            // tabPageAbout
            // 
            this.tabPageAbout.Controls.Add(this.labelAboutSoftware);
            this.tabPageAbout.Controls.Add(this.labelAboutVersion);
            this.tabPageAbout.Controls.Add(this.labelAboutTitle);
            this.tabPageAbout.Location = new System.Drawing.Point(4, 22);
            this.tabPageAbout.Name = "tabPageAbout";
            this.tabPageAbout.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAbout.Size = new System.Drawing.Size(510, 369);
            this.tabPageAbout.TabIndex = 2;
            this.tabPageAbout.Text = "About";
            this.tabPageAbout.UseVisualStyleBackColor = true;
            // 
            // labelAboutSoftware
            // 
            this.labelAboutSoftware.AutoSize = true;
            this.labelAboutSoftware.Location = new System.Drawing.Point(20, 60);
            this.labelAboutSoftware.Name = "labelAboutSoftware";
            this.labelAboutSoftware.Size = new System.Drawing.Size(180, 13);
            this.labelAboutSoftware.TabIndex = 2;
            this.labelAboutSoftware.Text = "Multi-user disk snapshot platform";
            // 
            // labelAboutVersion
            // 
            this.labelAboutVersion.AutoSize = true;
            this.labelAboutVersion.Location = new System.Drawing.Point(20, 40);
            this.labelAboutVersion.Name = "labelAboutVersion";
            this.labelAboutVersion.Size = new System.Drawing.Size(41, 13);
            this.labelAboutVersion.TabIndex = 1;
            this.labelAboutVersion.Text = "version";
            // 
            // labelAboutTitle
            // 
            this.labelAboutTitle.AutoSize = true;
            this.labelAboutTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.labelAboutTitle.Location = new System.Drawing.Point(18, 8);
            this.labelAboutTitle.Name = "labelAboutTitle";
            this.labelAboutTitle.Size = new System.Drawing.Size(180, 29);
            this.labelAboutTitle.TabIndex = 0;
            this.labelAboutTitle.Text = "TagMyDrive";
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // frmMain
            // 
            this.AcceptButton = this.cmdCreate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 453);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TagMyDrive";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyUp);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPageDisks.ResumeLayout(false);
            this.tabPageDisks.PerformLayout();
            this.tabPageAbout.ResumeLayout(false);
            this.tabPageAbout.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelMembership;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.ToolStripMenuItem disksMenu;
        private System.Windows.Forms.ToolStripMenuItem menuDiskManager;
        private System.Windows.Forms.ToolStripMenuItem menuRefreshDisks;
        private System.Windows.Forms.ToolStripMenuItem toolsMenu;
        private System.Windows.Forms.ToolStripMenuItem menuGoogleDrive;
        private System.Windows.Forms.ToolStripMenuItem accountMenu;
        private System.Windows.Forms.ToolStripMenuItem menuMembership;
        private System.Windows.Forms.ToolStripMenuItem menuLogout;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.Windows.Forms.ToolStripMenuItem menuAbout;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox txtRoot;
        private System.Windows.Forms.Label labelRootFolder;
        private System.Windows.Forms.Button cmdBrowse;
        private System.Windows.Forms.Button cmdCreate;
        private System.Windows.Forms.TabPage tabPageDisks;
        private System.Windows.Forms.ListBox lstDisks;
        private System.Windows.Forms.Label labelDisksTitle;
        private System.Windows.Forms.Button btnAddDisk;
        private System.Windows.Forms.Button btnDeleteDisk;
        private System.Windows.Forms.Button btnManageQRCodes;
        private System.Windows.Forms.Button btnSnapshotDisk;
        private System.Windows.Forms.PictureBox picDiskQR;
        private System.Windows.Forms.Label lblDiskQRLabel;
        private System.Windows.Forms.Label lblDiskInfo;
        private System.Windows.Forms.Label lblDiskGDriveLabel;
        private System.Windows.Forms.LinkLabel lnkDiskGDrive;
        private System.Windows.Forms.TabPage tabPageAbout;
        private System.Windows.Forms.Label labelAboutSoftware;
        private System.Windows.Forms.Label labelAboutVersion;
        private System.Windows.Forms.Label labelAboutTitle;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtLinkRoot;
        private System.Windows.Forms.CheckBox chkLinkFiles;
        private System.Windows.Forms.CheckBox chkHidden;
        private System.Windows.Forms.CheckBox chkSystem;
    }
}
