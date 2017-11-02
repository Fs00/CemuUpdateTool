namespace CemuUpdateTool
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
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
            this.lblPercentOverall = new System.Windows.Forms.Label();
            this.lblOverallProgress = new System.Windows.Forms.Label();
            this.progressBarOverall = new CemuUpdateTool.CustomProgressBar();
            this.lblPercentSingle = new System.Windows.Forms.Label();
            this.lblSingleProgress = new System.Windows.Forms.Label();
            this.progressBarSingle = new CemuUpdateTool.CustomProgressBar();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblVersion = new System.Windows.Forms.Label();
            this.errProviderOldFolder = new System.Windows.Forms.ErrorProvider(this.components);
            this.errProviderNewFolder = new System.Windows.Forms.ErrorProvider(this.components);
            this.grpBoxFolderSelect.SuspendLayout();
            this.grpBoxProgress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderOldFolder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderNewFolder)).BeginInit();
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
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
            this.btnStart.Click += new System.EventHandler(this.StartOperations);
            // 
            // btnOptions
            // 
            this.btnOptions.Location = new System.Drawing.Point(199, 129);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(75, 25);
            this.btnOptions.TabIndex = 6;
            this.btnOptions.Text = "Options...";
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // btnSelectNewFolder
            // 
            this.btnSelectNewFolder.Location = new System.Drawing.Point(361, 91);
            this.btnSelectNewFolder.Name = "btnSelectNewFolder";
            this.btnSelectNewFolder.Size = new System.Drawing.Size(76, 22);
            this.btnSelectNewFolder.TabIndex = 5;
            this.btnSelectNewFolder.Text = "Select";
            this.btnSelectNewFolder.UseVisualStyleBackColor = true;
            this.btnSelectNewFolder.Click += new System.EventHandler(this.btnSelectNewFolder_Click);
            // 
            // txtBoxNewFolder
            // 
            this.errProviderNewFolder.SetIconPadding(this.txtBoxNewFolder, -20);
            this.txtBoxNewFolder.Location = new System.Drawing.Point(22, 92);
            this.txtBoxNewFolder.Name = "txtBoxNewFolder";
            this.txtBoxNewFolder.Size = new System.Drawing.Size(333, 20);
            this.txtBoxNewFolder.TabIndex = 4;
            this.txtBoxNewFolder.TextChanged += new System.EventHandler(this.txtBoxNewFolder_TextChanged);
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
            this.btnSelectOldFolder.Click += new System.EventHandler(this.btnSelectOldFolder_Click);
            // 
            // txtBoxOldFolder
            // 
            this.errProviderOldFolder.SetIconPadding(this.txtBoxOldFolder, -20);
            this.txtBoxOldFolder.Location = new System.Drawing.Point(22, 44);
            this.txtBoxOldFolder.Name = "txtBoxOldFolder";
            this.txtBoxOldFolder.Size = new System.Drawing.Size(333, 20);
            this.txtBoxOldFolder.TabIndex = 1;
            this.txtBoxOldFolder.TextChanged += new System.EventHandler(this.txtBoxOldFolder_TextChanged);
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
            this.grpBoxProgress.Controls.Add(this.lblPercentOverall);
            this.grpBoxProgress.Controls.Add(this.lblOverallProgress);
            this.grpBoxProgress.Controls.Add(this.progressBarOverall);
            this.grpBoxProgress.Controls.Add(this.lblPercentSingle);
            this.grpBoxProgress.Controls.Add(this.lblSingleProgress);
            this.grpBoxProgress.Controls.Add(this.progressBarSingle);
            this.grpBoxProgress.Location = new System.Drawing.Point(13, 184);
            this.grpBoxProgress.Name = "grpBoxProgress";
            this.grpBoxProgress.Size = new System.Drawing.Size(456, 152);
            this.grpBoxProgress.TabIndex = 1;
            this.grpBoxProgress.TabStop = false;
            this.grpBoxProgress.Text = "Progress";
            // 
            // lblPercentOverall
            // 
            this.lblPercentOverall.AutoSize = true;
            this.lblPercentOverall.Location = new System.Drawing.Point(414, 83);
            this.lblPercentOverall.Name = "lblPercentOverall";
            this.lblPercentOverall.Size = new System.Drawing.Size(21, 13);
            this.lblPercentOverall.TabIndex = 5;
            this.lblPercentOverall.Text = "0%";
            this.lblPercentOverall.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblOverallProgress
            // 
            this.lblOverallProgress.AutoSize = true;
            this.lblOverallProgress.Location = new System.Drawing.Point(18, 83);
            this.lblOverallProgress.Name = "lblOverallProgress";
            this.lblOverallProgress.Size = new System.Drawing.Size(83, 13);
            this.lblOverallProgress.TabIndex = 4;
            this.lblOverallProgress.Text = "Overall progress";
            // 
            // progressBarOverall
            // 
            this.progressBarOverall.CustomText = null;
            this.progressBarOverall.DisplayStyle = CemuUpdateTool.ProgressBarDisplayText.Percentage;
            this.progressBarOverall.Location = new System.Drawing.Point(21, 102);
            this.progressBarOverall.Name = "progressBarOverall";
            this.progressBarOverall.Size = new System.Drawing.Size(414, 30);
            this.progressBarOverall.TabIndex = 3;
            // 
            // lblPercentSingle
            // 
            this.lblPercentSingle.AutoSize = true;
            this.lblPercentSingle.Location = new System.Drawing.Point(414, 24);
            this.lblPercentSingle.Name = "lblPercentSingle";
            this.lblPercentSingle.Size = new System.Drawing.Size(21, 13);
            this.lblPercentSingle.TabIndex = 2;
            this.lblPercentSingle.Text = "0%";
            this.lblPercentSingle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSingleProgress
            // 
            this.lblSingleProgress.AutoSize = true;
            this.lblSingleProgress.Location = new System.Drawing.Point(18, 24);
            this.lblSingleProgress.Name = "lblSingleProgress";
            this.lblSingleProgress.Size = new System.Drawing.Size(154, 13);
            this.lblSingleProgress.TabIndex = 1;
            this.lblSingleProgress.Text = "Waiting for operations to start...";
            // 
            // progressBarSingle
            // 
            this.progressBarSingle.CustomText = null;
            this.progressBarSingle.DisplayStyle = CemuUpdateTool.ProgressBarDisplayText.Percentage;
            this.progressBarSingle.Location = new System.Drawing.Point(21, 43);
            this.progressBarSingle.Name = "progressBarSingle";
            this.progressBarSingle.Size = new System.Drawing.Size(414, 30);
            this.progressBarSingle.TabIndex = 0;
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(222, 345);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 29);
            this.btnHelp.TabIndex = 2;
            this.btnHelp.Text = "Help!";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnAbout
            // 
            this.btnAbout.Location = new System.Drawing.Point(303, 345);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(75, 29);
            this.btnAbout.TabIndex = 3;
            this.btnAbout.Text = "About...";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(384, 345);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 29);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(72, 353);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(81, 13);
            this.lblVersion.TabIndex = 5;
            this.lblVersion.Text = "Version 1.1-dev";
            // 
            // errProviderOldFolder
            // 
            this.errProviderOldFolder.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errProviderOldFolder.ContainerControl = this;
            // 
            // errProviderNewFolder
            // 
            this.errProviderNewFolder.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errProviderNewFolder.ContainerControl = this;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 386);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.grpBoxProgress);
            this.Controls.Add(this.grpBoxFolderSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Cemu Version Migration Tool";
            this.grpBoxFolderSelect.ResumeLayout(false);
            this.grpBoxFolderSelect.PerformLayout();
            this.grpBoxProgress.ResumeLayout(false);
            this.grpBoxProgress.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderOldFolder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderNewFolder)).EndInit();
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
        private System.Windows.Forms.Label lblPercentOverall;
        private System.Windows.Forms.Label lblOverallProgress;
        private CemuUpdateTool.CustomProgressBar progressBarOverall;
        private System.Windows.Forms.Label lblPercentSingle;
        private System.Windows.Forms.Label lblSingleProgress;
        private CemuUpdateTool.CustomProgressBar progressBarSingle;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ErrorProvider errProviderOldFolder;
        private System.Windows.Forms.ErrorProvider errProviderNewFolder;
        private System.Windows.Forms.Label lblNewCemuVersion;
        private System.Windows.Forms.Label lblOldCemuVersion;
        private System.Windows.Forms.Label lblNewVersionNr;
        private System.Windows.Forms.Label lblOldVersionNr;
    }
}

