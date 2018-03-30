namespace CemuUpdateTool
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
            this.components = new System.ComponentModel.Container();
            this.grpBoxFolderSelect = new System.Windows.Forms.GroupBox();
            this.lblNewVersionNr = new System.Windows.Forms.Label();
            this.lblOldVersionNr = new System.Windows.Forms.Label();
            this.lblNewCemuVersion = new System.Windows.Forms.Label();
            this.lblOldCemuVersion = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnOptions = new System.Windows.Forms.Button();
            this.btnSelectNewFolder = new System.Windows.Forms.Button();
            this.txtBoxNewFolder = new System.Windows.Forms.TextBox();
            this.lblNewFolder = new System.Windows.Forms.Label();
            this.btnSelectOldFolder = new System.Windows.Forms.Button();
            this.txtBoxOldFolder = new System.Windows.Forms.TextBox();
            this.lblOldFolder = new System.Windows.Forms.Label();
            this.grpBoxProgress = new System.Windows.Forms.GroupBox();
            this.lblPercent = new System.Windows.Forms.Label();
            this.overallProgressBar = new System.Windows.Forms.ProgressBar();
            this.lblCurrentTask = new System.Windows.Forms.Label();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblVersion = new System.Windows.Forms.Label();
            this.errProviderNewFolder = new System.Windows.Forms.ErrorProvider(this.components);
            this.errProviderOldFolder = new System.Windows.Forms.ErrorProvider(this.components);
            this.lblDetails = new System.Windows.Forms.Label();
            this.txtBoxLog = new System.Windows.Forms.TextBox();
            this.grpBoxFolderSelect.SuspendLayout();
            this.grpBoxProgress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderNewFolder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderOldFolder)).BeginInit();
            this.SuspendLayout();
            // 
            // grpBoxFolderSelect
            // 
            this.grpBoxFolderSelect.Controls.Add(this.lblNewVersionNr);
            this.grpBoxFolderSelect.Controls.Add(this.lblOldVersionNr);
            this.grpBoxFolderSelect.Controls.Add(this.lblNewCemuVersion);
            this.grpBoxFolderSelect.Controls.Add(this.lblOldCemuVersion);
            this.grpBoxFolderSelect.Controls.Add(this.btnCancel);
            this.grpBoxFolderSelect.Controls.Add(this.btnStart);
            this.grpBoxFolderSelect.Controls.Add(this.btnOptions);
            this.grpBoxFolderSelect.Controls.Add(this.btnSelectNewFolder);
            this.grpBoxFolderSelect.Controls.Add(this.txtBoxNewFolder);
            this.grpBoxFolderSelect.Controls.Add(this.lblNewFolder);
            this.grpBoxFolderSelect.Controls.Add(this.btnSelectOldFolder);
            this.grpBoxFolderSelect.Controls.Add(this.txtBoxOldFolder);
            this.grpBoxFolderSelect.Controls.Add(this.lblOldFolder);
            this.grpBoxFolderSelect.Location = new System.Drawing.Point(12, 11);
            this.grpBoxFolderSelect.Name = "grpBoxFolderSelect";
            this.grpBoxFolderSelect.Size = new System.Drawing.Size(457, 165);
            this.grpBoxFolderSelect.TabIndex = 0;
            this.grpBoxFolderSelect.TabStop = false;
            this.grpBoxFolderSelect.Text = "Select Cemu folders";
            // 
            // lblNewVersionNr
            // 
            this.lblNewVersionNr.AutoSize = true;
            this.lblNewVersionNr.Location = new System.Drawing.Point(405, 73);
            this.lblNewVersionNr.Name = "lblNewVersionNr";
            this.lblNewVersionNr.Size = new System.Drawing.Size(19, 13);
            this.lblNewVersionNr.TabIndex = 12;
            this.lblNewVersionNr.Text = "    ";
            // 
            // lblOldVersionNr
            // 
            this.lblOldVersionNr.AutoSize = true;
            this.lblOldVersionNr.Location = new System.Drawing.Point(405, 25);
            this.lblOldVersionNr.Name = "lblOldVersionNr";
            this.lblOldVersionNr.Size = new System.Drawing.Size(19, 13);
            this.lblOldVersionNr.TabIndex = 11;
            this.lblOldVersionNr.Text = "    ";
            // 
            // lblNewCemuVersion
            // 
            this.lblNewCemuVersion.AutoSize = true;
            this.lblNewCemuVersion.Location = new System.Drawing.Point(335, 73);
            this.lblNewCemuVersion.Name = "lblNewCemuVersion";
            this.lblNewCemuVersion.Size = new System.Drawing.Size(74, 13);
            this.lblNewCemuVersion.TabIndex = 10;
            this.lblNewCemuVersion.Text = "Cemu version:";
            this.lblNewCemuVersion.Visible = false;
            // 
            // lblOldCemuVersion
            // 
            this.lblOldCemuVersion.AutoSize = true;
            this.lblOldCemuVersion.Location = new System.Drawing.Point(335, 25);
            this.lblOldCemuVersion.Name = "lblOldCemuVersion";
            this.lblOldCemuVersion.Size = new System.Drawing.Size(74, 13);
            this.lblOldCemuVersion.TabIndex = 9;
            this.lblOldCemuVersion.Text = "Cemu version:";
            this.lblOldCemuVersion.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(280, 129);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.CancelOperations);
            // 
            // btnStart
            // 
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(361, 129);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 25);
            this.btnStart.TabIndex = 7;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.DoOperationsAsync);
            // 
            // btnOptions
            // 
            this.btnOptions.Location = new System.Drawing.Point(199, 129);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(75, 25);
            this.btnOptions.TabIndex = 6;
            this.btnOptions.Text = "Options...";
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.OpenOptionsForm);
            // 
            // btnSelectNewFolder
            // 
            this.btnSelectNewFolder.Location = new System.Drawing.Point(361, 91);
            this.btnSelectNewFolder.Name = "btnSelectNewFolder";
            this.btnSelectNewFolder.Size = new System.Drawing.Size(76, 22);
            this.btnSelectNewFolder.TabIndex = 5;
            this.btnSelectNewFolder.Text = "Select";
            this.btnSelectNewFolder.UseVisualStyleBackColor = true;
            this.btnSelectNewFolder.Click += new System.EventHandler(this.SelectNewCemuFolder);
            // 
            // txtBoxNewFolder
            // 
            this.errProviderNewFolder.SetIconPadding(this.txtBoxNewFolder, -20);
            this.txtBoxNewFolder.Location = new System.Drawing.Point(22, 92);
            this.txtBoxNewFolder.Name = "txtBoxNewFolder";
            this.txtBoxNewFolder.Size = new System.Drawing.Size(333, 20);
            this.txtBoxNewFolder.TabIndex = 4;
            this.txtBoxNewFolder.TextChanged += new System.EventHandler(this.CheckNewFolderTextboxContent);
            // 
            // lblNewFolder
            // 
            this.lblNewFolder.AutoSize = true;
            this.lblNewFolder.Location = new System.Drawing.Point(19, 73);
            this.lblNewFolder.Name = "lblNewFolder";
            this.lblNewFolder.Size = new System.Drawing.Size(88, 13);
            this.lblNewFolder.TabIndex = 3;
            this.lblNewFolder.Text = "New Cemu folder";
            // 
            // btnSelectOldFolder
            // 
            this.btnSelectOldFolder.Location = new System.Drawing.Point(361, 43);
            this.btnSelectOldFolder.Name = "btnSelectOldFolder";
            this.btnSelectOldFolder.Size = new System.Drawing.Size(76, 22);
            this.btnSelectOldFolder.TabIndex = 2;
            this.btnSelectOldFolder.Text = "Select";
            this.btnSelectOldFolder.UseVisualStyleBackColor = true;
            this.btnSelectOldFolder.Click += new System.EventHandler(this.SelectOldCemuFolder);
            // 
            // txtBoxOldFolder
            // 
            this.errProviderOldFolder.SetIconPadding(this.txtBoxOldFolder, -20);
            this.txtBoxOldFolder.Location = new System.Drawing.Point(22, 44);
            this.txtBoxOldFolder.Name = "txtBoxOldFolder";
            this.txtBoxOldFolder.Size = new System.Drawing.Size(333, 20);
            this.txtBoxOldFolder.TabIndex = 1;
            this.txtBoxOldFolder.TextChanged += new System.EventHandler(this.CheckOldFolderTextboxContent);
            // 
            // lblOldFolder
            // 
            this.lblOldFolder.AutoSize = true;
            this.lblOldFolder.Location = new System.Drawing.Point(19, 25);
            this.lblOldFolder.Name = "lblOldFolder";
            this.lblOldFolder.Size = new System.Drawing.Size(91, 13);
            this.lblOldFolder.TabIndex = 0;
            this.lblOldFolder.Text = "Older Cemu folder";
            // 
            // grpBoxProgress
            // 
            this.grpBoxProgress.Controls.Add(this.txtBoxLog);
            this.grpBoxProgress.Controls.Add(this.lblDetails);
            this.grpBoxProgress.Controls.Add(this.lblPercent);
            this.grpBoxProgress.Controls.Add(this.overallProgressBar);
            this.grpBoxProgress.Controls.Add(this.lblCurrentTask);
            this.grpBoxProgress.Location = new System.Drawing.Point(13, 184);
            this.grpBoxProgress.Name = "grpBoxProgress";
            this.grpBoxProgress.Size = new System.Drawing.Size(456, 198);
            this.grpBoxProgress.TabIndex = 1;
            this.grpBoxProgress.TabStop = false;
            this.grpBoxProgress.Text = "Progress";
            // 
            // lblPercent
            // 
            this.lblPercent.AutoSize = true;
            this.lblPercent.Location = new System.Drawing.Point(414, 24);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(21, 13);
            this.lblPercent.TabIndex = 5;
            this.lblPercent.Text = "0%";
            this.lblPercent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // overallProgressBar
            // 
            this.overallProgressBar.Location = new System.Drawing.Point(21, 44);
            this.overallProgressBar.Name = "overallProgressBar";
            this.overallProgressBar.Size = new System.Drawing.Size(414, 30);
            this.overallProgressBar.TabIndex = 3;
            // 
            // lblCurrentTask
            // 
            this.lblCurrentTask.AutoSize = true;
            this.lblCurrentTask.Location = new System.Drawing.Point(18, 24);
            this.lblCurrentTask.Name = "lblCurrentTask";
            this.lblCurrentTask.Size = new System.Drawing.Size(154, 13);
            this.lblCurrentTask.TabIndex = 1;
            this.lblCurrentTask.Text = "Waiting for operations to start...";
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(222, 390);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 29);
            this.btnHelp.TabIndex = 2;
            this.btnHelp.Text = "Help!";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.OpenHelpForm);
            // 
            // btnAbout
            // 
            this.btnAbout.Location = new System.Drawing.Point(303, 390);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(75, 29);
            this.btnAbout.TabIndex = 3;
            this.btnAbout.Text = "About...";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.OpenAboutForm);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(384, 390);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 29);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.Exit);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(74, 353);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(0, 13);
            this.lblVersion.TabIndex = 5;
            //this.lblVersion.Text = $"Version {Program.Version}";
            // 
            // errProviderNewFolder
            // 
            this.errProviderNewFolder.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errProviderNewFolder.ContainerControl = this;
            // 
            // errProviderOldFolder
            // 
            this.errProviderOldFolder.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errProviderOldFolder.ContainerControl = this;
            // 
            // lblDetails
            // 
            this.lblDetails.AutoSize = true;
            this.lblDetails.Location = new System.Drawing.Point(18, 86);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(39, 13);
            this.lblDetails.TabIndex = 6;
            this.lblDetails.Text = "Details";
            // 
            // txtBoxLog
            // 
            this.txtBoxLog.BackColor = System.Drawing.Color.White;
            this.txtBoxLog.Location = new System.Drawing.Point(21, 104);
            this.txtBoxLog.Multiline = true;
            this.txtBoxLog.Name = "txtBoxLog";
            this.txtBoxLog.ReadOnly = true;
            this.txtBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtBoxLog.Size = new System.Drawing.Size(414, 78);
            this.txtBoxLog.TabIndex = 7;
            // 
            // MigrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 430);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.grpBoxProgress);
            this.Controls.Add(this.grpBoxFolderSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "MigrationForm";
            this.Text = "Cemu Update Tool";
            this.grpBoxFolderSelect.ResumeLayout(false);
            this.grpBoxFolderSelect.PerformLayout();
            this.grpBoxProgress.ResumeLayout(false);
            this.grpBoxProgress.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderNewFolder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderOldFolder)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpBoxFolderSelect;
        private System.Windows.Forms.TextBox txtBoxOldFolder;
        private System.Windows.Forms.Label lblOldFolder;
        private System.Windows.Forms.Button btnSelectOldFolder;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.Button btnSelectNewFolder;
        private System.Windows.Forms.TextBox txtBoxNewFolder;
        private System.Windows.Forms.Label lblNewFolder;
        private System.Windows.Forms.GroupBox grpBoxProgress;
        private System.Windows.Forms.Label lblPercent;
        private System.Windows.Forms.ProgressBar overallProgressBar;
        private System.Windows.Forms.Label lblCurrentTask;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ErrorProvider errProviderNewFolder;
        private System.Windows.Forms.Label lblNewCemuVersion;
        private System.Windows.Forms.Label lblOldCemuVersion;
        private System.Windows.Forms.Label lblNewVersionNr;
        private System.Windows.Forms.Label lblOldVersionNr;
        private System.Windows.Forms.ErrorProvider errProviderOldFolder;
        private System.Windows.Forms.Label lblDetails;
        private System.Windows.Forms.TextBox txtBoxLog;
    }
}

