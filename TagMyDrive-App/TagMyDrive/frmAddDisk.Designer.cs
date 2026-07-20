namespace TagMyDrive
{
    partial class frmAddDisk
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
            this.lblDiskName = new System.Windows.Forms.Label();
            this.txtDiskName = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDiskPath = new System.Windows.Forms.Label();
            this.txtDiskPath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblDiskType = new System.Windows.Forms.Label();
            this.cmbDiskType = new System.Windows.Forms.ComboBox();
            this.picDiskImage = new System.Windows.Forms.PictureBox();
            this.lblImageTitle = new System.Windows.Forms.Label();
            this.lblImageUrl = new System.Windows.Forms.Label();
            this.txtImageUrl = new System.Windows.Forms.TextBox();
            this.btnLoadImage = new System.Windows.Forms.Button();
            this.btnRemoveImage = new System.Windows.Forms.Button();
            this.lblImageStatus = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picDiskImage)).BeginInit();
            this.SuspendLayout();

            // lblDiskName
            this.lblDiskName.AutoSize = true;
            this.lblDiskName.Location = new System.Drawing.Point(20, 15);
            this.lblDiskName.Text = "Disk Name:";

            // txtDiskName
            this.txtDiskName.Location = new System.Drawing.Point(20, 35);
            this.txtDiskName.Size = new System.Drawing.Size(260, 23);

            // lblDescription
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(20, 65);
            this.lblDescription.Text = "Description:";

            // txtDescription
            this.txtDescription.Location = new System.Drawing.Point(20, 85);
            this.txtDescription.Size = new System.Drawing.Size(260, 23);

            // lblDiskPath
            this.lblDiskPath.AutoSize = true;
            this.lblDiskPath.Location = new System.Drawing.Point(20, 115);
            this.lblDiskPath.Text = "Disk Path:";

            // txtDiskPath
            this.txtDiskPath.Location = new System.Drawing.Point(20, 135);
            this.txtDiskPath.ReadOnly = true;
            this.txtDiskPath.Size = new System.Drawing.Size(180, 23);

            // btnBrowse
            this.btnBrowse.Location = new System.Drawing.Point(205, 135);
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);

            // lblDiskType
            this.lblDiskType.AutoSize = true;
            this.lblDiskType.Location = new System.Drawing.Point(20, 165);
            this.lblDiskType.Text = "Disk Type:";

            // cmbDiskType
            this.cmbDiskType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDiskType.Location = new System.Drawing.Point(20, 185);
            this.cmbDiskType.Size = new System.Drawing.Size(260, 23);

            // Image section (right side)
            // lblImageTitle
            this.lblImageTitle.AutoSize = true;
            this.lblImageTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblImageTitle.Location = new System.Drawing.Point(310, 15);
            this.lblImageTitle.Text = "Disk Image";

            // lblImageUrl
            this.lblImageUrl.AutoSize = true;
            this.lblImageUrl.Location = new System.Drawing.Point(310, 38);
            this.lblImageUrl.Text = "Image URL:";

            // txtImageUrl
            this.txtImageUrl.Location = new System.Drawing.Point(310, 56);
            this.txtImageUrl.Size = new System.Drawing.Size(160, 23);

            // btnLoadImage
            this.btnLoadImage.Location = new System.Drawing.Point(310, 84);
            this.btnLoadImage.Size = new System.Drawing.Size(75, 25);
            this.btnLoadImage.Text = "Load";
            this.btnLoadImage.Click += new System.EventHandler(this.btnLoadImage_Click);

            // btnRemoveImage
            this.btnRemoveImage.Location = new System.Drawing.Point(390, 84);
            this.btnRemoveImage.Size = new System.Drawing.Size(80, 25);
            this.btnRemoveImage.Text = "Remove";
            this.btnRemoveImage.Click += new System.EventHandler(this.btnRemoveImage_Click);

            // picDiskImage
            this.picDiskImage.Location = new System.Drawing.Point(310, 116);
            this.picDiskImage.Size = new System.Drawing.Size(160, 120);
            this.picDiskImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picDiskImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picDiskImage.BackColor = System.Drawing.Color.White;

            // lblImageStatus
            this.lblImageStatus.AutoSize = true;
            this.lblImageStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblImageStatus.Location = new System.Drawing.Point(310, 242);
            this.lblImageStatus.Text = "No image";

            // btnAdd
            this.btnAdd.Location = new System.Drawing.Point(140, 290);
            this.btnAdd.Size = new System.Drawing.Size(100, 35);
            this.btnAdd.Text = "Add Disk";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(250, 290);
            this.btnCancel.Size = new System.Drawing.Size(100, 35);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // frmAddDisk
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 340);
            this.Controls.Add(this.lblDiskName);
            this.Controls.Add(this.txtDiskName);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblDiskPath);
            this.Controls.Add(this.txtDiskPath);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.lblDiskType);
            this.Controls.Add(this.cmbDiskType);
            this.Controls.Add(this.lblImageTitle);
            this.Controls.Add(this.lblImageUrl);
            this.Controls.Add(this.txtImageUrl);
            this.Controls.Add(this.btnLoadImage);
            this.Controls.Add(this.btnRemoveImage);
            this.Controls.Add(this.picDiskImage);
            this.Controls.Add(this.lblImageStatus);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "frmAddDisk";
            this.Text = "Add New Disk - TagMyDrive";
            this.Load += new System.EventHandler(this.frmAddDisk_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picDiskImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblDiskName;
        private System.Windows.Forms.TextBox txtDiskName;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDiskPath;
        private System.Windows.Forms.TextBox txtDiskPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label lblDiskType;
        private System.Windows.Forms.ComboBox cmbDiskType;
        private System.Windows.Forms.PictureBox picDiskImage;
        private System.Windows.Forms.Label lblImageTitle;
        private System.Windows.Forms.Label lblImageUrl;
        private System.Windows.Forms.TextBox txtImageUrl;
        private System.Windows.Forms.Button btnLoadImage;
        private System.Windows.Forms.Button btnRemoveImage;
        private System.Windows.Forms.Label lblImageStatus;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnCancel;
    }
}
