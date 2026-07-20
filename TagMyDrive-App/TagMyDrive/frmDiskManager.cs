using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using TagMyDrive.Services;

namespace TagMyDrive
{
    public partial class frmDiskManager : Form
    {
        private readonly DatabaseService _dbService;
        private readonly DiskService _diskService;
        private readonly QRCodeService _qrCodeService;
        private readonly GoogleDriveService _googleDriveService;
        private readonly int _userId;
        private List<DiskService.Disk> _userDisks;
        private bool _isEditing;

        public frmDiskManager(DatabaseService dbService, DiskService diskService,
            QRCodeService qrCodeService, GoogleDriveService googleDriveService, int userId)
        {
            InitializeComponent();
            _dbService = dbService;
            _diskService = diskService;
            _qrCodeService = qrCodeService;
            _googleDriveService = googleDriveService;
            _userId = userId;
            _userDisks = new List<DiskService.Disk>();
        }

        private async void frmDiskManager_Load(object sender, EventArgs e)
        {
            this.Text = "Disk Manager - TagMyDrive";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new System.Drawing.Size(850, 620);

            picDiskImage.SizeMode = PictureBoxSizeMode.Zoom;
            picDiskImage.BorderStyle = BorderStyle.FixedSingle;
            picDiskImage.BackColor = Color.White;

            SetEditMode(false);
            await LoadDisksAsync();
            ClearDetails();
        }

        private void SetEditMode(bool editing)
        {
            _isEditing = editing;
            txtEditName.Visible = editing;
            txtEditDescription.Visible = editing;
            lblEditNameLabel.Visible = editing;
            lblEditDescLabel.Visible = editing;
            btnEditSave.Visible = editing;
            btnEditCancel.Visible = editing;
            btnChangeImage.Visible = editing;
            btnRemoveImage.Visible = editing;

            lblDiskName.Visible = !editing;
            lblDiskType.Visible = !editing;

            if (editing)
            {
                btnEditDisk.Text = "Cancel Edit";
            }
            else
            {
                btnEditDisk.Text = "Edit Disk";
            }
        }

        private async Task LoadDisksAsync()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                _userDisks = await _diskService.GetUserDisksAsync(_userId);

                lstDisks.Items.Clear();
                foreach (var disk in _userDisks)
                {
                    lstDisks.Items.Add(disk);
                }

                if (lstDisks.Items.Count == 0)
                {
                    lstDisks.Items.Add("(No disks registered)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading disks: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async void lstDisks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDisks.SelectedIndex < 0 || !(lstDisks.SelectedItem is DiskService.Disk))
            {
                ClearDetails();
                return;
            }

            var disk = (DiskService.Disk)lstDisks.SelectedItem;
            SetEditMode(false);
            await LoadDiskDetailsAsync(disk);
        }

        private async Task LoadDiskDetailsAsync(DiskService.Disk disk)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                lblDiskName.Text = disk.Name;
                lblDiskType.Text = $"Type: {disk.DiskType}";
                lblDiskPath.Text = $"Path: {disk.DiskPath}";
                lblDiskCreated.Text = $"Created: {disk.CreatedAt:yyyy-MM-dd HH:mm}";
                lblDiskUpdated.Text = $"Updated: {disk.UpdatedAt:yyyy-MM-dd HH:mm}";
                txtEditName.Text = disk.Name;
                txtEditDescription.Text = disk.Description;
                panelDetails.Visible = true;

                // Show disk image from URL
                if (!string.IsNullOrEmpty(disk.ImageUrl))
                {
                    try
                    {
                        var request = (HttpWebRequest)WebRequest.Create(disk.ImageUrl);
                        request.Timeout = 8000;
                        using (var response = request.GetResponse())
                        using (var stream = response.GetResponseStream())
                        {
                            picDiskImage.Image = Image.FromStream(stream).GetThumbnailImage(200, 200, null, IntPtr.Zero);
                        }
                        lblImagePlaceholder.Visible = false;
                    }
                    catch
                    {
                        picDiskImage.Image = null;
                        lblImagePlaceholder.Visible = true;
                    }
                }
                else
                {
                    picDiskImage.Image = null;
                    lblImagePlaceholder.Visible = true;
                }

                lstLinks.Items.Clear();
                try
                {
                    var links = await _googleDriveService.GetDiskGoogleDriveLinksAsync(disk.Id);
                    if (links.Count > 0)
                    {
                        foreach (var link in links)
                        {
                            lstLinks.Items.Add(link);
                        }
                    }
                    else
                    {
                        lstLinks.Items.Add("(No Google Drive links)");
                    }
                }
                catch
                {
                    lstLinks.Items.Add("(No Google Drive links)");
                }

                lstQRCodes.Items.Clear();
                try
                {
                    var qrCodes = await _qrCodeService.GetDiskQRCodesAsync(disk.Id);
                    if (qrCodes.Count > 0)
                    {
                        foreach (var qr in qrCodes)
                        {
                            lstQRCodes.Items.Add(qr);
                        }
                    }
                    else
                    {
                        lstQRCodes.Items.Add("(No QR codes)");
                    }
                }
                catch
                {
                    lstQRCodes.Items.Add("(No QR codes)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void ClearDetails()
        {
            lblDiskName.Text = "Select a disk";
            lblDiskType.Text = "";
            lblDiskPath.Text = "";
            lblDiskCreated.Text = "";
            lblDiskUpdated.Text = "";
            picDiskImage.Image = null;
            lblImagePlaceholder.Visible = true;
            panelDetails.Visible = false;
            lstLinks.Items.Clear();
            lstQRCodes.Items.Clear();
            SetEditMode(false);
        }

        private async void btnEditDisk_Click(object sender, EventArgs e)
        {
            if (_isEditing)
            {
                SetEditMode(false);
                if (lstDisks.SelectedItem is DiskService.Disk disk)
                {
                    await LoadDiskDetailsAsync(disk);
                }
                return;
            }

            if (!(lstDisks.SelectedItem is DiskService.Disk currentDisk)) return;
            SetEditMode(true);
        }

        private async void btnEditSave_Click(object sender, EventArgs e)
        {
            if (!(lstDisks.SelectedItem is DiskService.Disk disk)) return;

            if (string.IsNullOrWhiteSpace(txtEditName.Text))
            {
                MessageBox.Show("Disk name cannot be empty.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                var (success, message) = await _diskService.UpdateDiskDetailsAsync(disk.Id, txtEditName.Text, txtEditDescription.Text);
                if (success)
                {
                    SetEditMode(false);
                    await LoadDisksAsync();
                    var updatedDisk = await _diskService.GetDiskByIdAsync(disk.Id);
                    if (updatedDisk != null) await LoadDiskDetailsAsync(updatedDisk);
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

        private void btnEditCancel_Click(object sender, EventArgs e)
        {
            SetEditMode(false);
            if (lstDisks.SelectedItem is DiskService.Disk disk)
            {
                txtEditName.Text = disk.Name;
                txtEditDescription.Text = disk.Description;
            }
        }

        private async void btnChangeImage_Click(object sender, EventArgs e)
        {
            using (var inputForm = new Form())
            {
                inputForm.Text = "Enter Image URL";
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;
                inputForm.Size = new Size(450, 130);

                var lbl = new Label { Text = "Image URL:", Location = new Point(10, 15), AutoSize = true };
                var txtUrl = new TextBox { Location = new Point(10, 38), Size = new Size(415, 23) };

                if (lstDisks.SelectedItem is DiskService.Disk currentDisk)
                    txtUrl.Text = currentDisk.ImageUrl ?? "";

                var btnOk = new Button { Text = "Load", DialogResult = DialogResult.OK, Location = new Point(240, 68), Size = new Size(90, 28) };
                var btnCancelInput = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(335, 68), Size = new Size(90, 28) };

                inputForm.Controls.AddRange(new Control[] { lbl, txtUrl, btnOk, btnCancelInput });
                inputForm.AcceptButton = btnOk;
                inputForm.CancelButton = btnCancelInput;

                if (inputForm.ShowDialog() == DialogResult.OK && lstDisks.SelectedItem is DiskService.Disk disk)
                {
                    var url = txtUrl.Text.Trim();
                    if (!string.IsNullOrEmpty(url))
                    {
                        try
                        {
                            this.Cursor = Cursors.WaitCursor;
                            var request = (HttpWebRequest)WebRequest.Create(url);
                            request.Timeout = 8000;
                            using (var response = request.GetResponse())
                            using (var stream = response.GetResponseStream())
                            {
                                picDiskImage.Image = Image.FromStream(stream).GetThumbnailImage(200, 200, null, IntPtr.Zero);
                            }
                            lblImagePlaceholder.Visible = false;
                            await _diskService.UpdateDiskImageUrlAsync(disk.Id, url);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Failed to load image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            this.Cursor = Cursors.Default;
                        }
                    }
                }
            }
        }

        private async void btnRemoveImage_Click(object sender, EventArgs e)
        {
            if (!(lstDisks.SelectedItem is DiskService.Disk disk)) return;

            picDiskImage.Image = null;
            lblImagePlaceholder.Visible = true;
            await _diskService.UpdateDiskImageUrlAsync(disk.Id, null);
        }

        private void btnOpenGDriveLink_Click(object sender, EventArgs e)
        {
            if (lstLinks.SelectedItem is GoogleDriveService.GoogleDriveLinkInfo link)
            {
                System.Diagnostics.Process.Start(link.GoogleDriveUrl);
            }
            else
            {
                MessageBox.Show("Please select a Google Drive link.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCopyGDriveLink_Click(object sender, EventArgs e)
        {
            if (lstLinks.SelectedItem is GoogleDriveService.GoogleDriveLinkInfo link)
            {
                Clipboard.SetText(link.GoogleDriveUrl);
                MessageBox.Show("Google Drive link copied to clipboard.", "Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select a Google Drive link.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDownloadQR_Click(object sender, EventArgs e)
        {
            if (lstQRCodes.SelectedItem is QRCodeService.QRCodeInfo qr)
            {
                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.FileName = $"QRCode_{qr.Id}.png";
                    saveDialog.Filter = "PNG Image|*.png";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            this.Cursor = Cursors.WaitCursor;
                            var url = qr.QrCodeUrl;
                            if (!string.IsNullOrEmpty(url))
                            {
                                using (var bitmap = QRCodeService.GenerateQRCodeImage(url))
                                {
                                    bitmap.Save(saveDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                                }
                                MessageBox.Show($"QR code saved to:\n{saveDialog.FileName}", "Downloaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            else
            {
                MessageBox.Show("Please select a QR code.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnViewQR_Click(object sender, EventArgs e)
        {
            if (lstQRCodes.SelectedItem is QRCodeService.QRCodeInfo qr)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    var url = qr.QrCodeUrl;
                    if (!string.IsNullOrEmpty(url))
                    {
                        var bitmap = QRCodeService.GenerateQRCodeImage(url);
                        var previewForm = new Form
                        {
                            Text = $"QR Code - {qr.HtmlExportFilename}",
                            Size = new Size(350, 400),
                            FormBorderStyle = FormBorderStyle.FixedSingle,
                            MaximizeBox = false,
                            StartPosition = FormStartPosition.CenterParent
                        };

                        var picBox = new PictureBox
                        {
                            Image = bitmap,
                            SizeMode = PictureBoxSizeMode.Zoom,
                            Dock = DockStyle.Fill,
                            Padding = new Padding(20)
                        };
                        previewForm.Controls.Add(picBox);
                        previewForm.ShowDialog();
                        bitmap.Dispose();
                    }
                    else
                    {
                        MessageBox.Show("No URL available for this QR code.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
            else
            {
                MessageBox.Show("Please select a QR code.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void btnDeleteDisk_Click(object sender, EventArgs e)
        {
            if (lstDisks.SelectedIndex < 0 || !(lstDisks.SelectedItem is DiskService.Disk))
            {
                MessageBox.Show("Please select a disk to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var disk = (DiskService.Disk)lstDisks.SelectedItem;
            if (MessageBox.Show($"Are you sure you want to delete '{disk.Name}'?\n\nThis will also remove all QR codes and Google Drive links.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    var (success, message) = await _diskService.DeleteDiskAsync(disk.Id);

                    if (success)
                    {
                        ClearDetails();
                        await LoadDisksAsync();
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
