using System;
using System.Windows.Forms;
using TagMyDrive.Services;

namespace TagMyDrive
{
    static class Program
    {
        public static int? CurrentUserId { get; set; }

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var dbService = new DatabaseService(AppConfig.GetDatabaseConnectionString());
            var authService = new AuthService(dbService);
            var diskService = new DiskService(dbService);
            var qrCodeService = new QRCodeService(dbService);
            var googleDriveService = new GoogleDriveService(dbService);
            var membershipService = new MembershipService(dbService);
            var cryptoPaymentService = new CryptoPaymentService(dbService);

            var savedUserId = AppConfig.LoadSession();
            if (savedUserId > 0)
            {
                var user = System.Threading.Tasks.Task.Run(() => authService.GetUserByIdAsync(savedUserId)).Result;
                if (user != null && user.IsActive)
                {
                    CurrentUserId = savedUserId;
                    Application.Run(new frmMain(dbService, authService, diskService, qrCodeService, googleDriveService, membershipService, cryptoPaymentService));
                    return;
                }
                AppConfig.ClearSession();
            }

            using (var loginForm = new frmLogin(dbService, authService))
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    CurrentUserId = loginForm.CurrentUserId;
                    Application.Run(new frmMain(dbService, authService, diskService, qrCodeService, googleDriveService, membershipService, cryptoPaymentService));
                }
            }
        }
    }
}
