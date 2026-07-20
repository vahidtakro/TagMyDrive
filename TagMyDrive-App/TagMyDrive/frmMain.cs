using CommandLine.Utility;
using TagMyDrive.Properties;
using TagMyDrive.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySqlConnector;

namespace TagMyDrive
{
	public partial class frmMain : Form
	{
		private bool initDone = false;
		private bool runningAutomated = false;
		private bool silentMode = false;

		private DatabaseService _dbService;
		private AuthService _authService;
		private DiskService _diskService;
		private QRCodeService _qrCodeService;
		private GoogleDriveService _googleDriveService;
		private MembershipService _membershipService;
		private CryptoPaymentService _cryptoPaymentService;
		private int _currentUserId = 0;
		private string _lastOutputFile = "";
		private string _lastSnapshotTitle = "";

		public frmMain(DatabaseService dbService, AuthService authService, DiskService diskService,
			QRCodeService qrCodeService, GoogleDriveService googleDriveService, MembershipService membershipService,
			CryptoPaymentService cryptoPaymentService)
		{
			InitializeComponent();
			_dbService = dbService;
			_authService = authService;
			_diskService = diskService;
			_qrCodeService = qrCodeService;
			_googleDriveService = googleDriveService;
			_membershipService = membershipService;
			_cryptoPaymentService = cryptoPaymentService;
			_currentUserId = Program.CurrentUserId ?? 0;
		}

		private async void frmMain_Load(object sender, EventArgs e)
		{
			var version = Application.ProductVersion.Split('.')[0] + "." + Application.ProductVersion.Split('.')[1];
			this.Text = $"{Application.ProductName} {version} (Press F1 for Help)";
			labelAboutVersion.Text = $"version {version}";

			int left = Settings.Default.WindowLeft;
			int top = Settings.Default.WindowTop;
			if (left >= 0) this.Left = left;
			if (top >= 0) this.Top = top;

			if (System.IO.Directory.Exists(txtRoot.Text))
			{
				SetRootPath(txtRoot.Text, true);
			}
			else
			{
				SetRootPath("", false);
			}

			txtLinkRoot.Enabled = chkLinkFiles.Checked;

			tabPage1.DragDrop += DragDropHandler;
			tabPage1.DragEnter += DragEnterHandler;
			tabPage1.AllowDrop = true;
			foreach (Control cnt in tabPage1.Controls)
			{
				cnt.DragDrop += DragDropHandler;
				cnt.DragEnter += DragEnterHandler;
				cnt.AllowDrop = true;
			}

			HighDpiHelper.AdjustControlImagesDpiScale(this);
			Opacity = 0;
			initDone = true;

			if (_currentUserId > 0)
			{
				await InitializeUserSessionAsync();
				await LoadUserDisksAsync();
			}
		}

		private async Task InitializeUserSessionAsync()
		{
			try
			{
				var user = await _authService.GetUserByIdAsync(_currentUserId);
				if (user != null)
				{
					this.Text += $" - Logged in as: {user.Username}";
				}

				var membership = await _membershipService.GetUserMembershipAsync(_currentUserId);
				if (membership != null)
				{
					toolStripStatusLabelMembership.Text = $"Membership: {membership.Name}";
				}

				if (AppConfig.IsGoogleDriveConfigured() && !_googleDriveService.IsAuthenticated)
				{
					toolStripStatusLabel1.Text = "Authenticating with Google Drive...";
					Application.DoEvents();
					await _googleDriveService.AuthenticateAsync(
						AppConfig.GetGoogleDriveClientId(),
						AppConfig.GetGoogleDriveClientSecret(),
						AppConfig.GetGoogleDriveRedirectUri());
					toolStripStatusLabel1.Text = "Ready!";
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading user session: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async Task LoadUserDisksAsync()
		{
			try
			{
				var disks = await _diskService.GetUserDisksAsync(_currentUserId);
				lstDisks.Items.Clear();
				foreach (var disk in disks)
				{
					lstDisks.Items.Add(disk);
				}

				if (lstDisks.Items.Count == 0)
				{
					lstDisks.Items.Add("(No disks registered. Create a snapshot to auto-register.)");
				}
			}
			catch (Exception ex)
			{
				lstDisks.Items.Clear();
				lstDisks.Items.Add($"(Error loading disks: {ex.Message})");
			}
		}

		private void frmMain_Shown(object sender, EventArgs e)
		{
			try
			{
				var commandLine = Environment.CommandLine;
				commandLine = commandLine.Replace("-output:", "-outfile:");
				var splitCommandLine = Arguments.SplitCommandLine(commandLine);
				var arguments = new Arguments(splitCommandLine);

				if (splitCommandLine.Length == 2 && !arguments.Exists("path"))
				{
					if (System.IO.Directory.Exists(splitCommandLine[1]))
					{
						SetRootPath(splitCommandLine[1]);
					}
				}
				if (arguments.Exists("path") && !arguments.Exists("outfile"))
				{
					if (System.IO.Directory.Exists(splitCommandLine[1]))
					{
						SetRootPath(splitCommandLine[1]);
					}
				}

				var settings = new SnapSettings();
				if (arguments.Exists("path") && arguments.Exists("outfile"))
				{
					this.runningAutomated = true;

					if (arguments.Exists("silent"))
					{
						this.silentMode = true;
					}

					settings.rootFolder = arguments.Single("path");
					settings.outputFile = arguments.Single("outfile");

					if (!System.IO.Directory.Exists(settings.rootFolder))
					{
						if (!this.silentMode)
						{
							MessageBox.Show("Input path does not exist: " + settings.rootFolder, "Automation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
						Application.Exit();
					}
					if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(settings.outputFile)))
					{
						if (!this.silentMode)
						{
							MessageBox.Show("Output path does not exist: " + System.IO.Path.GetDirectoryName(settings.outputFile), "Automation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
						Application.Exit();
					}

					settings.skipHiddenItems = !arguments.Exists("hidden");
					settings.skipSystemItems = !arguments.Exists("system");
					settings.openInBrowser = false;

					settings.linkFiles = false;
					if (arguments.Exists("link"))
					{
						settings.linkFiles = true;
						settings.linkRoot = arguments.Single("link").Replace('\\', '/');
					}

					settings.title = "Snapshot of " + settings.rootFolder;
					if (arguments.Exists("title"))
					{
						settings.title = arguments.Single("title");
					}
				}

				if (!System.IO.File.Exists(Utils.GetTemplatePath()))
				{
					if (!this.silentMode)
					{
						MessageBox.Show("Template file was not found:\n\n" + Utils.GetTemplatePath(), "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					Application.Exit();
				}

				if (this.silentMode && this.runningAutomated)
				{
					Visible = false;
				}
				else
				{
					Opacity = 100;
				}

				if (this.runningAutomated)
				{
					StartProcessing(settings);
				}
			}
			catch (Exception)
			{
				Opacity = 100;
			}
		}

		private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (backgroundWorker.IsBusy) e.Cancel = true;

			if (!this.runningAutomated)
			{
				Settings.Default.WindowLeft = this.Left;
				Settings.Default.WindowTop = this.Top;
				Settings.Default.Save();
			}
		}

		private void cmdBrowse_Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Desktop;
			folderBrowserDialog1.SelectedPath = txtRoot.Text;
			folderBrowserDialog1.Description = "Select the root folder to create a snapshot from:";
			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
			{
				try
				{
					SetRootPath(folderBrowserDialog1.SelectedPath);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Could not select folder:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					SetRootPath("", false);
				}
			}
		}

		private void txtRoot_Leave(object sender, EventArgs e)
		{
			try
			{
				if (Directory.Exists(txtRoot.Text) == true)
				{
					if (Utils.IsWildcardMatch("?:", txtRoot.Text, false))
					{
						txtRoot.Text += @"\";
					}
					SetRootPath(txtRoot.Text);
				}
			}
			catch (Exception)
			{
			}
		}

		private void cmdCreate_Click(object sender, EventArgs e)
		{
			if (System.IO.Directory.Exists(txtRoot.Text) == false)
			{
				if (silentMode == false)
				{
					MessageBox.Show("Path does not exist:\n\n" + txtRoot.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				return;
			}

			if (!AppConfig.IsGoogleDriveConfigured())
			{
				MessageBox.Show(
					"Google Drive is not configured.\n\n" +
					"Please configure Google Drive in app.config before creating snapshots.",
					"Google Drive Not Configured",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning);
				return;
			}

			string fileName = new System.IO.DirectoryInfo(txtRoot.Text + @"\").Name;
			char[] invalid = System.IO.Path.GetInvalidFileNameChars();
			for (int i = 0; i < invalid.Length; i++)
			{
				fileName = fileName.Replace(invalid[i].ToString(), "");
			}

			if (!fileName.ToLower().EndsWith(".html")) fileName += ".html";

			string tempFolder = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "TagMyDrive");
			System.IO.Directory.CreateDirectory(tempFolder);
			string outputFile = System.IO.Path.Combine(tempFolder, fileName);

			var settings = new SnapSettings()
			{
				rootFolder = txtRoot.Text,
				title = txtTitle.Text,
				outputFile = outputFile,
				skipHiddenItems = !chkHidden.Checked,
				skipSystemItems = !chkSystem.Checked,
				openInBrowser = false,
				linkFiles = chkLinkFiles.Checked,
				linkRoot = txtLinkRoot.Text,
			};
			_lastSnapshotTitle = txtTitle.Text;
			StartProcessing(settings);
		}

		private void StartProcessing(SnapSettings settings)
		{
			settings.rootFolder = System.IO.Path.GetFullPath(settings.rootFolder);
			if (settings.rootFolder.EndsWith(@"\"))
			{
				settings.rootFolder = settings.rootFolder.Substring(0, settings.rootFolder.Length - 1);
			}
			if (Utils.IsWildcardMatch("?:", settings.rootFolder, false))
			{
				settings.rootFolder += @"\";
			}

			_lastOutputFile = settings.outputFile;
			_lastSnapshotTitle = settings.title;
			Cursor.Current = Cursors.WaitCursor;
			var version = Application.ProductVersion.Split('.')[0] + "." + Application.ProductVersion.Split('.')[1];
			this.Text = $"{Application.ProductName} {version} (Working... Press Escape to Cancel)";
			tabControl1.Enabled = false;
			backgroundWorker.RunWorkerAsync(argument: settings);
		}

		private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs args)
		{
			var path = args.UserState.ToString();
			var shortenedPath = Utils.ShortenPath(path, toolStripStatusLabel1.Font, statusStrip1.Width - 20);
			toolStripStatusLabel1.Text = shortenedPath;
		}

		private async void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args)
		{
			try
			{
				if (!this.silentMode)
				{
					if (args.Result != null)
					{
						var errorFolders = (List<SnappedFolder>)args.Result;
						if (errorFolders.Count > 0)
						{
							var yesno = MessageBox.Show(errorFolders.Count + " folder(s) could not be read. Show details?", "Errors Reported", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
							if (yesno == DialogResult.Yes)
							{
								var errorForm = new frmErrors(errorFolders);
								errorForm.ShowDialog();
							}
						}
					}
				}

				GC.Collect();
				GC.WaitForPendingFinalizers();

				Cursor.Current = Cursors.Default;
				tabControl1.Enabled = true;
				var version = Application.ProductVersion.Split('.')[0] + "." + Application.ProductVersion.Split('.')[1];
				this.Text = $"{Application.ProductName} {version} (Press F1 for Help)";

				if (_currentUserId > 0 && !string.IsNullOrEmpty(_lastOutputFile) && File.Exists(_lastOutputFile))
				{
					await PostSnapshotFlowAsync();
				}

				if (_currentUserId > 0)
				{
					await InitializeUserSessionAsync();
				}

				toolStripStatusLabel1.Text = "Ready!";

				if (!string.IsNullOrEmpty(_lastOutputFile) && File.Exists(_lastOutputFile))
				{
					await SaveExportHistoryAsync(_lastOutputFile);
				}

				if (this.runningAutomated)
				{
					Application.Exit();
				}
			}
			catch (Exception)
			{
				Cursor.Current = Cursors.Default;
				tabControl1.Enabled = true;
			}
		}

		private async Task PostSnapshotFlowAsync()
		{
			string gDriveUrl = null;
			string gDriveFileId = null;
			int? diskId = null;
			string statusMessage = "Snapshot created successfully.";

			try
			{
				toolStripStatusLabel1.Text = "Auto-creating disk entry...";

				var rootPath = txtRoot.Text;
				var existingDisk = await _diskService.GetDiskByPathAsync(_currentUserId, rootPath);

				if (existingDisk != null)
				{
					diskId = existingDisk.Id;
					await _diskService.UpdateDiskSnapshotAsync(diskId.Value,
						$"Updated from snapshot: {_lastOutputFile}");
					await _qrCodeService.DeactivateAllForDiskAsync(diskId.Value);
					await _googleDriveService.DeactivateAllForDiskAsync(diskId.Value);
					statusMessage = "Snapshot updated existing disk.";
				}
				else
				{
					var diskName = Path.GetFileNameWithoutExtension(_lastOutputFile);
					if (!string.IsNullOrEmpty(_lastSnapshotTitle) && _lastSnapshotTitle.StartsWith("Snapshot of "))
					{
						diskName = _lastSnapshotTitle.Substring("Snapshot of ".Length);
					}

					var (success, message, newDiskId) = await _diskService.CreateDiskAsync(
						_currentUserId,
						diskName,
						$"Auto-created from snapshot: {_lastOutputFile}",
						rootPath,
						"snapshot");

					if (success)
					{
						diskId = newDiskId;
					}
					else
					{
						statusMessage = $"Snapshot created. Disk creation failed: {message}";
					}
				}

				await LoadUserDisksAsync();
			}
			catch (Exception ex)
			{
				statusMessage = $"Snapshot created. Disk creation error: {ex.Message}";
			}

			if (AppConfig.IsGoogleDriveConfigured() && _currentUserId > 0 && diskId.HasValue)
			{
				try
				{
					toolStripStatusLabel1.Text = "Uploading to Google Drive...";

					if (!_googleDriveService.IsAuthenticated)
					{
						var (authSuccess, authMessage) = await _googleDriveService.AuthenticateAsync(
							AppConfig.GetGoogleDriveClientId(),
							AppConfig.GetGoogleDriveClientSecret(),
							AppConfig.GetGoogleDriveRedirectUri());

						if (!authSuccess)
						{
							statusMessage += " Google Drive authentication failed: " + authMessage;
							goto ShowResult;
						}
					}

					var (folderSuccess, folderId) = await _googleDriveService.GetOrCreateSnapshotsFolderAsync();
					if (!folderSuccess)
					{
						statusMessage += " Google Drive: could not create/access snapshots folder.";
						goto ShowResult;
					}

					var fileName = Path.GetFileName(_lastOutputFile);
					var (uploadSuccess, uploadMessage, fileId, fileUrl) = await _googleDriveService.UploadFileAsync(
						_lastOutputFile, fileName, folderId);

					if (uploadSuccess)
					{
						gDriveUrl = fileUrl;
						gDriveFileId = fileId;
						statusMessage = "Snapshot created and uploaded to Google Drive.";

						if (diskId.HasValue)
						{
							await _googleDriveService.SaveGoogleDriveLinkAsync(
								diskId.Value, _currentUserId, null,
								fileId, fileName, gDriveUrl, fileName);
						}
					}
					else
					{
						statusMessage += " Upload failed: " + uploadMessage;
					}
				}
				catch (Exception ex)
				{
					statusMessage += " Google Drive error: " + ex.Message;
				}
			}

ShowResult:
			Bitmap qrImage = null;

			if (!string.IsNullOrEmpty(gDriveUrl))
			{
				qrImage = QRCodeService.GenerateQRCodeImage(gDriveUrl);
			}

			try
			{
				var resultForm = new frmSnapshotResult(qrImage, gDriveUrl, txtRoot.Text, statusMessage);
				resultForm.ShowDialog();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Snapshot result: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

			try
			{
				if (!string.IsNullOrEmpty(_lastOutputFile) && File.Exists(_lastOutputFile)
					&& _lastOutputFile.StartsWith(Path.GetTempPath()))
				{
					File.Delete(_lastOutputFile);
				}
			}
			catch { }
		}

		private async Task SaveExportHistoryAsync(string htmlFile)
		{
			try
			{
				long fileSize = new FileInfo(htmlFile).Length;
				var query = "INSERT INTO export_history (user_id, html_filename, file_size, export_format) " +
							"VALUES (@userId, @filename, @fileSize, 'html')";
				await _dbService.ExecuteNonQueryAsync(query,
					new MySqlParameter("@userId", _currentUserId),
					new MySqlParameter("@filename", Path.GetFileName(htmlFile)),
					new MySqlParameter("@fileSize", fileSize));
			}
			catch
			{
			}
		}

		private void chkLinkFiles_CheckedChanged(object sender, EventArgs e)
		{
			if (chkLinkFiles.Checked == true)
				txtLinkRoot.Enabled = true;
			else
				txtLinkRoot.Enabled = false;
		}

		private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("notepad.exe", System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\template.html");
		}

		private void DragEnterHandler(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.Copy;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void DragDropHandler(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				if (files.Length == 1 && System.IO.Directory.Exists(files[0]))
				{
					SetRootPath(files[0]);
				}
			}
		}

		private void frmMain_KeyUp(object sender, KeyEventArgs e)
		{
			if (backgroundWorker.IsBusy)
			{
				if (e.KeyCode == Keys.Escape)
				{
					backgroundWorker.CancelAsync();
				}
			}
			else
			{
				if (e.KeyCode == Keys.F1)
				{
					System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\ReadMe.txt");
				}
			}
		}

		private void SetRootPath(string path, bool pathIsValid = true)
		{
			if (pathIsValid)
			{
				txtRoot.Text = path;
				cmdCreate.Enabled = true;
				toolStripStatusLabel1.Text = "";
				if (initDone)
				{
					txtLinkRoot.Text = Utils.PathToFileUri(path);
					txtTitle.Text = "Snapshot of " + path;
				}
			}
			else
			{
				txtRoot.Text = "";
				cmdCreate.Enabled = false;
				toolStripStatusLabel1.Text = "";
				if (initDone)
				{
					txtLinkRoot.Text = "";
					txtTitle.Text = "";
				}
			}
		}

		// === Menu Handlers ===

		private void menuExit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private async void menuDiskManager_Click(object sender, EventArgs e)
		{
			await OpenDiskManagerAsync();
		}

		private async void menuRefreshDisks_Click(object sender, EventArgs e)
		{
			await LoadUserDisksAsync();
		}

		private async void menuGoogleDrive_Click(object sender, EventArgs e)
		{
			await UploadToGoogleDriveAsync();
		}

		private async void menuMembership_Click(object sender, EventArgs e)
		{
			using (var memberForm = new frmMembership(_membershipService, _cryptoPaymentService, _currentUserId))
			{
				memberForm.ShowDialog();
				await InitializeUserSessionAsync();
			}
		}

		private void menuLogout_Click(object sender, EventArgs e)
		{
			if (backgroundWorker.IsBusy)
			{
				MessageBox.Show("Please wait for the current operation to finish.", "Busy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			Program.CurrentUserId = null;
			AppConfig.ClearSession();
			Application.Restart();
		}

		private void menuAbout_Click(object sender, EventArgs e)
		{
			tabControl1.SelectedTab = tabPageAbout;
		}

		// === Disk Tab Handlers ===

		private async void btnAddDisk_Click(object sender, EventArgs e)
		{
			using (var addForm = new frmAddDisk())
			{
				if (addForm.ShowDialog() == DialogResult.OK)
				{
					var result = await _diskService.CreateDiskAsync(_currentUserId, addForm.DiskName, addForm.DiskDescription, addForm.DiskPath, addForm.DiskType, addForm.DiskImageUrl);

					if (result.success)
					{
						MessageBox.Show("Disk added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
						await LoadUserDisksAsync();
					}
					else
					{
						MessageBox.Show(result.message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
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
			if (MessageBox.Show($"Delete disk '{disk.Name}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				var result = await _diskService.DeleteDiskAsync(disk.Id);

				if (result.success)
				{
					await LoadUserDisksAsync();
				}
				else
				{
					MessageBox.Show(result.message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void btnManageQRCodes_Click(object sender, EventArgs e)
		{
			if (lstDisks.SelectedIndex < 0 || !(lstDisks.SelectedItem is DiskService.Disk))
			{
				MessageBox.Show("Please select a disk first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			var disk = (DiskService.Disk)lstDisks.SelectedItem;
			using (var qrForm = new frmQRCodeManager(_qrCodeService, disk.Id, disk.Name))
			{
				qrForm.ShowDialog();
			}
		}

		private void btnSnapshotDisk_Click(object sender, EventArgs e)
		{
			if (lstDisks.SelectedIndex < 0 || !(lstDisks.SelectedItem is DiskService.Disk))
			{
				MessageBox.Show("Please select a disk first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			var disk = (DiskService.Disk)lstDisks.SelectedItem;

			if (!System.IO.Directory.Exists(disk.DiskPath))
			{
				MessageBox.Show($"Disk path does not exist:\n\n{disk.DiskPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			tabControl1.SelectedTab = tabPage1;
			txtRoot.Text = disk.DiskPath;
			txtTitle.Text = $"Snapshot of {disk.Name}";
			SetRootPath(disk.DiskPath);
		}

		private void lstDisks_DoubleClick(object sender, EventArgs e)
		{
			btnManageQRCodes_Click(sender, e);
		}

		private async void lstDisks_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstDisks.SelectedIndex < 0 || !(lstDisks.SelectedItem is DiskService.Disk))
			{
				picDiskQR.Image = null;
				lblDiskInfo.Text = "";
				lblDiskGDriveLabel.Text = "Google Drive Link:";
				lnkDiskGDrive.Text = "(Select a disk)";
				lnkDiskGDrive.Tag = null;
				return;
			}

			var disk = (DiskService.Disk)lstDisks.SelectedItem;
			lblDiskInfo.Text = $"Type: {disk.DiskType}\nPath: {disk.DiskPath}";

			try
			{
				var links = await _googleDriveService.GetDiskGoogleDriveLinksAsync(disk.Id);
				if (links.Count > 0)
				{
					var latest = links[0];
					lblDiskGDriveLabel.Text = "Google Drive Link:";
					lnkDiskGDrive.Text = latest.GoogleDriveUrl;
					lnkDiskGDrive.Tag = latest.GoogleDriveUrl;
				}
				else
				{
					lnkDiskGDrive.Tag = null;
				}

				var gDriveUrl = links.Count > 0 ? links[0].GoogleDriveUrl : null;

				if (!string.IsNullOrEmpty(gDriveUrl))
				{
					var qrImage = QRCodeService.GenerateQRCodeImage(gDriveUrl);
					picDiskQR.Image = qrImage;
					lblDiskQRLabel.Text = "QR Code (Google Drive)";
				}
				else
				{
					picDiskQR.Image = null;
					lblDiskQRLabel.Text = "No QR code";
				}
			}
			catch
			{
				picDiskQR.Image = null;
				lblDiskQRLabel.Text = "No QR code";
			}
		}

		private void lnkDiskGDrive_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (lnkDiskGDrive.Tag != null)
			{
				System.Diagnostics.Process.Start(lnkDiskGDrive.Tag.ToString());
			}
		}

		// === Feature Methods ===

		private async Task OpenDiskManagerAsync()
		{
			using (var diskForm = new frmDiskManager(_dbService, _diskService, _qrCodeService, _googleDriveService, _currentUserId))
			{
				diskForm.ShowDialog();
				await LoadUserDisksAsync();
			}
		}

		private async Task UploadToGoogleDriveAsync()
		{
			if (string.IsNullOrEmpty(_lastOutputFile) || !File.Exists(_lastOutputFile))
			{
				MessageBox.Show("No snapshot file available. Create a snapshot first.", "No File", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			if (!AppConfig.IsGoogleDriveConfigured())
			{
				MessageBox.Show(
					"Google Drive is not configured.\n\n" +
					"To enable Google Drive uploads, set your OAuth credentials in app.config.",
					"Google Drive Not Configured",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				return;
			}

			try
			{
				toolStripStatusLabel1.Text = "Uploading to Google Drive...";
				Application.DoEvents();

				if (!_googleDriveService.IsAuthenticated)
				{
					var (authSuccess, authMessage) = await _googleDriveService.AuthenticateAsync(
						AppConfig.GetGoogleDriveClientId(),
						AppConfig.GetGoogleDriveClientSecret(),
						AppConfig.GetGoogleDriveRedirectUri());

					if (!authSuccess)
					{
						MessageBox.Show(authMessage, "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
						toolStripStatusLabel1.Text = "Ready!";
						return;
					}
				}

				var (folderSuccess, folderId) = await _googleDriveService.GetOrCreateSnapshotsFolderAsync();

				var uploadResult = await _googleDriveService.UploadFileAsync(
					_lastOutputFile,
					Path.GetFileName(_lastOutputFile),
					folderId);

				if (uploadResult.success)
				{
					MessageBox.Show($"File uploaded to Google Drive!\n\n{uploadResult.fileUrl}", "Upload Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else
				{
					MessageBox.Show(uploadResult.message, "Upload Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Google Drive error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				toolStripStatusLabel1.Text = "Ready!";
			}
		}

		private async System.Threading.Tasks.Task ShowMembershipInfoAsync()
		{
			try
			{
				var membership = await _membershipService.GetUserMembershipAsync(_currentUserId);
				var diskCount = await _diskService.GetUserActiveDiskCountAsync(_currentUserId);

				if (membership != null)
				{
					var limitText = membership.MaxDisks >= 999 ? "Unlimited" : membership.MaxDisks.ToString();
					MessageBox.Show(
						$"Current Plan: {membership.Name}\n" +
						$"Description: {membership.Description}\n" +
						$"Disks Used: {diskCount} / {limitText}\n" +
						$"Price: {(membership.PriceMonthly > 0 ? $"${membership.PriceMonthly}/month" : "Free")}",
						"Membership Info",
						MessageBoxButtons.OK,
						MessageBoxIcon.Information);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading membership: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
