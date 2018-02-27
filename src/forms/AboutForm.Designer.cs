namespace CemuUpdateTool
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.pictBoxInfo = new System.Windows.Forms.PictureBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblCredit = new System.Windows.Forms.Label();
            this.lblUpdates = new System.Windows.Forms.Label();
            this.PageLinkLbl = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // pictBoxInfo
            // 
            this.pictBoxInfo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictBoxInfo.BackgroundImage")));
            this.pictBoxInfo.Location = new System.Drawing.Point(18, 31);
            this.pictBoxInfo.Name = "pictBoxInfo";
            this.pictBoxInfo.Size = new System.Drawing.Size(64, 64);
            this.pictBoxInfo.TabIndex = 0;
            this.pictBoxInfo.TabStop = false;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(98, 24);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(112, 13);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Cemu Update Tool";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(104, 46);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(112, 13);
            this.lblVersion.TabIndex = 2;
            this.lblVersion.Text = $"Version {Program.Version}";
            // 
            // lblCredit
            // 
            this.lblCredit.AutoSize = true;
            this.lblCredit.Location = new System.Drawing.Point(104, 66);
            this.lblCredit.Name = "lblCredit";
            this.lblCredit.Size = new System.Drawing.Size(158, 13);
            this.lblCredit.TabIndex = 3;
            this.lblCredit.Text = "Created and developed by Fs00";
            // 
            // lblUpdates
            // 
            this.lblUpdates.AutoSize = true;
            this.lblUpdates.Location = new System.Drawing.Point(103, 86);
            this.lblUpdates.Name = "lblUpdates";
            this.lblUpdates.Size = new System.Drawing.Size(184, 13);
            this.lblUpdates.TabIndex = 4;
            this.lblUpdates.Text = "For updates and news, check out the";
            // 
            // PageLinkLbl
            // 
            this.PageLinkLbl.AutoSize = true;
            this.PageLinkLbl.Location = new System.Drawing.Point(283, 86);
            this.PageLinkLbl.Name = "PageLinkLbl";
            this.PageLinkLbl.Size = new System.Drawing.Size(96, 13);
            this.PageLinkLbl.TabIndex = 5;
            this.PageLinkLbl.TabStop = true;
            this.PageLinkLbl.Text = "Cemu forum thread";
            this.PageLinkLbl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelClicked);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 126);
            this.Controls.Add(this.PageLinkLbl);
            this.Controls.Add(this.lblUpdates);
            this.Controls.Add(this.lblCredit);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.pictBoxInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AboutForm";
            this.Text = "About";
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxInfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictBoxInfo;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblCredit;
        private System.Windows.Forms.Label lblUpdates;
        private System.Windows.Forms.LinkLabel PageLinkLbl;
    }
}