namespace TagMyDrive
{
    partial class frmMembership
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
            this.lblCurrentPlan = new System.Windows.Forms.Label();
            this.lblCurrentDesc = new System.Windows.Forms.Label();
            this.lblCurrentLimits = new System.Windows.Forms.Label();
            this.lblSelectPlan = new System.Windows.Forms.Label();
            this.cmbPlans = new System.Windows.Forms.ComboBox();
            this.lblPlanDetails = new System.Windows.Forms.Label();
            this.panelPayment = new System.Windows.Forms.Panel();
            this.lblPayAmount = new System.Windows.Forms.Label();
            this.btnShowQR = new System.Windows.Forms.Button();
            this.lblTxHash = new System.Windows.Forms.Label();
            this.txtTxHash = new System.Windows.Forms.TextBox();
            this.lblPaymentNote = new System.Windows.Forms.Label();
            this.btnUpgrade = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelPayment.SuspendLayout();
            this.SuspendLayout();

            this.lblCurrentPlan.AutoSize = true;
            this.lblCurrentPlan.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblCurrentPlan.Location = new System.Drawing.Point(15, 15);
            this.lblCurrentPlan.Size = new System.Drawing.Size(300, 25);

            this.lblCurrentDesc.AutoSize = true;
            this.lblCurrentDesc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCurrentDesc.ForeColor = System.Drawing.Color.Gray;
            this.lblCurrentDesc.Location = new System.Drawing.Point(15, 45);
            this.lblCurrentDesc.Size = new System.Drawing.Size(400, 15);

            this.lblCurrentLimits.AutoSize = true;
            this.lblCurrentLimits.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCurrentLimits.Location = new System.Drawing.Point(15, 65);
            this.lblCurrentLimits.Size = new System.Drawing.Size(400, 15);

            this.lblSelectPlan.AutoSize = true;
            this.lblSelectPlan.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblSelectPlan.Location = new System.Drawing.Point(15, 100);
            this.lblSelectPlan.Size = new System.Drawing.Size(200, 19);
            this.lblSelectPlan.Text = "Upgrade to:";

            this.cmbPlans.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPlans.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbPlans.Location = new System.Drawing.Point(15, 125);
            this.cmbPlans.Size = new System.Drawing.Size(440, 25);
            this.cmbPlans.SelectedIndexChanged += new System.EventHandler(this.cmbPlans_SelectedIndexChanged);

            this.lblPlanDetails.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPlanDetails.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            this.lblPlanDetails.Location = new System.Drawing.Point(15, 155);
            this.lblPlanDetails.Size = new System.Drawing.Size(440, 50);

            this.panelPayment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPayment.Location = new System.Drawing.Point(15, 215);
            this.panelPayment.Size = new System.Drawing.Size(440, 130);

            this.lblPayAmount.AutoSize = true;
            this.lblPayAmount.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblPayAmount.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblPayAmount.Location = new System.Drawing.Point(10, 10);
            this.lblPayAmount.Size = new System.Drawing.Size(300, 20);

            this.btnShowQR.Location = new System.Drawing.Point(10, 40);
            this.btnShowQR.Size = new System.Drawing.Size(150, 30);
            this.btnShowQR.Text = "Show Payment QR";
            this.btnShowQR.UseVisualStyleBackColor = true;
            this.btnShowQR.Click += new System.EventHandler(this.btnShowQR_Click);

            this.lblTxHash.AutoSize = true;
            this.lblTxHash.Location = new System.Drawing.Point(10, 78);
            this.lblTxHash.Size = new System.Drawing.Size(200, 15);
            this.lblTxHash.Text = "Transaction Hash:";

            this.txtTxHash.Location = new System.Drawing.Point(10, 98);
            this.txtTxHash.Size = new System.Drawing.Size(415, 23);
            this.txtTxHash.Font = new System.Drawing.Font("Segoe UI", 9F);

            this.panelPayment.Controls.Add(this.lblPayAmount);
            this.panelPayment.Controls.Add(this.btnShowQR);
            this.panelPayment.Controls.Add(this.lblTxHash);
            this.panelPayment.Controls.Add(this.txtTxHash);

            this.lblPaymentNote.AutoSize = true;
            this.lblPaymentNote.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblPaymentNote.ForeColor = System.Drawing.Color.Gray;
            this.lblPaymentNote.Location = new System.Drawing.Point(15, 355);
            this.lblPaymentNote.Size = new System.Drawing.Size(440, 30);
            this.lblPaymentNote.Text = "Send USDT (BEP20) to the address shown, enter your transaction hash, and click Upgrade.\nPayment is verified automatically via BSCScan - no waiting for admin.";

            this.btnUpgrade.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnUpgrade.Location = new System.Drawing.Point(15, 395);
            this.btnUpgrade.Size = new System.Drawing.Size(150, 35);
            this.btnUpgrade.Text = "Submit Upgrade";
            this.btnUpgrade.UseVisualStyleBackColor = true;
            this.btnUpgrade.Click += new System.EventHandler(this.btnUpgrade_Click);

            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(320, 395);
            this.btnClose.Size = new System.Drawing.Size(135, 35);
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(470, 445);
            this.Controls.Add(this.lblCurrentPlan);
            this.Controls.Add(this.lblCurrentDesc);
            this.Controls.Add(this.lblCurrentLimits);
            this.Controls.Add(this.lblSelectPlan);
            this.Controls.Add(this.cmbPlans);
            this.Controls.Add(this.lblPlanDetails);
            this.Controls.Add(this.panelPayment);
            this.Controls.Add(this.lblPaymentNote);
            this.Controls.Add(this.btnUpgrade);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "frmMembership";
            this.Text = "Membership";
            this.Load += new System.EventHandler(this.frmMembership_Load);
            this.panelPayment.ResumeLayout(false);
            this.panelPayment.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblCurrentPlan;
        private System.Windows.Forms.Label lblCurrentDesc;
        private System.Windows.Forms.Label lblCurrentLimits;
        private System.Windows.Forms.Label lblSelectPlan;
        private System.Windows.Forms.ComboBox cmbPlans;
        private System.Windows.Forms.Label lblPlanDetails;
        private System.Windows.Forms.Panel panelPayment;
        private System.Windows.Forms.Label lblPayAmount;
        private System.Windows.Forms.Button btnShowQR;
        private System.Windows.Forms.Label lblTxHash;
        private System.Windows.Forms.TextBox txtTxHash;
        private System.Windows.Forms.Label lblPaymentNote;
        private System.Windows.Forms.Button btnUpgrade;
        private System.Windows.Forms.Button btnClose;
    }
}
