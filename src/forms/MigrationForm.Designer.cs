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
            this.txtBoxLog = new System.Windows.Forms.TextBox();
            this.lblDetails = new System.Windows.Forms.Label();
            this.lblPercent = new System.Windows.Forms.Label();
            this.overallProgressBar = new System.Windows.Forms.ProgressBar();
            this.lblCurrentTask = new System.Windows.Forms.Label();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.errProviderNewFolder = new System.Windows.Forms.ErrorProvider(this.components);
            this.errProviderOldFolder = new System.Windows.Forms.ErrorProvider(this.components);
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.grpBoxFolderSelect.SuspendLayout();
            this.grpBoxProgress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderNewFolder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderOldFolder)).BeginInit();
            this.bottomPanel.SuspendLayout();
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
            this.grpBoxFolderSelect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBoxFolderSelect.Location = new System.Drawing.Point(14, 9);
            this.grpBoxFolderSelect.Name = "grpBoxFolderSelect";
            this.grpBoxFolderSelect.Size = new System.Drawing.Size(539, 180);
            this.grpBoxFolderSelect.TabIndex = 0;
            this.grpBoxFolderSelect.TabStop = false;
            this.grpBoxFolderSelect.Text = "Select Cemu folders";
            // 
            // lblNewVersionNr
            // 
            this.lblNewVersionNr.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblNewVersionNr.Location = new System.Drawing.Point(474, 82);
            this.lblNewVersionNr.Name = "lblNewVersionNr";
            this.lblNewVersionNr.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblNewVersionNr.Size = new System.Drawing.Size(40, 15);
            this.lblNewVersionNr.TabIndex = 12;
            this.lblNewVersionNr.Text = "    ";
            this.lblNewVersionNr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblOldVersionNr
            // 
            this.lblOldVersionNr.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblOldVersionNr.Location = new System.Drawing.Point(474, 27);
            this.lblOldVersionNr.Name = "lblOldVersionNr";
            this.lblOldVersionNr.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblOldVersionNr.Size = new System.Drawing.Size(40, 15);
            this.lblOldVersionNr.TabIndex = 11;
            this.lblOldVersionNr.Text = "    ";
            this.lblOldVersionNr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblNewCemuVersion
            // 
            this.lblNewCemuVersion.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblNewCemuVersion.AutoSize = true;
            this.lblNewCemuVersion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewCemuVersion.Location = new System.Drawing.Point(395, 82);
            this.lblNewCemuVersion.Name = "lblNewCemuVersion";
            this.lblNewCemuVersion.Size = new System.Drawing.Size(83, 15);
            this.lblNewCemuVersion.TabIndex = 10;
            this.lblNewCemuVersion.Text = "Cemu version:";
            this.lblNewCemuVersion.Visible = false;
            // 
            // lblOldCemuVersion
            // 
            this.lblOldCemuVersion.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblOldCemuVersion.AutoSize = true;
            this.lblOldCemuVersion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOldCemuVersion.Location = new System.Drawing.Point(395, 27);
            this.lblOldCemuVersion.Name = "lblOldCemuVersion";
            this.lblOldCemuVersion.Size = new System.Drawing.Size(83, 15);
            this.lblOldCemuVersion.TabIndex = 9;
            this.lblOldCemuVersion.Text = "Cemu version:";
            this.lblOldCemuVersion.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(334, 141);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 27);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.CancelOperations);
            // 
            // btnStart
            // 
            this.btnStart.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(426, 141);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(89, 27);
            this.btnStart.TabIndex = 7;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.DoOperationsAsync);
            // 
            // btnOptions
            // 
            this.btnOptions.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnOptions.Location = new System.Drawing.Point(241, 141);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(87, 27);
            this.btnOptions.TabIndex = 6;
            this.btnOptions.Text = "Options...";
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.OpenOptionsForm);
            // 
            // btnSelectNewFolder
            // 
            this.btnSelectNewFolder.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnSelectNewFolder.Location = new System.Drawing.Point(426, 103);
            this.btnSelectNewFolder.Name = "btnSelectNewFolder";
            this.btnSelectNewFolder.Size = new System.Drawing.Size(89, 25);
            this.btnSelectNewFolder.TabIndex = 5;
            this.btnSelectNewFolder.Text = "Select";
            this.btnSelectNewFolder.UseVisualStyleBackColor = true;
            this.btnSelectNewFolder.Click += new System.EventHandler(this.SelectNewCemuFolder);
            // 
            // txtBoxNewFolder
            // 
            this.txtBoxNewFolder.AllowDrop = true;
            this.errProviderNewFolder.SetIconPadding(this.txtBoxNewFolder, -20);
            this.txtBoxNewFolder.Location = new System.Drawing.Point(26, 104);
            this.txtBoxNewFolder.Name = "txtBoxNewFolder";
            this.txtBoxNewFolder.Size = new System.Drawing.Size(394, 23);
            this.txtBoxNewFolder.TabIndex = 4;
            this.txtBoxNewFolder.TextChanged += new System.EventHandler(this.CheckNewFolderTextboxContent);
            this.txtBoxNewFolder.DragDrop += new System.Windows.Forms.DragEventHandler(this.TextboxDragDrop);
            this.txtBoxNewFolder.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextboxDragEnter);
            // 
            // lblNewFolder
            // 
            this.lblNewFolder.AutoSize = true;
            this.lblNewFolder.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewFolder.Location = new System.Drawing.Point(22, 82);
            this.lblNewFolder.Name = "lblNewFolder";
            this.lblNewFolder.Size = new System.Drawing.Size(100, 15);
            this.lblNewFolder.TabIndex = 3;
            this.lblNewFolder.Text = "New Cemu folder";
            // 
            // btnSelectOldFolder
            // 
            this.btnSelectOldFolder.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnSelectOldFolder.Location = new System.Drawing.Point(426, 48);
            this.btnSelectOldFolder.Name = "btnSelectOldFolder";
            this.btnSelectOldFolder.Size = new System.Drawing.Size(89, 25);
            this.btnSelectOldFolder.TabIndex = 2;
            this.btnSelectOldFolder.Text = "Select";
            this.btnSelectOldFolder.UseVisualStyleBackColor = true;
            this.btnSelectOldFolder.Click += new System.EventHandler(this.SelectOldCemuFolder);
            // 
            // txtBoxOldFolder
            // 
            this.txtBoxOldFolder.AllowDrop = true;
            this.errProviderOldFolder.SetIconPadding(this.txtBoxOldFolder, -20);
            this.txtBoxOldFolder.Location = new System.Drawing.Point(26, 49);
            this.txtBoxOldFolder.Name = "txtBoxOldFolder";
            this.txtBoxOldFolder.Size = new System.Drawing.Size(394, 23);
            this.txtBoxOldFolder.TabIndex = 1;
            this.txtBoxOldFolder.TextChanged += new System.EventHandler(this.CheckOldFolderTextboxContent);
            this.txtBoxOldFolder.DragDrop += new System.Windows.Forms.DragEventHandler(this.TextboxDragDrop);
            this.txtBoxOldFolder.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextboxDragEnter);
            // 
            // lblOldFolder
            // 
            this.lblOldFolder.AutoSize = true;
            this.lblOldFolder.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOldFolder.Location = new System.Drawing.Point(22, 27);
            this.lblOldFolder.Name = "lblOldFolder";
            this.lblOldFolder.Size = new System.Drawing.Size(105, 15);
            this.lblOldFolder.TabIndex = 0;
            this.lblOldFolder.Text = "Older Cemu folder";
            // 
            // grpBoxProgress
            // 
            this.grpBoxProgress.AutoSize = true;
            this.grpBoxProgress.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpBoxProgress.Controls.Add(this.txtBoxLog);
            this.grpBoxProgress.Controls.Add(this.lblDetails);
            this.grpBoxProgress.Controls.Add(this.lblPercent);
            this.grpBoxProgress.Controls.Add(this.overallProgressBar);
            this.grpBoxProgress.Controls.Add(this.lblCurrentTask);
            this.grpBoxProgress.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBoxProgress.Location = new System.Drawing.Point(14, 196);
            this.grpBoxProgress.Name = "grpBoxProgress";
            this.grpBoxProgress.Size = new System.Drawing.Size(539, 273);
            this.grpBoxProgress.TabIndex = 1;
            this.grpBoxProgress.TabStop = false;
            this.grpBoxProgress.Text = "Progress";
            // 
            // txtBoxLog
            // 
            this.txtBoxLog.BackColor = System.Drawing.Color.White;
            this.txtBoxLog.Location = new System.Drawing.Point(37, 121);
            this.txtBoxLog.Margin = new System.Windows.Forms.Padding(3, 7, 3, 0);
            this.txtBoxLog.Multiline = true;
            this.txtBoxLog.Name = "txtBoxLog";
            this.txtBoxLog.ReadOnly = true;
            this.txtBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtBoxLog.Size = new System.Drawing.Size(478, 133);
            this.txtBoxLog.TabIndex = 7;
            this.txtBoxLog.VisibleChanged += new System.EventHandler(this.ResizeFormOnLogTextboxVisibleChanged);
            // 
            // lblDetails
            // 
            this.lblDetails.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetails.Location = new System.Drawing.Point(21, 99);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(512, 15);
            this.lblDetails.TabIndex = 6;
            this.lblDetails.Text = "▽ Details";
            this.lblDetails.Click += new System.EventHandler(this.ShowHideDetailsTextbox);
            // 
            // lblPercent
            // 
            this.lblPercent.AutoSize = true;
            this.lblPercent.Location = new System.Drawing.Point(491, 29);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(23, 15);
            this.lblPercent.TabIndex = 5;
            this.lblPercent.Text = "0%";
            this.lblPercent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // overallProgressBar
            // 
            this.overallProgressBar.Location = new System.Drawing.Point(23, 51);
            this.overallProgressBar.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.overallProgressBar.Name = "overallProgressBar";
            this.overallProgressBar.Size = new System.Drawing.Size(492, 35);
            this.overallProgressBar.TabIndex = 3;
            // 
            // lblCurrentTask
            // 
            this.lblCurrentTask.AutoSize = true;
            this.lblCurrentTask.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentTask.Location = new System.Drawing.Point(21, 29);
            this.lblCurrentTask.Name = "lblCurrentTask";
            this.lblCurrentTask.Size = new System.Drawing.Size(174, 15);
            this.lblCurrentTask.TabIndex = 1;
            this.lblCurrentTask.Text = "Waiting for operations to start...";
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnHelp.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHelp.Location = new System.Drawing.Point(263, 7);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(87, 29);
            this.btnHelp.TabIndex = 2;
            this.btnHelp.Text = "Help!";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.OpenHelpForm);
            // 
            // btnAbout
            // 
            this.btnAbout.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnAbout.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAbout.Location = new System.Drawing.Point(356, 7);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(87, 29);
            this.btnAbout.TabIndex = 3;
            this.btnAbout.Text = "About...";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.OpenAboutForm);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(449, 7);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(87, 29);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.Exit);
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
            // bottomPanel
            // 
            this.bottomPanel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.bottomPanel.BackColor = System.Drawing.SystemColors.Control;
            this.bottomPanel.Controls.Add(this.label1);
            this.bottomPanel.Controls.Add(this.btnHelp);
            this.bottomPanel.Controls.Add(this.btnExit);
            this.bottomPanel.Controls.Add(this.btnAbout);
            this.bottomPanel.Location = new System.Drawing.Point(-8, 480);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(582, 51);
            this.bottomPanel.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(582, 2);
            this.label1.TabIndex = 13;
            // 
            // MigrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(567, 524);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.grpBoxProgress);
            this.Controls.Add(this.grpBoxFolderSelect);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "MigrationForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Cemu Update Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ShutdownDispatcherOnFormClosing);
            this.grpBoxFolderSelect.ResumeLayout(false);
            this.grpBoxFolderSelect.PerformLayout();
            this.grpBoxProgress.ResumeLayout(false);
            this.grpBoxProgress.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderNewFolder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderOldFolder)).EndInit();
            this.bottomPanel.ResumeLayout(false);
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
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ErrorProvider errProviderNewFolder;
        private System.Windows.Forms.Label lblNewCemuVersion;
        private System.Windows.Forms.Label lblOldCemuVersion;
        private System.Windows.Forms.Label lblNewVersionNr;
        private System.Windows.Forms.Label lblOldVersionNr;
        private System.Windows.Forms.ErrorProvider errProviderOldFolder;
        private System.Windows.Forms.Label lblDetails;
        private System.Windows.Forms.TextBox txtBoxLog;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Label label1;
    }
}

