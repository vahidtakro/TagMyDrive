using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using TagMyDrive.Services;

namespace TagMyDrive
{
    public partial class frmLogin : Form
    {
        private readonly AuthService _authService;
        private readonly DatabaseService _dbService;

        public int? CurrentUserId { get; private set; }

        public frmLogin(DatabaseService dbService, AuthService authService)
        {
            InitializeComponent();
            _dbService = dbService;
            _authService = authService;
            CurrentUserId = null;
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            // Set up form
            this.Text = "TagMyDrive - Login";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(400, 300);

            // Focus on username field
            txtUsername.Focus();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter your username or email.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter your password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            // Disable button during login
            btnLogin.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                var (success, message, user) = await _authService.LoginAsync(txtUsername.Text, txtPassword.Text);

                if (success)
                {
                    CurrentUserId = user.Id;
                    if (chkRememberMe.Checked)
                        AppConfig.SaveSession(user.Id);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(message, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Open register form
            frmRegister registerForm = new frmRegister(_dbService, new AuthService(_dbService));
            if (registerForm.ShowDialog() == DialogResult.OK)
            {
                // Auto-fill username if registration was successful
                txtUsername.Text = registerForm.NewUsername;
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                btnLogin_Click(sender, e);
                e.Handled = true;
            }
        }
    }
}
