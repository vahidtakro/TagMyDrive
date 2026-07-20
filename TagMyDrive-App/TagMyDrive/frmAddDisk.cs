using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace TagMyDrive
{
    public partial class frmAddDisk : Form
    {
        public string DiskName { get; private set; }
        public string DiskDescription { get; private set; }
        public string DiskPath { get; private set; }
        public string DiskType { get; private set; }
        public string DiskImageUrl { get; private set; }

        public frmAddDisk()
        {
            InitializeComponent();
        }

        private void frmAddDisk_Load(object sender, EventArgs e)
        {
            this.Text = "Add New Disk - TagMyDrive";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new System.Drawing.Size(500, 380);

            cmbDiskType.Items.Add("Local");
            cmbDiskType.Items.Add("External");
            cmbDiskType.Items.Add("Network");
            cmbDiskType.SelectedIndex = 0;

            picDiskImage.SizeMode = PictureBoxSizeMode.Zoom;
            picDiskImage.BorderStyle = BorderStyle.FixedSingle;
            picDiskImage.BackColor = Color.White;

            txtDiskName.Focus();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var folderBrowser = new FolderBrowserDialog())
            {
                folderBrowser.Description = "Select a folder or disk to track";
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    txtDiskPath.Text = folderBrowser.SelectedPath;

                    if (string.IsNullOrWhiteSpace(txtDiskName.Text))
                    {
                        txtDiskName.Text = Path.GetFileName(folderBrowser.SelectedPath);
                    }
                }
            }
        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            var url = txtImageUrl.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Please enter an image URL.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 10000;
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    var image = Image.FromStream(stream);
                    picDiskImage.Image = image;
                    DiskImageUrl = url;
                    lblImageStatus.Text = "Image loaded";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load image from URL:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnRemoveImage_Click(object sender, EventArgs e)
        {
            picDiskImage.Image = null;
            DiskImageUrl = null;
            txtImageUrl.Text = "";
            lblImageStatus.Text = "No image";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDiskName.Text))
            {
                MessageBox.Show("Please enter a disk name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiskName.Focus();
                return;
            }

            DiskName = txtDiskName.Text;
            DiskDescription = txtDescription.Text;
            DiskPath = txtDiskPath.Text;
            DiskType = cmbDiskType.SelectedItem?.ToString().ToLower() ?? "local";

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
