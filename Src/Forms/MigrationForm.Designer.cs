namespace CemuUpdateTool.Forms
{
    partial class MigrationForm
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblDestVersionNr = new System.Windows.Forms.Label();
            this.lblSrcVersionNr = new System.Windows.Forms.Label();
            this.lblDestCemuVersion = new System.Windows.Forms.Label();
            this.lblSrcCemuVersion = new System.Windows.Forms.Label();
            this.btnOptions = new System.Windows.Forms.Button();
            this.btnSelectDestFolder = new System.Windows.Forms.Button();
            this.txtBoxDestFolder = new System.Windows.Forms.TextBox();
            this.lblDestFolder = new System.Windows.Forms.Label();
            this.btnSelectSrcFolder = new System.Windows.Forms.Button();
            this.txtBoxSrcFolder = new System.Windows.Forms.TextBox();
            this.lblSrcFolder = new System.Windows.Forms.Label();
            this.comboBoxVersion = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderFolders)).BeginInit();
            this.bottomPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(419, 189);
            // 
            // lblPercent
            // 
            this.lblPercent.Location = new System.Drawing.Point(471, 240);
            this.lblPercent.Size = new System.Drawing.Size(40, 15);
            // 
            // overallProgressBar
            // 
            this.overallProgressBar.Location = new System.Drawing.Point(16, 262);
            // 
            // lblCurrentTask
            // 
            this.lblCurrentTask.Location = new System.Drawing.Point(14, 240);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(326, 189);
            // 
            // lblDetails
            // 
            this.lblDetails.Location = new System.Drawing.Point(14, 310);
            this.lblDetails.Size = new System.Drawing.Size(494, 15);
            // 
            // txtBoxLog
            // 
            this.txtBoxLog.Location = new System.Drawing.Point(30, 331);
            this.txtBoxLog.Size = new System.Drawing.Size(478, 133);
            this.txtBoxLog.VisibleChanged += new System.EventHandler(this.ResizeFormOnLogTextboxVisibleChanged);
            // 
            // bottomPanel
            // 
            this.bottomPanel.Location = new System.Drawing.Point(0, 478);
            // 
            // lblDestVersionNr
            // 
            this.lblDestVersionNr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDestVersionNr.Location = new System.Drawing.Point(468, 130);
            this.lblDestVersionNr.Name = "lblDestVersionNr";
            this.lblDestVersionNr.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblDestVersionNr.Size = new System.Drawing.Size(40, 15);
            this.lblDestVersionNr.TabIndex = 12;
            this.lblDestVersionNr.Text = "    ";
            this.lblDestVersionNr.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblSrcVersionNr
            // 
            this.lblSrcVersionNr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSrcVersionNr.Location = new System.Drawing.Point(468, 75);
            this.lblSrcVersionNr.Name = "lblSrcVersionNr";
            this.lblSrcVersionNr.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblSrcVersionNr.Size = new System.Drawing.Size(40, 15);
            this.lblSrcVersionNr.TabIndex = 11;
            this.lblSrcVersionNr.Text = "    ";
            this.lblSrcVersionNr.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblDestCemuVersion
            // 
            this.lblDestCemuVersion.AutoSize = true;
            this.lblDestCemuVersion.Location = new System.Drawing.Point(387, 130);
            this.lblDestCemuVersion.Name = "lblDestCemuVersion";
            this.lblDestCemuVersion.Size = new System.Drawing.Size(83, 15);
            this.lblDestCemuVersion.TabIndex = 10;
            this.lblDestCemuVersion.Text = "Cemu version:";
            this.lblDestCemuVersion.Visible = false;
            // 
            // lblSrcCemuVersion
            // 
            this.lblSrcCemuVersion.AutoSize = true;
            this.lblSrcCemuVersion.Location = new System.Drawing.Point(387, 75);
            this.lblSrcCemuVersion.Name = "lblSrcCemuVersion";
            this.lblSrcCemuVersion.Size = new System.Drawing.Size(83, 15);
            this.lblSrcCemuVersion.TabIndex = 9;
            this.lblSrcCemuVersion.Text = "Cemu version:";
            this.lblSrcCemuVersion.Visible = false;
            // 
            // btnOptions
            // 
            this.btnOptions.Location = new System.Drawing.Point(16, 189);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(87, 27);
            this.btnOptions.TabIndex = 6;
            this.btnOptions.Text = "Options...";
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.OpenOptionsForm);
            // 
            // btnSelectDestFolder
            // 
            this.btnSelectDestFolder.Location = new System.Drawing.Point(419, 150);
            this.btnSelectDestFolder.Name = "btnSelectDestFolder";
            this.btnSelectDestFolder.Size = new System.Drawing.Size(89, 25);
            this.btnSelectDestFolder.TabIndex = 5;
            this.btnSelectDestFolder.Text = "Select";
            this.btnSelectDestFolder.UseVisualStyleBackColor = true;
            this.btnSelectDestFolder.Click += new System.EventHandler(this.SelectDestCemuFolder);
            // 
            // txtBoxDestFolder
            // 
            this.txtBoxDestFolder.AllowDrop = true;
            this.errProviderFolders.SetIconPadding(this.txtBoxDestFolder, -20);
            this.txtBoxDestFolder.Location = new System.Drawing.Point(16, 151);
            this.txtBoxDestFolder.Name = "txtBoxDestFolder";
            this.txtBoxDestFolder.Size = new System.Drawing.Size(397, 23);
            this.txtBoxDestFolder.TabIndex = 4;
            this.txtBoxDestFolder.TextChanged += new System.EventHandler(this.CheckDestFolderTextboxContent);
            this.txtBoxDestFolder.DragDrop += new System.Windows.Forms.DragEventHandler(this.TextboxDragDrop);
            this.txtBoxDestFolder.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextboxDragEnter);
            // 
            // lblDestFolder
            // 
            this.lblDestFolder.AutoSize = true;
            this.lblDestFolder.Location = new System.Drawing.Point(12, 130);
            this.lblDestFolder.Name = "lblDestFolder";
            this.lblDestFolder.Size = new System.Drawing.Size(136, 15);
            this.lblDestFolder.TabIndex = 3;
            this.lblDestFolder.Text = "Destination Cemu folder";
            // 
            // btnSelectSrcFolder
            // 
            this.btnSelectSrcFolder.Location = new System.Drawing.Point(419, 95);
            this.btnSelectSrcFolder.Name = "btnSelectSrcFolder";
            this.btnSelectSrcFolder.Size = new System.Drawing.Size(89, 25);
            this.btnSelectSrcFolder.TabIndex = 2;
            this.btnSelectSrcFolder.Text = "Select";
            this.btnSelectSrcFolder.UseVisualStyleBackColor = true;
            this.btnSelectSrcFolder.Click += new System.EventHandler(this.SelectSrcCemuFolder);
            // 
            // txtBoxSrcFolder
            // 
            this.txtBoxSrcFolder.AllowDrop = true;
            this.errProviderFolders.SetIconPadding(this.txtBoxSrcFolder, -20);
            this.txtBoxSrcFolder.Location = new System.Drawing.Point(16, 96);
            this.txtBoxSrcFolder.Name = "txtBoxSrcFolder";
            this.txtBoxSrcFolder.Size = new System.Drawing.Size(397, 23);
            this.txtBoxSrcFolder.TabIndex = 1;
            this.txtBoxSrcFolder.TextChanged += new System.EventHandler(this.CheckSrcFolderTextboxContent);
            this.txtBoxSrcFolder.DragDrop += new System.Windows.Forms.DragEventHandler(this.TextboxDragDrop);
            this.txtBoxSrcFolder.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextboxDragEnter);
            // 
            // lblSrcFolder
            // 
            this.lblSrcFolder.AutoSize = true;
            this.lblSrcFolder.Location = new System.Drawing.Point(12, 75);
            this.lblSrcFolder.Name = "lblSrcFolder";
            this.lblSrcFolder.Size = new System.Drawing.Size(112, 15);
            this.lblSrcFolder.TabIndex = 0;
            this.lblSrcFolder.Text = "Source Cemu folder";
            // 
            // comboBoxVersion
            // 
            this.comboBoxVersion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxVersion.Items.AddRange(new object[] {
            "Latest"});
            this.comboBoxVersion.Location = new System.Drawing.Point(446, 127);
            this.comboBoxVersion.MaxDropDownItems = 1;
            this.comboBoxVersion.MaxLength = 9;
            this.comboBoxVersion.Name = "comboBoxVersion";
            this.comboBoxVersion.Size = new System.Drawing.Size(62, 23);
            this.comboBoxVersion.TabIndex = 3;
            this.comboBoxVersion.Leave += new System.EventHandler(this.ParseSuppliedVersionInCombobox);
            // 
            // MigrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.ClientSize = new System.Drawing.Size(519, 524);
            this.Controls.Add(this.comboBoxVersion);
            this.Controls.Add(this.lblDestVersionNr);
            this.Controls.Add(this.lblSrcVersionNr);
            this.Controls.Add(this.lblSrcCemuVersion);
            this.Controls.Add(this.lblSrcFolder);
            this.Controls.Add(this.txtBoxSrcFolder);
            this.Controls.Add(this.btnSelectSrcFolder);
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.lblDestFolder);
            this.Controls.Add(this.btnSelectDestFolder);
            this.Controls.Add(this.txtBoxDestFolder);
            this.Controls.Add(this.lblDestCemuVersion);
            this.Name = "MigrationForm";
            this.Controls.SetChildIndex(this.lblDestCemuVersion, 0);
            this.Controls.SetChildIndex(this.txtBoxDestFolder, 0);
            this.Controls.SetChildIndex(this.btnSelectDestFolder, 0);
            this.Controls.SetChildIndex(this.lblDestFolder, 0);
            this.Controls.SetChildIndex(this.btnOptions, 0);
            this.Controls.SetChildIndex(this.btnSelectSrcFolder, 0);
            this.Controls.SetChildIndex(this.txtBoxSrcFolder, 0);
            this.Controls.SetChildIndex(this.lblSrcFolder, 0);
            this.Controls.SetChildIndex(this.lblSrcCemuVersion, 0);
            this.Controls.SetChildIndex(this.lblSrcVersionNr, 0);
            this.Controls.SetChildIndex(this.lblDestVersionNr, 0);
            this.Controls.SetChildIndex(this.btnStart, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.lblCurrentTask, 0);
            this.Controls.SetChildIndex(this.overallProgressBar, 0);
            this.Controls.SetChildIndex(this.lblPercent, 0);
            this.Controls.SetChildIndex(this.bottomPanel, 0);
            this.Controls.SetChildIndex(this.lblDetails, 0);
            this.Controls.SetChildIndex(this.txtBoxLog, 0);
            this.Controls.SetChildIndex(this.headerPanel, 0);
            this.Controls.SetChildIndex(this.comboBoxVersion, 0);
            ((System.ComponentModel.ISupportInitialize)(this.errProviderFolders)).EndInit();
            this.bottomPanel.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtBoxSrcFolder;
        private System.Windows.Forms.Label lblSrcFolder;
        private System.Windows.Forms.Button btnSelectSrcFolder;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.Button btnSelectDestFolder;
        private System.Windows.Forms.TextBox txtBoxDestFolder;
        private System.Windows.Forms.Label lblDestFolder;
        private System.Windows.Forms.Label lblDestCemuVersion;
        private System.Windows.Forms.Label lblSrcCemuVersion;
        private System.Windows.Forms.Label lblDestVersionNr;
        private System.Windows.Forms.Label lblSrcVersionNr;
        private System.Windows.Forms.ComboBox comboBoxVersion;
    }
}

