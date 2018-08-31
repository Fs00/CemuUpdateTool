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
            this.linklblForum = new System.Windows.Forms.LinkLabel();
            this.linkLblDonate = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // pictBoxInfo
            // 
            this.pictBoxInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictBoxInfo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictBoxInfo.BackgroundImage")));
            this.pictBoxInfo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictBoxInfo.Location = new System.Drawing.Point(19, 16);
            this.pictBoxInfo.Name = "pictBoxInfo";
            this.pictBoxInfo.Size = new System.Drawing.Size(72, 89);
            this.pictBoxInfo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictBoxInfo.TabIndex = 0;
            this.pictBoxInfo.TabStop = false;
            // 
            // lblName
            // 
            this.lblName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(112, 11);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(122, 22);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Cemu Update Tool";
            this.lblName.UseCompatibleTextRendering = true;
            // 
            // lblVersion
            // 
            this.lblVersion.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(112, 31);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(49, 21);
            this.lblVersion.TabIndex = 2;
            this.lblVersion.Text = "Version ";
            this.lblVersion.UseCompatibleTextRendering = true;
            // 
            // lblCredit
            // 
            this.lblCredit.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCredit.AutoSize = true;
            this.lblCredit.Location = new System.Drawing.Point(112, 59);
            this.lblCredit.Name = "lblCredit";
            this.lblCredit.Size = new System.Drawing.Size(176, 21);
            this.lblCredit.TabIndex = 3;
            this.lblCredit.Text = "Created and developed by Fs00";
            this.lblCredit.UseCompatibleTextRendering = true;
            // 
            // linklblForum
            // 
            this.linklblForum.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.linklblForum.AutoSize = true;
            this.linklblForum.LinkArea = new System.Windows.Forms.LinkArea(57, 0);
            this.linklblForum.Location = new System.Drawing.Point(112, 77);
            this.linklblForum.Name = "linklblForum";
            this.linklblForum.Size = new System.Drawing.Size(319, 21);
            this.linklblForum.TabIndex = 5;
            this.linklblForum.Text = "For updates and news, check out the official forum thread";
            this.linklblForum.UseCompatibleTextRendering = true;
            this.linklblForum.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelClicked);
            // 
            // linkLblDonate
            // 
            this.linkLblDonate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.linkLblDonate.AutoSize = true;
            this.linkLblDonate.LinkArea = new System.Windows.Forms.LinkArea(37, 0);
            this.linkLblDonate.Location = new System.Drawing.Point(112, 95);
            this.linkLblDonate.Name = "linkLblDonate";
            this.linkLblDonate.Size = new System.Drawing.Size(209, 21);
            this.linkLblDonate.TabIndex = 7;
            this.linkLblDonate.Text = "If you want to say thanks, donate to me!";
            this.linkLblDonate.UseCompatibleTextRendering = true;
            this.linkLblDonate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelClicked);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(442, 124);
            this.Controls.Add(this.linkLblDonate);
            this.Controls.Add(this.linklblForum);
            this.Controls.Add(this.lblCredit);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.pictBoxInfo);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
        private System.Windows.Forms.LinkLabel linklblForum;
        private System.Windows.Forms.LinkLabel linkLblDonate;
    }
}