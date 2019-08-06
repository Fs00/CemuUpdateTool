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
            this.lblDestinationCemuVersionNumber = new System.Windows.Forms.Label();
            this.lblSourceCemuVersionNumber = new System.Windows.Forms.Label();
            this.lblDestinationCemuVersion = new System.Windows.Forms.Label();
            this.lblSourceCemuVersion = new System.Windows.Forms.Label();
            this.btnOptions = new System.Windows.Forms.Button();
            this.btnSelectDestinationFolder = new System.Windows.Forms.Button();
            this.txtBoxDestinationFolder = new System.Windows.Forms.TextBox();
            this.lblDestFolder = new System.Windows.Forms.Label();
            this.btnSelectSourceFolder = new System.Windows.Forms.Button();
            this.txtBoxSourceFolder = new System.Windows.Forms.TextBox();
            this.lblSrcFolder = new System.Windows.Forms.Label();
            this.comboBoxDestinationVersion = new System.Windows.Forms.ComboBox();
            this.flowPanelSource = new System.Windows.Forms.FlowLayoutPanel();
            this.flowPanelDestination = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderFolders)).BeginInit();
            this.bottomPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.flowPanelSource.SuspendLayout();
            this.flowPanelDestination.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(419, 189);
            // 
            // lblPercent
            // 
            this.lblPercent.Location = new System.Drawing.Point(468, 240);
            this.lblPercent.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
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
            this.txtBoxLog.VisibleChanged += new System.EventHandler(this.ResizeFormOnLogTextBoxVisibleChanged);
            // 
            // bottomPanel
            // 
            this.bottomPanel.Location = new System.Drawing.Point(0, 478);
            // 
            // lblDestinationCemuVersionNumber
            // 
            this.lblDestinationCemuVersionNumber.AutoSize = true;
            this.lblDestinationCemuVersionNumber.Location = new System.Drawing.Point(98, 3);
            this.lblDestinationCemuVersionNumber.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.lblDestinationCemuVersionNumber.Name = "lblDestinationCemuVersionNumber";
            this.lblDestinationCemuVersionNumber.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblDestinationCemuVersionNumber.Size = new System.Drawing.Size(19, 15);
            this.lblDestinationCemuVersionNumber.TabIndex = 12;
            this.lblDestinationCemuVersionNumber.Text = "    ";
            this.lblDestinationCemuVersionNumber.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblSourceCemuVersionNumber
            // 
            this.lblSourceCemuVersionNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSourceCemuVersionNumber.AutoSize = true;
            this.lblSourceCemuVersionNumber.Location = new System.Drawing.Point(163, 0);
            this.lblSourceCemuVersionNumber.Margin = new System.Windows.Forms.Padding(0);
            this.lblSourceCemuVersionNumber.Name = "lblSourceCemuVersionNumber";
            this.lblSourceCemuVersionNumber.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblSourceCemuVersionNumber.Size = new System.Drawing.Size(19, 15);
            this.lblSourceCemuVersionNumber.TabIndex = 11;
            this.lblSourceCemuVersionNumber.Text = "    ";
            this.lblSourceCemuVersionNumber.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblDestinationCemuVersion
            // 
            this.lblDestinationCemuVersion.AutoSize = true;
            this.lblDestinationCemuVersion.Location = new System.Drawing.Point(15, 3);
            this.lblDestinationCemuVersion.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.lblDestinationCemuVersion.Name = "lblDestinationCemuVersion";
            this.lblDestinationCemuVersion.Size = new System.Drawing.Size(83, 15);
            this.lblDestinationCemuVersion.TabIndex = 10;
            this.lblDestinationCemuVersion.Text = "Cemu version:";
            this.lblDestinationCemuVersion.Visible = false;
            // 
            // lblSourceCemuVersion
            // 
            this.lblSourceCemuVersion.AutoSize = true;
            this.lblSourceCemuVersion.Location = new System.Drawing.Point(80, 0);
            this.lblSourceCemuVersion.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.lblSourceCemuVersion.Name = "lblSourceCemuVersion";
            this.lblSourceCemuVersion.Size = new System.Drawing.Size(83, 15);
            this.lblSourceCemuVersion.TabIndex = 9;
            this.lblSourceCemuVersion.Text = "Cemu version:";
            this.lblSourceCemuVersion.Visible = false;
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
            // btnSelectDestinationFolder
            // 
            this.btnSelectDestinationFolder.Location = new System.Drawing.Point(419, 150);
            this.btnSelectDestinationFolder.Name = "btnSelectDestinationFolder";
            this.btnSelectDestinationFolder.Size = new System.Drawing.Size(89, 25);
            this.btnSelectDestinationFolder.TabIndex = 5;
            this.btnSelectDestinationFolder.Text = "Select";
            this.btnSelectDestinationFolder.UseVisualStyleBackColor = true;
            this.btnSelectDestinationFolder.Click += new System.EventHandler(this.SelectCemuFolder);
            // 
            // txtBoxDestinationFolder
            // 
            this.txtBoxDestinationFolder.AllowDrop = true;
            this.errProviderFolders.SetIconPadding(this.txtBoxDestinationFolder, -20);
            this.txtBoxDestinationFolder.Location = new System.Drawing.Point(16, 151);
            this.txtBoxDestinationFolder.Name = "txtBoxDestinationFolder";
            this.txtBoxDestinationFolder.Size = new System.Drawing.Size(397, 23);
            this.txtBoxDestinationFolder.TabIndex = 4;
            this.txtBoxDestinationFolder.TextChanged += new System.EventHandler(this.CheckDestinationFolderTextBoxContent);
            this.txtBoxDestinationFolder.DragDrop += new System.Windows.Forms.DragEventHandler(this.PasteContentIntoTextBoxOnDragDrop);
            this.txtBoxDestinationFolder.DragEnter += new System.Windows.Forms.DragEventHandler(this.ChangeCursorEffectOnTextBoxDragEnter);
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
            // btnSelectSourceFolder
            // 
            this.btnSelectSourceFolder.Location = new System.Drawing.Point(419, 95);
            this.btnSelectSourceFolder.Name = "btnSelectSourceFolder";
            this.btnSelectSourceFolder.Size = new System.Drawing.Size(89, 25);
            this.btnSelectSourceFolder.TabIndex = 2;
            this.btnSelectSourceFolder.Text = "Select";
            this.btnSelectSourceFolder.UseVisualStyleBackColor = true;
            this.btnSelectSourceFolder.Click += new System.EventHandler(this.SelectCemuFolder);
            // 
            // txtBoxSourceFolder
            // 
            this.txtBoxSourceFolder.AllowDrop = true;
            this.errProviderFolders.SetIconPadding(this.txtBoxSourceFolder, -20);
            this.txtBoxSourceFolder.Location = new System.Drawing.Point(16, 96);
            this.txtBoxSourceFolder.Name = "txtBoxSourceFolder";
            this.txtBoxSourceFolder.Size = new System.Drawing.Size(397, 23);
            this.txtBoxSourceFolder.TabIndex = 1;
            this.txtBoxSourceFolder.TextChanged += new System.EventHandler(this.CheckSourceFolderTextBoxContent);
            this.txtBoxSourceFolder.DragDrop += new System.Windows.Forms.DragEventHandler(this.PasteContentIntoTextBoxOnDragDrop);
            this.txtBoxSourceFolder.DragEnter += new System.Windows.Forms.DragEventHandler(this.ChangeCursorEffectOnTextBoxDragEnter);
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
            // comboBoxDestinationVersion
            // 
            this.comboBoxDestinationVersion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxDestinationVersion.Items.AddRange(new object[] {
            "Latest"});
            this.comboBoxDestinationVersion.Location = new System.Drawing.Point(119, 0);
            this.comboBoxDestinationVersion.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.comboBoxDestinationVersion.MaxDropDownItems = 1;
            this.comboBoxDestinationVersion.MaxLength = 9;
            this.comboBoxDestinationVersion.Name = "comboBoxDestinationVersion";
            this.comboBoxDestinationVersion.Size = new System.Drawing.Size(62, 23);
            this.comboBoxDestinationVersion.TabIndex = 3;
            this.comboBoxDestinationVersion.Leave += new System.EventHandler(this.ParseSuppliedVersionInCombobox);
            // 
            // flowPanelSource
            // 
            this.flowPanelSource.Controls.Add(this.lblSourceCemuVersionNumber);
            this.flowPanelSource.Controls.Add(this.lblSourceCemuVersion);
            this.flowPanelSource.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowPanelSource.Location = new System.Drawing.Point(326, 75);
            this.flowPanelSource.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.flowPanelSource.Name = "flowPanelSource";
            this.flowPanelSource.Size = new System.Drawing.Size(182, 15);
            this.flowPanelSource.TabIndex = 14;
            this.flowPanelSource.WrapContents = false;
            // 
            // flowPanelDestination
            // 
            this.flowPanelDestination.Controls.Add(this.comboBoxDestinationVersion);
            this.flowPanelDestination.Controls.Add(this.lblDestinationCemuVersionNumber);
            this.flowPanelDestination.Controls.Add(this.lblDestinationCemuVersion);
            this.flowPanelDestination.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowPanelDestination.Location = new System.Drawing.Point(327, 127);
            this.flowPanelDestination.Name = "flowPanelDestination";
            this.flowPanelDestination.Size = new System.Drawing.Size(181, 23);
            this.flowPanelDestination.TabIndex = 15;
            this.flowPanelDestination.WrapContents = false;
            // 
            // MigrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.ClientSize = new System.Drawing.Size(519, 524);
            this.Controls.Add(this.flowPanelDestination);
            this.Controls.Add(this.flowPanelSource);
            this.Controls.Add(this.lblSrcFolder);
            this.Controls.Add(this.txtBoxSourceFolder);
            this.Controls.Add(this.btnSelectSourceFolder);
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.lblDestFolder);
            this.Controls.Add(this.btnSelectDestinationFolder);
            this.Controls.Add(this.txtBoxDestinationFolder);
            this.Name = "MigrationForm";
            this.Controls.SetChildIndex(this.txtBoxDestinationFolder, 0);
            this.Controls.SetChildIndex(this.btnSelectDestinationFolder, 0);
            this.Controls.SetChildIndex(this.lblDestFolder, 0);
            this.Controls.SetChildIndex(this.btnOptions, 0);
            this.Controls.SetChildIndex(this.btnSelectSourceFolder, 0);
            this.Controls.SetChildIndex(this.txtBoxSourceFolder, 0);
            this.Controls.SetChildIndex(this.lblSrcFolder, 0);
            this.Controls.SetChildIndex(this.btnStart, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.lblCurrentTask, 0);
            this.Controls.SetChildIndex(this.overallProgressBar, 0);
            this.Controls.SetChildIndex(this.lblPercent, 0);
            this.Controls.SetChildIndex(this.bottomPanel, 0);
            this.Controls.SetChildIndex(this.lblDetails, 0);
            this.Controls.SetChildIndex(this.txtBoxLog, 0);
            this.Controls.SetChildIndex(this.headerPanel, 0);
            this.Controls.SetChildIndex(this.flowPanelSource, 0);
            this.Controls.SetChildIndex(this.flowPanelDestination, 0);
            ((System.ComponentModel.ISupportInitialize)(this.errProviderFolders)).EndInit();
            this.bottomPanel.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.flowPanelSource.ResumeLayout(false);
            this.flowPanelSource.PerformLayout();
            this.flowPanelDestination.ResumeLayout(false);
            this.flowPanelDestination.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtBoxSourceFolder;
        private System.Windows.Forms.Label lblSrcFolder;
        private System.Windows.Forms.Button btnSelectSourceFolder;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.Button btnSelectDestinationFolder;
        private System.Windows.Forms.TextBox txtBoxDestinationFolder;
        private System.Windows.Forms.Label lblDestFolder;
        private System.Windows.Forms.Label lblDestinationCemuVersion;
        private System.Windows.Forms.Label lblSourceCemuVersion;
        private System.Windows.Forms.Label lblDestinationCemuVersionNumber;
        private System.Windows.Forms.Label lblSourceCemuVersionNumber;
        private System.Windows.Forms.ComboBox comboBoxDestinationVersion;
        private System.Windows.Forms.FlowLayoutPanel flowPanelSource;
        private System.Windows.Forms.FlowLayoutPanel flowPanelDestination;
    }
}

