using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TagMyDrive.Services;

namespace TagMyDrive
{
    public partial class frmQRCodeManager : Form
    {
        private readonly QRCodeService _qrCodeService;
        private readonly int _diskId;
        private readonly string _diskName;
        private List<QRCodeService.QRCodeInfo> _qrCodes;

        public frmQRCodeManager(QRCodeService qrCodeService, int diskId, string diskName)
        {
            InitializeComponent();
            _qrCodeService = qrCodeService;
            _diskId = diskId;
            _diskName = diskName;
            _qrCodes = new List<QRCodeService.QRCodeInfo>();
        }

        private void frmQRCodeManager_Load(object sender, EventArgs e)
        {
            this.Text = $"QR Codes - {_diskName}";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new System.Drawing.Size(700, 500);

            LoadQRCodes();
        }

        private void LoadQRCodes()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                _qrCodes = System.Threading.Tasks.Task.Run(async () => 
                    await _qrCodeService.GetDiskQRCodesAsync(_diskId)).Result;

                lstQRCodes.Items.Clear();
                if (_qrCodes.Count == 0)
                {
                    lstQRCodes.Items.Add("No QR codes generated yet");
                }
                else
                {
                    foreach (var qr in _qrCodes)
                    {
                        lstQRCodes.Items.Add($"ID: {qr.Id} | Created: {qr.CreatedAt:yyyy-MM-dd HH:mm} | Downloads: {qr.DownloadCount}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading QR codes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async void btnGenerateQR_Click(object sender, EventArgs e)
        {
            var htmlFilename = Microsoft.VisualBasic.Interaction.InputBox("Enter HTML filename:", "Generate QR Code", "export.html");

            if (!string.IsNullOrWhiteSpace(htmlFilename))
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    var (success, message, qrCodeId) = await _qrCodeService.GenerateQRCodeAsync(
                        _diskId, 0, htmlFilename, $"https://tagmydrive.local/view/{_diskId}");

                    if (success)
                    {
                        MessageBox.Show("QR code generated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadQRCodes();
                    }
                    else
                    {
                        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void btnDownloadQR_Click(object sender, EventArgs e)
        {
            if (lstQRCodes.SelectedIndex < 0 || lstQRCodes.SelectedIndex >= _qrCodes.Count)
            {
                MessageBox.Show("Please select a QR code to download.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var qrCode = _qrCodes[lstQRCodes.SelectedIndex];

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.FileName = $"QRCode_{qrCode.Id}.png";
                saveDialog.Filter = "PNG Image|*.png";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        this.Cursor = Cursors.WaitCursor;
                        var url = qrCode.QrCodeUrl;
                        if (!string.IsNullOrEmpty(url))
                        {
                            using (var bitmap = QRCodeService.GenerateQRCodeImage(url))
                            {
                                bitmap.Save(saveDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                            }
                            MessageBox.Show($"QR code saved to:\n{saveDialog.FileName}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No URL available for this QR code.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }

        private async void btnDeleteQR_Click(object sender, EventArgs e)
        {
            if (lstQRCodes.SelectedIndex < 0 || lstQRCodes.SelectedIndex >= _qrCodes.Count)
            {
                MessageBox.Show("Please select a QR code to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var qrCode = _qrCodes[lstQRCodes.SelectedIndex];

            if (MessageBox.Show("Are you sure you want to delete this QR code?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    var (success, message) = await _qrCodeService.DeactivateQRCodeAsync(qrCode.Id);

                    if (success)
                    {
                        MessageBox.Show("QR code deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadQRCodes();
                    }
                    else
                    {
                        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
