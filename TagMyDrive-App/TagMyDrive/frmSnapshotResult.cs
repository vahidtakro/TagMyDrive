using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace TagMyDrive
{
    public partial class frmSnapshotResult : Form
    {
        private Bitmap _qrCodeImage;
        private string _googleDriveUrl;
        private string _diskName;
        private string _statusMessage;

        public frmSnapshotResult(Bitmap qrCodeImage, string googleDriveUrl, string diskName, string statusMessage)
        {
            InitializeComponent();
            _qrCodeImage = qrCodeImage;
            _googleDriveUrl = googleDriveUrl;
            _diskName = diskName;
            _statusMessage = statusMessage;
        }

        private void frmSnapshotResult_Load(object sender, EventArgs e)
        {
            this.Text = "Snapshot Created - TagMyDrive";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            lblTitle.Text = $"Snapshot: {_diskName}";
            lblStatus.Text = _statusMessage;

            if (_qrCodeImage != null)
            {
                picQRCode.Image = _qrCodeImage;
                btnDownloadQR.Enabled = true;
                btnPrintQR.Enabled = true;
            }
            else
            {
                picQRCode.Image = null;
                btnDownloadQR.Enabled = false;
                btnPrintQR.Enabled = false;
                lblQRInfo.Text = string.IsNullOrEmpty(_googleDriveUrl)
                    ? "No QR code (Google Drive upload failed)"
                    : "QR code generation failed";
            }

            if (!string.IsNullOrEmpty(_googleDriveUrl))
            {
                lnkGoogleDrive.Text = _googleDriveUrl;
                lnkGoogleDrive.Tag = _googleDriveUrl;
                lnkGoogleDrive.Visible = true;
                lblGoogleDriveLabel.Visible = true;
            }
            else
            {
                lnkGoogleDrive.Visible = false;
                lblGoogleDriveLabel.Visible = false;
                lblQRInfo.Text = "No Google Drive link available";
            }
        }

        private void btnDownloadQR_Click(object sender, EventArgs e)
        {
            if (_qrCodeImage == null) return;

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.FileName = $"QRCode_{_diskName}_{DateTime.Now:yyyyMMdd}.png";
                saveDialog.Filter = "PNG Image|*.png";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    _qrCodeImage.Save(saveDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    MessageBox.Show($"QR code saved to:\n{saveDialog.FileName}", "Downloaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnPrintQR_Click(object sender, EventArgs e)
        {
            if (_qrCodeImage == null) return;

            var printDoc = new PrintDocument();
            printDoc.PrintPage += (s, pe) =>
            {
                var img = _qrCodeImage;
                var targetWidth = 300;
                var targetHeight = 300;

                var x = (pe.MarginBounds.Width - targetWidth) / 2 + pe.MarginBounds.X;
                var y = pe.MarginBounds.Y;

                pe.Graphics.DrawImage(img, x, y, targetWidth, targetHeight);

                var titleFont = new Font("Arial", 14, FontStyle.Bold);
                var textFont = new Font("Arial", 10);
                var textY = y + targetHeight + 20;

                pe.Graphics.DrawString("TagMyDrive", titleFont, Brushes.Black, x, textY);
                pe.Graphics.DrawString(_diskName, textFont, Brushes.Black, x, textY + 25);

                if (!string.IsNullOrEmpty(_googleDriveUrl))
                {
                    var smallFont = new Font("Arial", 7);
                    pe.Graphics.DrawString("Scan to view on Google Drive", smallFont, Brushes.Gray, x, textY + 45);
                }
            };

            var printDialog = new PrintDialog();
            printDialog.Document = printDoc;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDoc.Print();
            }
        }

        private void lnkGoogleDrive_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (lnkGoogleDrive.Tag != null)
            {
                System.Diagnostics.Process.Start(lnkGoogleDrive.Tag.ToString());
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
