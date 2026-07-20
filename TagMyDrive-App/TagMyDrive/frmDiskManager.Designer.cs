namespace TagMyDrive
{
    partial class frmDiskManager
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

        private void InitializeComponent()
        {
            this.lstDisks = new System.Windows.Forms.ListBox();
            this.labelDisksTitle = new System.Windows.Forms.Label();
            this.panelDetails = new System.Windows.Forms.Panel();
            this.picDiskImage = new System.Windows.Forms.PictureBox();
            this.lblImagePlaceholder = new System.Windows.Forms.Label();
            this.lblDiskName = new System.Windows.Forms.Label();
            this.lblDiskType = new System.Windows.Forms.Label();
            this.lblDiskPath = new System.Windows.Forms.Label();
            this.lblDiskCreated = new System.Windows.Forms.Label();
            this.lblDiskUpdated = new System.Windows.Forms.Label();
            this.txtEditName = new System.Windows.Forms.TextBox();
            this.txtEditDescription = new System.Windows.Forms.TextBox();
            this.lblEditNameLabel = new System.Windows.Forms.Label();
            this.lblEditDescLabel = new System.Windows.Forms.Label();
            this.btnEditSave = new System.Windows.Forms.Button();
            this.btnEditCancel = new System.Windows.Forms.Button();
            this.btnChangeImage = new System.Windows.Forms.Button();
            this.btnRemoveImage = new System.Windows.Forms.Button();
            this.btnEditDisk = new System.Windows.Forms.Button();
            this.lblLinksTitle = new System.Windows.Forms.Label();
            this.lstLinks = new System.Windows.Forms.ListBox();
            this.lblQRTitle = new System.Windows.Forms.Label();
            this.lstQRCodes = new System.Windows.Forms.ListBox();
            this.btnOpenGDriveLink = new System.Windows.Forms.Button();
            this.btnCopyGDriveLink = new System.Windows.Forms.Button();
            this.btnViewQR = new System.Windows.Forms.Button();
            this.btnDownloadQR = new System.Windows.Forms.Button();
            this.btnDeleteDisk = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picDiskImage)).BeginInit();
            this.SuspendLayout();

            // labelDisksTitle
            this.labelDisksTitle.AutoSize = true;
            this.labelDisksTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.labelDisksTitle.Location = new System.Drawing.Point(12, 12);
            this.labelDisksTitle.Text = "My Disks";

            // lstDisks
            this.lstDisks.FormattingEnabled = true;
            this.lstDisks.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lstDisks.Location = new System.Drawing.Point(12, 38);
            this.lstDisks.Size = new System.Drawing.Size(250, 470);
            this.lstDisks.SelectedIndexChanged += new System.EventHandler(this.lstDisks_SelectedIndexChanged);

            // panelDetails
            this.panelDetails.Controls.Add(this.picDiskImage);
            this.panelDetails.Controls.Add(this.lblImagePlaceholder);
            this.panelDetails.Controls.Add(this.lblDiskName);
            this.panelDetails.Controls.Add(this.lblDiskType);
            this.panelDetails.Controls.Add(this.txtEditName);
            this.panelDetails.Controls.Add(this.txtEditDescription);
            this.panelDetails.Controls.Add(this.lblEditNameLabel);
            this.panelDetails.Controls.Add(this.lblEditDescLabel);
            this.panelDetails.Controls.Add(this.btnEditSave);
            this.panelDetails.Controls.Add(this.btnEditCancel);
            this.panelDetails.Controls.Add(this.btnChangeImage);
            this.panelDetails.Controls.Add(this.btnRemoveImage);
            this.panelDetails.Controls.Add(this.lblDiskPath);
            this.panelDetails.Controls.Add(this.lblDiskCreated);
            this.panelDetails.Controls.Add(this.lblDiskUpdated);
            this.panelDetails.Controls.Add(this.lblLinksTitle);
            this.panelDetails.Controls.Add(this.lstLinks);
            this.panelDetails.Controls.Add(this.btnOpenGDriveLink);
            this.panelDetails.Controls.Add(this.btnCopyGDriveLink);
            this.panelDetails.Controls.Add(this.lblQRTitle);
            this.panelDetails.Controls.Add(this.lstQRCodes);
            this.panelDetails.Controls.Add(this.btnViewQR);
            this.panelDetails.Controls.Add(this.btnDownloadQR);
            this.panelDetails.Location = new System.Drawing.Point(275, 12);
            this.panelDetails.Size = new System.Drawing.Size(545, 470);
            this.panelDetails.Visible = false;

            // picDiskImage
            this.picDiskImage.Location = new System.Drawing.Point(5, 5);
            this.picDiskImage.Size = new System.Drawing.Size(100, 100);
            this.picDiskImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picDiskImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picDiskImage.BackColor = System.Drawing.Color.White;

            // lblImagePlaceholder
            this.lblImagePlaceholder.AutoSize = true;
            this.lblImagePlaceholder.ForeColor = System.Drawing.Color.Gray;
            this.lblImagePlaceholder.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblImagePlaceholder.Location = new System.Drawing.Point(20, 42);
            this.lblImagePlaceholder.Text = "No image";

            // btnChangeImage
            this.btnChangeImage.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            this.btnChangeImage.Location = new System.Drawing.Point(5, 108);
            this.btnChangeImage.Size = new System.Drawing.Size(48, 22);
            this.btnChangeImage.Text = "Image";
            this.btnChangeImage.Visible = false;
            this.btnChangeImage.Click += new System.EventHandler(this.btnChangeImage_Click);

            // btnRemoveImage
            this.btnRemoveImage.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            this.btnRemoveImage.Location = new System.Drawing.Point(57, 108);
            this.btnRemoveImage.Size = new System.Drawing.Size(48, 22);
            this.btnRemoveImage.Text = "Remove";
            this.btnRemoveImage.Visible = false;
            this.btnRemoveImage.Click += new System.EventHandler(this.btnRemoveImage_Click);

            // lblDiskName
            this.lblDiskName.AutoSize = true;
            this.lblDiskName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblDiskName.Location = new System.Drawing.Point(112, 5);

            // lblDiskType
            this.lblDiskType.AutoSize = true;
            this.lblDiskType.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblDiskType.ForeColor = System.Drawing.Color.Gray;
            this.lblDiskType.Location = new System.Drawing.Point(112, 28);

            // Edit controls (hidden by default)
            // lblEditNameLabel
            this.lblEditNameLabel.AutoSize = true;
            this.lblEditNameLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblEditNameLabel.Location = new System.Drawing.Point(112, 8);
            this.lblEditNameLabel.Text = "Name:";
            this.lblEditNameLabel.Visible = false;

            // txtEditName
            this.txtEditName.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.txtEditName.Location = new System.Drawing.Point(145, 5);
            this.txtEditName.Size = new System.Drawing.Size(250, 25);
            this.txtEditName.Visible = false;

            // lblEditDescLabel
            this.lblEditDescLabel.AutoSize = true;
            this.lblEditDescLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblEditDescLabel.Location = new System.Drawing.Point(112, 35);
            this.lblEditDescLabel.Text = "Desc:";
            this.lblEditDescLabel.Visible = false;

            // txtEditDescription
            this.txtEditDescription.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.txtEditDescription.Location = new System.Drawing.Point(145, 32);
            this.txtEditDescription.Size = new System.Drawing.Size(250, 23);
            this.txtEditDescription.Visible = false;

            // btnEditSave
            this.btnEditSave.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnEditSave.Location = new System.Drawing.Point(112, 60);
            this.btnEditSave.Size = new System.Drawing.Size(75, 25);
            this.btnEditSave.Text = "Save";
            this.btnEditSave.Visible = false;
            this.btnEditSave.Click += new System.EventHandler(this.btnEditSave_Click);

            // btnEditCancel
            this.btnEditCancel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnEditCancel.Location = new System.Drawing.Point(192, 60);
            this.btnEditCancel.Size = new System.Drawing.Size(75, 25);
            this.btnEditCancel.Text = "Cancel";
            this.btnEditCancel.Visible = false;
            this.btnEditCancel.Click += new System.EventHandler(this.btnEditCancel_Click);

            // lblDiskPath
            this.lblDiskPath.AutoSize = true;
            this.lblDiskPath.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblDiskPath.ForeColor = System.Drawing.Color.Gray;
            this.lblDiskPath.Location = new System.Drawing.Point(112, 50);
            this.lblDiskPath.MaximumSize = new System.Drawing.Size(420, 0);

            // lblDiskCreated
            this.lblDiskCreated.AutoSize = true;
            this.lblDiskCreated.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblDiskCreated.ForeColor = System.Drawing.Color.Gray;
            this.lblDiskCreated.Location = new System.Drawing.Point(112, 72);

            // lblDiskUpdated
            this.lblDiskUpdated.AutoSize = true;
            this.lblDiskUpdated.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblDiskUpdated.ForeColor = System.Drawing.Color.Gray;
            this.lblDiskUpdated.Location = new System.Drawing.Point(112, 88);

            // btnEditDisk
            this.btnEditDisk.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnEditDisk.Location = new System.Drawing.Point(400, 5);
            this.btnEditDisk.Size = new System.Drawing.Size(75, 25);
            this.btnEditDisk.Text = "Edit Disk";
            this.btnEditDisk.Click += new System.EventHandler(this.btnEditDisk_Click);

            // lblLinksTitle
            this.lblLinksTitle.AutoSize = true;
            this.lblLinksTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblLinksTitle.Location = new System.Drawing.Point(5, 140);
            this.lblLinksTitle.Text = "Google Drive Links";

            // lstLinks
            this.lstLinks.FormattingEnabled = true;
            this.lstLinks.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lstLinks.Location = new System.Drawing.Point(5, 160);
            this.lstLinks.Size = new System.Drawing.Size(535, 95);

            // btnOpenGDriveLink
            this.btnOpenGDriveLink.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnOpenGDriveLink.Location = new System.Drawing.Point(5, 260);
            this.btnOpenGDriveLink.Size = new System.Drawing.Size(100, 28);
            this.btnOpenGDriveLink.Text = "Open Link";
            this.btnOpenGDriveLink.Click += new System.EventHandler(this.btnOpenGDriveLink_Click);

            // btnCopyGDriveLink
            this.btnCopyGDriveLink.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnCopyGDriveLink.Location = new System.Drawing.Point(110, 260);
            this.btnCopyGDriveLink.Size = new System.Drawing.Size(100, 28);
            this.btnCopyGDriveLink.Text = "Copy Link";
            this.btnCopyGDriveLink.Click += new System.EventHandler(this.btnCopyGDriveLink_Click);

            // lblQRTitle
            this.lblQRTitle.AutoSize = true;
            this.lblQRTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblQRTitle.Location = new System.Drawing.Point(5, 300);
            this.lblQRTitle.Text = "QR Codes";

            // lstQRCodes
            this.lstQRCodes.FormattingEnabled = true;
            this.lstQRCodes.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lstQRCodes.Location = new System.Drawing.Point(5, 320);
            this.lstQRCodes.Size = new System.Drawing.Size(535, 95);

            // btnViewQR
            this.btnViewQR.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnViewQR.Location = new System.Drawing.Point(5, 420);
            this.btnViewQR.Size = new System.Drawing.Size(100, 28);
            this.btnViewQR.Text = "View QR";
            this.btnViewQR.Click += new System.EventHandler(this.btnViewQR_Click);

            // btnDownloadQR
            this.btnDownloadQR.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnDownloadQR.Location = new System.Drawing.Point(110, 420);
            this.btnDownloadQR.Size = new System.Drawing.Size(100, 28);
            this.btnDownloadQR.Text = "Download QR";
            this.btnDownloadQR.Click += new System.EventHandler(this.btnDownloadQR_Click);

            // btnDeleteDisk
            this.btnDeleteDisk.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnDeleteDisk.Location = new System.Drawing.Point(12, 518);
            this.btnDeleteDisk.Size = new System.Drawing.Size(120, 35);
            this.btnDeleteDisk.Text = "Delete Disk";
            this.btnDeleteDisk.Click += new System.EventHandler(this.btnDeleteDisk_Click);

            // btnClose
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnClose.Location = new System.Drawing.Point(700, 518);
            this.btnClose.Size = new System.Drawing.Size(120, 35);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            // frmDiskManager
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(835, 565);
            this.Controls.Add(this.labelDisksTitle);
            this.Controls.Add(this.lstDisks);
            this.Controls.Add(this.panelDetails);
            this.Controls.Add(this.btnDeleteDisk);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "frmDiskManager";
            this.Text = "Disk Manager - TagMyDrive";
            this.Load += new System.EventHandler(this.frmDiskManager_Load);
            this.panelDetails.ResumeLayout(false);
            this.panelDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picDiskImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label labelDisksTitle;
        private System.Windows.Forms.ListBox lstDisks;
        private System.Windows.Forms.Panel panelDetails;
        private System.Windows.Forms.PictureBox picDiskImage;
        private System.Windows.Forms.Label lblImagePlaceholder;
        private System.Windows.Forms.Label lblDiskName;
        private System.Windows.Forms.Label lblDiskType;
        private System.Windows.Forms.TextBox txtEditName;
        private System.Windows.Forms.TextBox txtEditDescription;
        private System.Windows.Forms.Label lblEditNameLabel;
        private System.Windows.Forms.Label lblEditDescLabel;
        private System.Windows.Forms.Button btnEditSave;
        private System.Windows.Forms.Button btnEditCancel;
        private System.Windows.Forms.Button btnChangeImage;
        private System.Windows.Forms.Button btnRemoveImage;
        private System.Windows.Forms.Button btnEditDisk;
        private System.Windows.Forms.Label lblDiskPath;
        private System.Windows.Forms.Label lblDiskCreated;
        private System.Windows.Forms.Label lblDiskUpdated;
        private System.Windows.Forms.Label lblLinksTitle;
        private System.Windows.Forms.ListBox lstLinks;
        private System.Windows.Forms.Button btnOpenGDriveLink;
        private System.Windows.Forms.Button btnCopyGDriveLink;
        private System.Windows.Forms.Label lblQRTitle;
        private System.Windows.Forms.ListBox lstQRCodes;
        private System.Windows.Forms.Button btnViewQR;
        private System.Windows.Forms.Button btnDownloadQR;
        private System.Windows.Forms.Button btnDeleteDisk;
        private System.Windows.Forms.Button btnClose;
    }
}
