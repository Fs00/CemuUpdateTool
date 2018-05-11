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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.lnklblOpts = new System.Windows.Forms.LinkLabel();
            this.lnklblAbout = new System.Windows.Forms.LinkLabel();
            this.lnkLblHelp = new System.Windows.Forms.LinkLabel();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.pnlBorder = new System.Windows.Forms.Label();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(97, 100);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(140, 138);
            this.button1.TabIndex = 0;
            this.button1.Text = "Migrate";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ShowMigrateForm);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(309, 100);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(140, 138);
            this.button2.TabIndex = 1;
            this.button2.Text = "Download and Migrate";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.ShowDownloadMigrateForm);
            // 
            // lnklblOpts
            // 
            this.lnklblOpts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnklblOpts.AutoSize = true;
            this.lnklblOpts.Location = new System.Drawing.Point(293, 15);
            this.lnklblOpts.Name = "lnklblOpts";
            this.lnklblOpts.Size = new System.Drawing.Size(49, 15);
            this.lnklblOpts.TabIndex = 2;
            this.lnklblOpts.TabStop = true;
            this.lnklblOpts.Text = "Options";
            this.lnklblOpts.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ShowOptionsForm);
            // 
            // lnklblAbout
            // 
            this.lnklblAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnklblAbout.AutoSize = true;
            this.lnklblAbout.Location = new System.Drawing.Point(436, 15);
            this.lnklblAbout.Name = "lnklblAbout";
            this.lnklblAbout.Size = new System.Drawing.Size(49, 15);
            this.lnklblAbout.TabIndex = 3;
            this.lnklblAbout.TabStop = true;
            this.lnklblAbout.Text = "About...";
            this.lnklblAbout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ShowAboutForm);
            // 
            // lnkLblHelp
            // 
            this.lnkLblHelp.AutoSize = true;
            this.lnkLblHelp.Location = new System.Drawing.Point(372, 15);
            this.lnkLblHelp.Name = "lnkLblHelp";
            this.lnkLblHelp.Size = new System.Drawing.Size(35, 15);
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
            this.bottomPanel.Location = new System.Drawing.Point(0, 317);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(516, 46);
            this.bottomPanel.TabIndex = 5;
            // 
            // pnlBorder
            // 
            this.pnlBorder.BackColor = System.Drawing.SystemColors.Control;
            this.pnlBorder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlBorder.Location = new System.Drawing.Point(-7, 317);
            this.pnlBorder.Name = "pnlBorder";
            this.pnlBorder.Size = new System.Drawing.Size(530, 2);
            this.pnlBorder.TabIndex = 14;
            // 
            // HomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(516, 363);
            this.Controls.Add(this.pnlBorder);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "HomeForm";
            this.Text = "HomeForm";
            this.bottomPanel.ResumeLayout(false);
            this.bottomPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.LinkLabel lnklblOpts;
        private System.Windows.Forms.LinkLabel lnklblAbout;
        private System.Windows.Forms.LinkLabel lnkLblHelp;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Label pnlBorder;
    }
}