namespace CemuUpdateTool
{
    partial class UpdateForm
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
            this.lblVersionNr = new System.Windows.Forms.Label();
            this.lblCemuVersion = new System.Windows.Forms.Label();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.txtBoxCemuFolder = new System.Windows.Forms.TextBox();
            this.lblCemuFolder = new System.Windows.Forms.Label();
            this.chkBoxUpdGameProfiles = new System.Windows.Forms.CheckBox();
            this.chkBoxDeletePrecompiled = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderFolders)).BeginInit();
            this.bottomPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(433, 133);
            // 
            // lblPercent
            // 
            this.lblPercent.Location = new System.Drawing.Point(485, 174);
            this.lblPercent.Size = new System.Drawing.Size(40, 15);
            // 
            // overallProgressBar
            // 
            this.overallProgressBar.Location = new System.Drawing.Point(16, 196);
            this.overallProgressBar.Size = new System.Drawing.Size(506, 35);
            // 
            // lblCurrentTask
            // 
            this.lblCurrentTask.Location = new System.Drawing.Point(14, 174);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(435, 10);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(340, 133);
            // 
            // lblDetails
            // 
            this.lblDetails.Location = new System.Drawing.Point(14, 244);
            this.lblDetails.Size = new System.Drawing.Size(508, 15);
            // 
            // txtBoxLog
            // 
            this.txtBoxLog.Location = new System.Drawing.Point(30, 265);
            this.txtBoxLog.Size = new System.Drawing.Size(492, 133);
            this.txtBoxLog.VisibleChanged += new System.EventHandler(this.ResizeFormOnLogTextboxVisibleChanged);
            // 
            // bottomPanel
            // 
            this.bottomPanel.Location = new System.Drawing.Point(0, 412);
            this.bottomPanel.Size = new System.Drawing.Size(537, 46);
            // 
            // pnlBorderBottom
            // 
            this.pnlBorderBottom.Size = new System.Drawing.Size(540, 1);
            // 
            // headerPanel
            // 
            this.headerPanel.Size = new System.Drawing.Size(537, 64);
            // 
            // pnlBorderTop
            // 
            this.pnlBorderTop.Size = new System.Drawing.Size(540, 1);
            // 
            // lblTitle
            // 
            this.lblTitle.Size = new System.Drawing.Size(52, 17);
            this.lblTitle.Text = "Update";
            // 
            // lblDescription
            // 
            this.lblDescription.Size = new System.Drawing.Size(496, 15);
            this.lblDescription.Text = "Select the folder that contains the Cemu installation you want to update and then" +
    " press Start.";
            // 
            // lblVersionNr
            // 
            this.lblVersionNr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVersionNr.Location = new System.Drawing.Point(482, 75);
            this.lblVersionNr.Name = "lblVersionNr";
            this.lblVersionNr.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblVersionNr.Size = new System.Drawing.Size(40, 15);
            this.lblVersionNr.TabIndex = 11;
            this.lblVersionNr.Text = "    ";
            this.lblVersionNr.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblCemuVersion
            // 
            this.lblCemuVersion.AutoSize = true;
            this.lblCemuVersion.Location = new System.Drawing.Point(403, 75);
            this.lblCemuVersion.Name = "lblCemuVersion";
            this.lblCemuVersion.Size = new System.Drawing.Size(83, 15);
            this.lblCemuVersion.TabIndex = 9;
            this.lblCemuVersion.Text = "Cemu version:";
            this.lblCemuVersion.Visible = false;
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Location = new System.Drawing.Point(433, 95);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(89, 25);
            this.btnSelectFolder.TabIndex = 2;
            this.btnSelectFolder.Text = "Select";
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new System.EventHandler(this.SelectCemuFolder);
            // 
            // txtBoxCemuFolder
            // 
            this.txtBoxCemuFolder.AllowDrop = true;
            this.errProviderFolders.SetIconPadding(this.txtBoxCemuFolder, -20);
            this.txtBoxCemuFolder.Location = new System.Drawing.Point(16, 96);
            this.txtBoxCemuFolder.Name = "txtBoxCemuFolder";
            this.txtBoxCemuFolder.Size = new System.Drawing.Size(411, 23);
            this.txtBoxCemuFolder.TabIndex = 1;
            this.txtBoxCemuFolder.TextChanged += new System.EventHandler(this.CheckFolderTextboxContent);
            this.txtBoxCemuFolder.DragDrop += new System.Windows.Forms.DragEventHandler(this.TextboxDragDrop);
            this.txtBoxCemuFolder.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextboxDragEnter);
            // 
            // lblCemuFolder
            // 
            this.lblCemuFolder.AutoSize = true;
            this.lblCemuFolder.Location = new System.Drawing.Point(12, 75);
            this.lblCemuFolder.Name = "lblCemuFolder";
            this.lblCemuFolder.Size = new System.Drawing.Size(73, 15);
            this.lblCemuFolder.TabIndex = 0;
            this.lblCemuFolder.Text = "Cemu folder";
            // 
            // chkBoxUpdGameProfiles
            // 
            this.chkBoxUpdGameProfiles.AutoSize = true;
            this.chkBoxUpdGameProfiles.Location = new System.Drawing.Point(15, 138);
            this.chkBoxUpdGameProfiles.Name = "chkBoxUpdGameProfiles";
            this.chkBoxUpdGameProfiles.Size = new System.Drawing.Size(139, 19);
            this.chkBoxUpdGameProfiles.TabIndex = 3;
            this.chkBoxUpdGameProfiles.Text = "Update game profiles";
            this.chkBoxUpdGameProfiles.UseVisualStyleBackColor = true;
            // 
            // chkBoxDeletePrecompiled
            // 
            this.chkBoxDeletePrecompiled.AutoSize = true;
            this.chkBoxDeletePrecompiled.Location = new System.Drawing.Point(160, 138);
            this.chkBoxDeletePrecompiled.Name = "chkBoxDeletePrecompiled";
            this.chkBoxDeletePrecompiled.Size = new System.Drawing.Size(168, 19);
            this.chkBoxDeletePrecompiled.TabIndex = 4;
            this.chkBoxDeletePrecompiled.Text = "Delete precompiled caches";
            this.chkBoxDeletePrecompiled.UseVisualStyleBackColor = true;
            // 
            // UpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.ClientSize = new System.Drawing.Size(537, 458);
            this.Controls.Add(this.chkBoxDeletePrecompiled);
            this.Controls.Add(this.chkBoxUpdGameProfiles);
            this.Controls.Add(this.lblVersionNr);
            this.Controls.Add(this.lblCemuVersion);
            this.Controls.Add(this.lblCemuFolder);
            this.Controls.Add(this.txtBoxCemuFolder);
            this.Controls.Add(this.btnSelectFolder);
            this.Name = "UpdateForm";
            this.Controls.SetChildIndex(this.btnSelectFolder, 0);
            this.Controls.SetChildIndex(this.txtBoxCemuFolder, 0);
            this.Controls.SetChildIndex(this.lblCemuFolder, 0);
            this.Controls.SetChildIndex(this.lblCemuVersion, 0);
            this.Controls.SetChildIndex(this.lblVersionNr, 0);
            this.Controls.SetChildIndex(this.btnStart, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.lblCurrentTask, 0);
            this.Controls.SetChildIndex(this.overallProgressBar, 0);
            this.Controls.SetChildIndex(this.lblPercent, 0);
            this.Controls.SetChildIndex(this.bottomPanel, 0);
            this.Controls.SetChildIndex(this.lblDetails, 0);
            this.Controls.SetChildIndex(this.txtBoxLog, 0);
            this.Controls.SetChildIndex(this.headerPanel, 0);
            this.Controls.SetChildIndex(this.chkBoxUpdGameProfiles, 0);
            this.Controls.SetChildIndex(this.chkBoxDeletePrecompiled, 0);
            ((System.ComponentModel.ISupportInitialize)(this.errProviderFolders)).EndInit();
            this.bottomPanel.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtBoxCemuFolder;
        private System.Windows.Forms.Label lblCemuFolder;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.Label lblCemuVersion;
        private System.Windows.Forms.Label lblVersionNr;
        private System.Windows.Forms.CheckBox chkBoxUpdGameProfiles;
        private System.Windows.Forms.CheckBox chkBoxDeletePrecompiled;
    }
}