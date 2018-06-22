namespace CemuUpdateTool
{
    partial class HomeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HomeForm));
            this.btnMigrate = new System.Windows.Forms.Button();
            this.btnDlMigrate = new System.Windows.Forms.Button();
            this.lnklblOpts = new System.Windows.Forms.LinkLabel();
            this.lnklblAbout = new System.Windows.Forms.LinkLabel();
            this.lnkLblHelp = new System.Windows.Forms.LinkLabel();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.pnlBorder = new System.Windows.Forms.Label();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnMigrate
            // 
            this.btnMigrate.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnMigrate.FlatAppearance.BorderSize = 2;
            this.btnMigrate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMigrate.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMigrate.Image = ((System.Drawing.Image)(resources.GetObject("btnMigrate.Image")));
            this.btnMigrate.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnMigrate.Location = new System.Drawing.Point(78, 86);
            this.btnMigrate.Name = "btnMigrate";
            this.btnMigrate.Size = new System.Drawing.Size(148, 153);
            this.btnMigrate.TabIndex = 1;
            this.btnMigrate.Text = "Migrate";
            this.btnMigrate.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnMigrate.UseMnemonic = false;
            this.btnMigrate.UseVisualStyleBackColor = true;
            this.btnMigrate.Click += new System.EventHandler(this.ShowMigrateForm);
            // 
            // btnDlMigrate
            // 
            this.btnDlMigrate.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnDlMigrate.FlatAppearance.BorderSize = 2;
            this.btnDlMigrate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDlMigrate.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDlMigrate.Image = ((System.Drawing.Image)(resources.GetObject("btnDlMigrate.Image")));
            this.btnDlMigrate.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnDlMigrate.Location = new System.Drawing.Point(289, 86);
            this.btnDlMigrate.Name = "btnDlMigrate";
            this.btnDlMigrate.Size = new System.Drawing.Size(148, 153);
            this.btnDlMigrate.TabIndex = 2;
            this.btnDlMigrate.Text = "Download & Migrate";
            this.btnDlMigrate.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnDlMigrate.UseMnemonic = false;
            this.btnDlMigrate.UseVisualStyleBackColor = true;
            this.btnDlMigrate.Click += new System.EventHandler(this.ShowDownloadMigrateForm);
            // 
            // lnklblOpts
            // 
            this.lnklblOpts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnklblOpts.AutoSize = true;
            this.lnklblOpts.Location = new System.Drawing.Point(281, 15);
            this.lnklblOpts.Name = "lnklblOpts";
            this.lnklblOpts.Size = new System.Drawing.Size(43, 13);
            this.lnklblOpts.TabIndex = 3;
            this.lnklblOpts.TabStop = true;
            this.lnklblOpts.Text = "Options";
            this.lnklblOpts.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ShowOptionsForm);
            // 
            // lnklblAbout
            // 
            this.lnklblAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnklblAbout.AutoSize = true;
            this.lnklblAbout.Location = new System.Drawing.Point(424, 15);
            this.lnklblAbout.Name = "lnklblAbout";
            this.lnklblAbout.Size = new System.Drawing.Size(44, 13);
            this.lnklblAbout.TabIndex = 5;
            this.lnklblAbout.TabStop = true;
            this.lnklblAbout.Text = "About...";
            this.lnklblAbout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ShowAboutForm);
            // 
            // lnkLblHelp
            // 
            this.lnkLblHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkLblHelp.AutoSize = true;
            this.lnkLblHelp.Location = new System.Drawing.Point(361, 15);
            this.lnkLblHelp.Name = "lnkLblHelp";
            this.lnkLblHelp.Size = new System.Drawing.Size(32, 13);
            this.lnkLblHelp.TabIndex = 4;
            this.lnkLblHelp.TabStop = true;
            this.lnkLblHelp.Text = "Help!";
            this.lnkLblHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ShowHelpForm);
            // 
            // bottomPanel
            // 
            this.bottomPanel.BackColor = System.Drawing.SystemColors.Control;
            this.bottomPanel.Controls.Add(this.lnklblOpts);
            this.bottomPanel.Controls.Add(this.lnkLblHelp);
            this.bottomPanel.Controls.Add(this.lnklblAbout);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 325);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(504, 46);
            this.bottomPanel.TabIndex = 0;
            // 
            // pnlBorder
            // 
            this.pnlBorder.BackColor = System.Drawing.SystemColors.Control;
            this.pnlBorder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlBorder.Location = new System.Drawing.Point(-7, 325);
            this.pnlBorder.Name = "pnlBorder";
            this.pnlBorder.Size = new System.Drawing.Size(530, 2);
            this.pnlBorder.TabIndex = 14;
            // 
            // HomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(504, 371);
            this.Controls.Add(this.pnlBorder);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.btnDlMigrate);
            this.Controls.Add(this.btnMigrate);
            this.Name = "HomeForm";
            this.Text = "HomeForm";
            this.bottomPanel.ResumeLayout(false);
            this.bottomPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnMigrate;
        private System.Windows.Forms.Button btnDlMigrate;
        private System.Windows.Forms.LinkLabel lnklblOpts;
        private System.Windows.Forms.LinkLabel lnklblAbout;
        private System.Windows.Forms.LinkLabel lnkLblHelp;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Label pnlBorder;
    }
}