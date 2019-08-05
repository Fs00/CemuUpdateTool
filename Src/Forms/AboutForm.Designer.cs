namespace CemuUpdateTool.Forms
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
            this.lblProgramName = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.linkLblMadeBy = new System.Windows.Forms.LinkLabel();
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
            // lblProgramName
            // 
            this.lblProgramName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblProgramName.AutoSize = true;
            this.lblProgramName.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgramName.Location = new System.Drawing.Point(112, 11);
            this.lblProgramName.Name = "lblProgramName";
            this.lblProgramName.Size = new System.Drawing.Size(122, 22);
            this.lblProgramName.TabIndex = 1;
            this.lblProgramName.Text = "Cemu Update Tool";
            this.lblProgramName.UseCompatibleTextRendering = true;
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
            // linkLblMadeBy
            // 
            this.linkLblMadeBy.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.linkLblMadeBy.AutoSize = true;
            this.linkLblMadeBy.LinkArea = new System.Windows.Forms.LinkArea(29, 0);
            this.linkLblMadeBy.Location = new System.Drawing.Point(112, 59);
            this.linkLblMadeBy.Name = "linkLblMadeBy";
            this.linkLblMadeBy.Size = new System.Drawing.Size(176, 21);
            this.linkLblMadeBy.TabIndex = 3;
            this.linkLblMadeBy.Text = "Created and developed by Fs00";
            this.linkLblMadeBy.UseCompatibleTextRendering = true;
            this.linkLblMadeBy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelClicked);
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
            this.linkLblDonate.Size = new System.Drawing.Size(223, 21);
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
            this.Controls.Add(this.linkLblMadeBy);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblProgramName);
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
        private System.Windows.Forms.Label lblProgramName;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.LinkLabel linkLblMadeBy;
        private System.Windows.Forms.LinkLabel linklblForum;
        private System.Windows.Forms.LinkLabel linkLblDonate;
    }
}