namespace TagMyDrive
{
    partial class frmSnapshotResult
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.picQRCode = new System.Windows.Forms.PictureBox();
            this.lblQRInfo = new System.Windows.Forms.Label();
            this.lblGoogleDriveLabel = new System.Windows.Forms.Label();
            this.lnkGoogleDrive = new System.Windows.Forms.LinkLabel();
            this.btnDownloadQR = new System.Windows.Forms.Button();
            this.btnPrintQR = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picQRCode)).BeginInit();
            this.SuspendLayout();

            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Size = new System.Drawing.Size(300, 30);

            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblStatus.ForeColor = System.Drawing.Color.Green;
            this.lblStatus.Location = new System.Drawing.Point(20, 50);
            this.lblStatus.Size = new System.Drawing.Size(300, 15);

            this.picQRCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picQRCode.Location = new System.Drawing.Point(20, 80);
            this.picQRCode.Name = "picQRCode";
            this.picQRCode.Size = new System.Drawing.Size(200, 200);
            this.picQRCode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picQRCode.TabIndex = 3;

            this.lblQRInfo.AutoSize = true;
            this.lblQRInfo.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblQRInfo.ForeColor = System.Drawing.Color.Gray;
            this.lblQRInfo.Location = new System.Drawing.Point(20, 285);
            this.lblQRInfo.Size = new System.Drawing.Size(200, 13);

            this.btnDownloadQR.Enabled = false;
            this.btnDownloadQR.Location = new System.Drawing.Point(20, 305);
            this.btnDownloadQR.Name = "btnDownloadQR";
            this.btnDownloadQR.Size = new System.Drawing.Size(95, 30);
            this.btnDownloadQR.Text = "Download QR";
            this.btnDownloadQR.UseVisualStyleBackColor = true;
            this.btnDownloadQR.Click += new System.EventHandler(this.btnDownloadQR_Click);

            this.btnPrintQR.Enabled = false;
            this.btnPrintQR.Location = new System.Drawing.Point(125, 305);
            this.btnPrintQR.Name = "btnPrintQR";
            this.btnPrintQR.Size = new System.Drawing.Size(95, 30);
            this.btnPrintQR.Text = "Print QR";
            this.btnPrintQR.UseVisualStyleBackColor = true;
            this.btnPrintQR.Click += new System.EventHandler(this.btnPrintQR_Click);

            this.lblGoogleDriveLabel.AutoSize = true;
            this.lblGoogleDriveLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblGoogleDriveLabel.Location = new System.Drawing.Point(240, 80);
            this.lblGoogleDriveLabel.Size = new System.Drawing.Size(200, 15);
            this.lblGoogleDriveLabel.Text = "Google Drive Link:";

            this.lnkGoogleDrive.AutoSize = true;
            this.lnkGoogleDrive.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lnkGoogleDrive.Location = new System.Drawing.Point(240, 100);
            this.lnkGoogleDrive.Size = new System.Drawing.Size(280, 15);
            this.lnkGoogleDrive.MaximumSize = new System.Drawing.Size(280, 0);
            this.lnkGoogleDrive.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkGoogleDrive_LinkClicked);

            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(240, 305);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(120, 30);
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(540, 350);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.picQRCode);
            this.Controls.Add(this.lblQRInfo);
            this.Controls.Add(this.btnDownloadQR);
            this.Controls.Add(this.btnPrintQR);
            this.Controls.Add(this.lblGoogleDriveLabel);
            this.Controls.Add(this.lnkGoogleDrive);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "frmSnapshotResult";
            this.Text = "Snapshot Created";
            this.Load += new System.EventHandler(this.frmSnapshotResult_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picQRCode)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.PictureBox picQRCode;
        private System.Windows.Forms.Label lblQRInfo;
        private System.Windows.Forms.Label lblGoogleDriveLabel;
        private System.Windows.Forms.LinkLabel lnkGoogleDrive;
        private System.Windows.Forms.Button btnDownloadQR;
        private System.Windows.Forms.Button btnPrintQR;
        private System.Windows.Forms.Button btnClose;
    }
}
