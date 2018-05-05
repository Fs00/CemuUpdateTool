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
            this.headerPanel = new System.Windows.Forms.Panel();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderNewFolder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderOldFolder)).BeginInit();
            this.bottomPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblNewVersionNr
            // 
            this.lblNewVersionNr.Location = new System.Drawing.Point(464, 129);
            this.lblNewVersionNr.Name = "lblNewVersionNr";
            this.lblNewVersionNr.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblNewVersionNr.Size = new System.Drawing.Size(40, 15);
            this.lblNewVersionNr.TabIndex = 12;
            this.lblNewVersionNr.Text = "    ";
            this.lblNewVersionNr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblOldVersionNr
            // 
            this.lblOldVersionNr.Location = new System.Drawing.Point(465, 74);
            this.lblOldVersionNr.Name = "lblOldVersionNr";
            this.lblOldVersionNr.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblOldVersionNr.Size = new System.Drawing.Size(40, 15);
            this.lblOldVersionNr.TabIndex = 11;
            this.lblOldVersionNr.Text = "    ";
            this.lblOldVersionNr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblNewCemuVersion
            // 
            this.lblNewCemuVersion.AutoSize = true;
            this.lblNewCemuVersion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewCemuVersion.Location = new System.Drawing.Point(385, 129);
            this.lblNewCemuVersion.Name = "lblNewCemuVersion";
            this.lblNewCemuVersion.Size = new System.Drawing.Size(83, 15);
            this.lblNewCemuVersion.TabIndex = 10;
            this.lblNewCemuVersion.Text = "Cemu version:";
            this.lblNewCemuVersion.Visible = false;
            // 
            // lblOldCemuVersion
            // 
            this.lblOldCemuVersion.AutoSize = true;
            this.lblOldCemuVersion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOldCemuVersion.Location = new System.Drawing.Point(385, 74);
            this.lblOldCemuVersion.Name = "lblOldCemuVersion";
            this.lblOldCemuVersion.Size = new System.Drawing.Size(83, 15);
            this.lblOldCemuVersion.TabIndex = 9;
            this.lblOldCemuVersion.Text = "Cemu version:";
            this.lblOldCemuVersion.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(324, 189);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 27);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.CancelOperations);
            // 
            // btnStart
            // 
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(416, 189);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(89, 27);
            this.btnStart.TabIndex = 7;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.DoOperationsAsync);
            // 
            // btnOptions
            // 
            this.btnOptions.Location = new System.Drawing.Point(231, 189);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(87, 27);
            this.btnOptions.TabIndex = 6;
            this.btnOptions.Text = "Options...";
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.OpenOptionsForm);
            // 
            // btnSelectNewFolder
            // 
            this.btnSelectNewFolder.Location = new System.Drawing.Point(416, 150);
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
            this.txtBoxNewFolder.Location = new System.Drawing.Point(16, 151);
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
            this.lblNewFolder.Location = new System.Drawing.Point(12, 129);
            this.lblNewFolder.Name = "lblNewFolder";
            this.lblNewFolder.Size = new System.Drawing.Size(100, 15);
            this.lblNewFolder.TabIndex = 3;
            this.lblNewFolder.Text = "New Cemu folder";
            // 
            // btnSelectOldFolder
            // 
            this.btnSelectOldFolder.Location = new System.Drawing.Point(416, 95);
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
            this.txtBoxOldFolder.Location = new System.Drawing.Point(16, 96);
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
            this.lblOldFolder.Location = new System.Drawing.Point(12, 74);
            this.lblOldFolder.Name = "lblOldFolder";
            this.lblOldFolder.Size = new System.Drawing.Size(105, 15);
            this.lblOldFolder.TabIndex = 0;
            this.lblOldFolder.Text = "Older Cemu folder";
            // 
            // txtBoxLog
            // 
            this.txtBoxLog.BackColor = System.Drawing.Color.White;
            this.txtBoxLog.Location = new System.Drawing.Point(30, 331);
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
            this.lblDetails.Location = new System.Drawing.Point(14, 310);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(494, 15);
            this.lblDetails.TabIndex = 6;
            this.lblDetails.Text = "▽ Details";
            this.lblDetails.Click += new System.EventHandler(this.ShowHideDetailsTextbox);
            // 
            // lblPercent
            // 
            this.lblPercent.AutoSize = true;
            this.lblPercent.Location = new System.Drawing.Point(484, 240);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(23, 15);
            this.lblPercent.TabIndex = 5;
            this.lblPercent.Text = "0%";
            this.lblPercent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // overallProgressBar
            // 
            this.overallProgressBar.Location = new System.Drawing.Point(16, 262);
            this.overallProgressBar.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.overallProgressBar.Name = "overallProgressBar";
            this.overallProgressBar.Size = new System.Drawing.Size(492, 35);
            this.overallProgressBar.TabIndex = 3;
            // 
            // lblCurrentTask
            // 
            this.lblCurrentTask.AutoSize = true;
            this.lblCurrentTask.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentTask.Location = new System.Drawing.Point(14, 240);
            this.lblCurrentTask.Name = "lblCurrentTask";
            this.lblCurrentTask.Size = new System.Drawing.Size(174, 15);
            this.lblCurrentTask.TabIndex = 1;
            this.lblCurrentTask.Text = "Waiting for operations to start...";
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnHelp.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHelp.Location = new System.Drawing.Point(234, 9);
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
            this.btnAbout.Location = new System.Drawing.Point(327, 9);
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
            this.btnExit.Location = new System.Drawing.Point(420, 9);
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
            this.bottomPanel.BackColor = System.Drawing.SystemColors.Control;
            this.bottomPanel.Controls.Add(this.label1);
            this.bottomPanel.Controls.Add(this.btnHelp);
            this.bottomPanel.Controls.Add(this.btnExit);
            this.bottomPanel.Controls.Add(this.btnAbout);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 473);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(519, 51);
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
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.SystemColors.Control;
            this.headerPanel.Controls.Add(this.lblDescription);
            this.headerPanel.Controls.Add(this.lblTitle);
            this.headerPanel.Controls.Add(this.label2);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(519, 64);
            this.headerPanel.TabIndex = 13;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(18, 34);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(403, 15);
            this.lblDescription.TabIndex = 15;
            this.lblDescription.Text = "Choose the source Cemu folder, the destination folder and then press Start.";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTitle.Location = new System.Drawing.Point(18, 13);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(0, 17);
            this.lblTitle.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(-3, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(582, 2);
            this.label2.TabIndex = 13;
            // 
            // MigrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(519, 524);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.txtBoxLog);
            this.Controls.Add(this.lblNewVersionNr);
            this.Controls.Add(this.lblDetails);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.lblPercent);
            this.Controls.Add(this.lblOldVersionNr);
            this.Controls.Add(this.overallProgressBar);
            this.Controls.Add(this.lblCurrentTask);
            this.Controls.Add(this.lblNewCemuVersion);
            this.Controls.Add(this.lblOldCemuVersion);
            this.Controls.Add(this.lblOldFolder);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtBoxOldFolder);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnSelectOldFolder);
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.lblNewFolder);
            this.Controls.Add(this.btnSelectNewFolder);
            this.Controls.Add(this.txtBoxNewFolder);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "MigrationForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Cemu Update Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ShutdownDispatcherOnFormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.errProviderNewFolder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderOldFolder)).EndInit();
            this.bottomPanel.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtBoxOldFolder;
        private System.Windows.Forms.Label lblOldFolder;
        private System.Windows.Forms.Button btnSelectOldFolder;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.Button btnSelectNewFolder;
        private System.Windows.Forms.TextBox txtBoxNewFolder;
        private System.Windows.Forms.Label lblNewFolder;
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
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDescription;
    }
}

