namespace TagMyDrive
{
    partial class frmQRCodeManager
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
            this.lstQRCodes = new System.Windows.Forms.ListBox();
            this.btnGenerateQR = new System.Windows.Forms.Button();
            this.btnDownloadQR = new System.Windows.Forms.Button();
            this.btnDeleteQR = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(130, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "QR Codes";

            // lstQRCodes
            this.lstQRCodes.FormattingEnabled = true;
            this.lstQRCodes.Location = new System.Drawing.Point(20, 50);
            this.lstQRCodes.Name = "lstQRCodes";
            this.lstQRCodes.Size = new System.Drawing.Size(640, 350);
            this.lstQRCodes.TabIndex = 1;

            // btnGenerateQR
            this.btnGenerateQR.Location = new System.Drawing.Point(20, 415);
            this.btnGenerateQR.Name = "btnGenerateQR";
            this.btnGenerateQR.Size = new System.Drawing.Size(120, 30);
            this.btnGenerateQR.TabIndex = 2;
            this.btnGenerateQR.Text = "Generate QR";
            this.btnGenerateQR.UseVisualStyleBackColor = true;
            this.btnGenerateQR.Click += new System.EventHandler(this.btnGenerateQR_Click);

            // btnDownloadQR
            this.btnDownloadQR.Location = new System.Drawing.Point(150, 415);
            this.btnDownloadQR.Name = "btnDownloadQR";
            this.btnDownloadQR.Size = new System.Drawing.Size(120, 30);
            this.btnDownloadQR.TabIndex = 3;
            this.btnDownloadQR.Text = "Download";
            this.btnDownloadQR.UseVisualStyleBackColor = true;
            this.btnDownloadQR.Click += new System.EventHandler(this.btnDownloadQR_Click);

            // btnDeleteQR
            this.btnDeleteQR.Location = new System.Drawing.Point(280, 415);
            this.btnDeleteQR.Name = "btnDeleteQR";
            this.btnDeleteQR.Size = new System.Drawing.Size(120, 30);
            this.btnDeleteQR.TabIndex = 4;
            this.btnDeleteQR.Text = "Delete";
            this.btnDeleteQR.UseVisualStyleBackColor = true;
            this.btnDeleteQR.Click += new System.EventHandler(this.btnDeleteQR_Click);

            // btnClose
            this.btnClose.Location = new System.Drawing.Point(540, 415);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(120, 30);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            // frmQRCodeManager
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 470);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnDeleteQR);
            this.Controls.Add(this.btnDownloadQR);
            this.Controls.Add(this.btnGenerateQR);
            this.Controls.Add(this.lstQRCodes);
            this.Controls.Add(this.lblTitle);
            this.Name = "frmQRCodeManager";
            this.Text = "QR Code Manager - TagMyDrive";
            this.Load += new System.EventHandler(this.frmQRCodeManager_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ListBox lstQRCodes;
        private System.Windows.Forms.Button btnGenerateQR;
        private System.Windows.Forms.Button btnDownloadQR;
        private System.Windows.Forms.Button btnDeleteQR;
        private System.Windows.Forms.Button btnClose;
    }
}
