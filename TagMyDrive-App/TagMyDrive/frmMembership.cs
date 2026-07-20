using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TagMyDrive.Services;

namespace TagMyDrive
{
    public partial class frmMembership : Form
    {
        private readonly MembershipService _membershipService;
        private readonly CryptoPaymentService _cryptoPaymentService;
        private readonly int _userId;

        private List<MembershipService.Membership> _memberships;
        private MembershipService.Membership _currentMembership;
        private int _selectedPaymentId = 0;

        public frmMembership(MembershipService membershipService, CryptoPaymentService cryptoPaymentService, int userId)
        {
            InitializeComponent();
            _membershipService = membershipService;
            _cryptoPaymentService = cryptoPaymentService;
            _userId = userId;
        }

        private async void frmMembership_Load(object sender, EventArgs e)
        {
            this.Text = "Membership & Plans - TagMyDrive";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            panelPayment.Visible = false;

            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                _currentMembership = await _membershipService.GetUserMembershipAsync(_userId);
                _memberships = await _membershipService.GetAllMembershipsAsync();

                if (_currentMembership != null)
                {
                    lblCurrentPlan.Text = $"Current Plan: {_currentMembership.Name}";
                    lblCurrentDesc.Text = _currentMembership.Description;
                    var limitText = _currentMembership.MaxDisks >= 999 ? "Unlimited" : _currentMembership.MaxDisks.ToString();
                    lblCurrentLimits.Text = $"Disk Limit: {limitText} | Monthly: {(_currentMembership.PriceMonthly > 0 ? $"${_currentMembership.PriceMonthly}" : "Free")}";
                }

                cmbPlans.Items.Clear();
                foreach (var m in _memberships)
                {
                    if (_currentMembership != null && m.Id == _currentMembership.Id) continue;
                    cmbPlans.Items.Add(m);
                }

                if (cmbPlans.Items.Count > 0)
                    cmbPlans.SelectedIndex = 0;

                if (!_cryptoPaymentService.IsConfigured())
                {
                    lblPaymentNote.Text = "Crypto payments not configured. Contact administrator.";
                    btnUpgrade.Enabled = false;
                    btnShowQR.Enabled = false;
                }
                else
                {
                    btnUpgrade.Enabled = cmbPlans.Items.Count > 0;
                    btnShowQR.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading membership data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void cmbPlans_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPlans.SelectedItem is MembershipService.Membership selected)
            {
                lblPlanDetails.Text = $"{selected.Description}\nDisk Limit: {(selected.MaxDisks >= 999 ? "Unlimited" : selected.MaxDisks.ToString())}\nPrice: ${(selected.PriceMonthly > 0 ? $"${selected.PriceMonthly}/month" : "Free")}";

                UpdatePaymentInfo();
            }
        }

        private void UpdatePaymentInfo()
        {
            if (cmbPlans.SelectedItem is MembershipService.Membership selected)
            {
                var amount = _cryptoPaymentService.GetPriceForMembership(selected.Id);
                if (amount > 0)
                {
                    lblPayAmount.Text = $"Amount: {amount} USDT (BEP20)";
                    panelPayment.Visible = true;
                }
                else
                {
                    lblPayAmount.Text = "Price not set for this plan.";
                    panelPayment.Visible = false;
                }
            }
        }

        private async void btnUpgrade_Click(object sender, EventArgs e)
        {
            if (cmbPlans.SelectedItem is MembershipService.Membership selected)
            {
                if (string.IsNullOrWhiteSpace(txtTxHash.Text))
                {
                    MessageBox.Show("Please enter the transaction hash after sending payment.", "Transaction Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    btnUpgrade.Enabled = false;

                    _selectedPaymentId = await _cryptoPaymentService.CreatePaymentAsync(_userId, selected.Id);

                    if (_selectedPaymentId > 0)
                    {
                        var (success, message) = await _cryptoPaymentService.VerifyAndUpgradeAsync(_selectedPaymentId, txtTxHash.Text.Trim());

                        if (success)
                        {
                            MessageBox.Show(
                                $"Payment verified and membership upgraded!\n\n" +
                                $"Plan: {selected.Name}\n" +
                                $"Amount: {_cryptoPaymentService.GetPriceForMembership(selected.Id)} USDT\n" +
                                $"Transaction: {txtTxHash.Text.Trim()}\n\n" +
                                $"Your membership is now active.",
                                "Upgrade Complete",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show(message, "Verification Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            btnUpgrade.Enabled = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to create payment record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btnUpgrade.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnUpgrade.Enabled = true;
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void btnShowQR_Click(object sender, EventArgs e)
        {
            if (cmbPlans.SelectedItem is MembershipService.Membership selected)
            {
                var address = _cryptoPaymentService.GetWalletAddress();
                var amount = _cryptoPaymentService.GetPriceForMembership(selected.Id);

                if (!string.IsNullOrEmpty(address) && amount > 0)
                {
                    using (var qrImage = _cryptoPaymentService.GeneratePaymentQRCode(address, amount))
                    {
                        var form = new Form();
                        form.Text = $"Pay {amount} USDT (BEP20)";
                        form.Size = new Size(380, 450);
                        form.FormBorderStyle = FormBorderStyle.FixedSingle;
                        form.MaximizeBox = false;
                        form.StartPosition = FormStartPosition.CenterParent;

                        var picBox = new PictureBox();
                        picBox.Image = new Bitmap(qrImage);
                        picBox.SizeMode = PictureBoxSizeMode.Zoom;
                        picBox.Size = new Size(250, 250);
                        picBox.Location = new Point(55, 10);
                        form.Controls.Add(picBox);

                        var lblNetwork = new Label();
                        lblNetwork.Text = "Network: BNB Smart Chain (BEP20)";
                        lblNetwork.Location = new Point(20, 270);
                        lblNetwork.AutoSize = true;
                        lblNetwork.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
                        lblNetwork.ForeColor = Color.FromArgb(243, 156, 18);
                        form.Controls.Add(lblNetwork);

                        var lblAmt = new Label();
                        lblAmt.Text = $"Send exactly {amount} USDT to:";
                        lblAmt.Location = new Point(20, 292);
                        lblAmt.AutoSize = true;
                        lblAmt.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                        form.Controls.Add(lblAmt);

                        var txtAddr = new TextBox();
                        txtAddr.Text = address;
                        txtAddr.Location = new Point(20, 315);
                        txtAddr.Size = new Size(325, 50);
                        txtAddr.ReadOnly = true;
                        txtAddr.Multiline = true;
                        txtAddr.WordWrap = true;
                        txtAddr.Font = new Font("Segoe UI", 9F);
                        form.Controls.Add(txtAddr);

                        var btnCopy = new Button();
                        btnCopy.Text = "Copy Address";
                        btnCopy.Location = new Point(20, 380);
                        btnCopy.Size = new Size(150, 30);
                        btnCopy.Click += (s, ev) => { Clipboard.SetText(address); };
                        form.Controls.Add(btnCopy);

                        var btnClose = new Button();
                        btnClose.Text = "Close";
                        btnClose.Location = new Point(200, 380);
                        btnClose.Size = new Size(145, 30);
                        btnClose.Click += (s, ev) => { form.Close(); };
                        form.Controls.Add(btnClose);

                        form.ShowDialog(this);
                    }
                }
                else
                {
                    MessageBox.Show("Payment details not available. Check USDT wallet configuration.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
