namespace CemuUpdateTool
{
    partial class OperationsForm
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
            this.components = new System.ComponentModel.Container();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.txtBoxLog = new System.Windows.Forms.TextBox();
            this.lblDetails = new System.Windows.Forms.Label();
            this.lblPercent = new System.Windows.Forms.Label();
            this.overallProgressBar = new System.Windows.Forms.ProgressBar();
            this.lblCurrentTask = new System.Windows.Forms.Label();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.errProviderFolders = new System.Windows.Forms.ErrorProvider(this.components);
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.pnlBorderBottom = new System.Windows.Forms.Label();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlBorderTop = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderFolders)).BeginInit();
            this.bottomPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(326, 142);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 27);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.CancelOperations);
            // 
            // btnStart
            // 
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(419, 142);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(89, 27);
            this.btnStart.TabIndex = 8;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.DoOperationsAsync);
            // 
            // txtBoxLog
            // 
            this.txtBoxLog.BackColor = System.Drawing.Color.White;
            this.txtBoxLog.Location = new System.Drawing.Point(30, 284);
            this.txtBoxLog.Margin = new System.Windows.Forms.Padding(3, 7, 3, 0);
            this.txtBoxLog.Multiline = true;
            this.txtBoxLog.Name = "txtBoxLog";
            this.txtBoxLog.ReadOnly = true;
            this.txtBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtBoxLog.Size = new System.Drawing.Size(478, 134);
            this.txtBoxLog.TabIndex = 11;
            this.txtBoxLog.TabStop = false;
            // 
            // lblDetails
            // 
            this.lblDetails.Location = new System.Drawing.Point(14, 263);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(493, 15);
            this.lblDetails.TabIndex = 6;
            this.lblDetails.Text = "▽ Details";
            this.lblDetails.Click += new System.EventHandler(this.ShowHideDetailsTextbox);
            // 
            // lblPercent
            // 
            this.lblPercent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPercent.Location = new System.Drawing.Point(470, 193);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(38, 15);
            this.lblPercent.TabIndex = 5;
            this.lblPercent.Text = "0%";
            this.lblPercent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // overallProgressBar
            // 
            this.overallProgressBar.Location = new System.Drawing.Point(16, 215);
            this.overallProgressBar.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.overallProgressBar.Name = "overallProgressBar";
            this.overallProgressBar.Size = new System.Drawing.Size(492, 35);
            this.overallProgressBar.TabIndex = 10;
            // 
            // lblCurrentTask
            // 
            this.lblCurrentTask.AutoSize = true;
            this.lblCurrentTask.Location = new System.Drawing.Point(14, 193);
            this.lblCurrentTask.Name = "lblCurrentTask";
            this.lblCurrentTask.Size = new System.Drawing.Size(174, 15);
            this.lblCurrentTask.TabIndex = 1;
            this.lblCurrentTask.Text = "Waiting for operations to start...";
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnHelp.Location = new System.Drawing.Point(421, 10);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(87, 29);
            this.btnHelp.TabIndex = 11;
            this.btnHelp.Text = "Help!";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.OpenHelpForm);
            // 
            // btnBack
            // 
            this.btnBack.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnBack.Location = new System.Drawing.Point(16, 10);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(87, 29);
            this.btnBack.TabIndex = 10;
            this.btnBack.Text = "< Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.Back);
            // 
            // errProviderFolders
            // 
            this.errProviderFolders.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errProviderFolders.ContainerControl = this;
            // 
            // bottomPanel
            // 
            this.bottomPanel.BackColor = System.Drawing.SystemColors.Control;
            this.bottomPanel.Controls.Add(this.pnlBorderBottom);
            this.bottomPanel.Controls.Add(this.btnHelp);
            this.bottomPanel.Controls.Add(this.btnBack);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 435);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(519, 46);
            this.bottomPanel.TabIndex = 12;
            // 
            // pnlBorderBottom
            // 
            this.pnlBorderBottom.BackColor = System.Drawing.SystemColors.Control;
            this.pnlBorderBottom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlBorderBottom.Location = new System.Drawing.Point(-1, 0);
            this.pnlBorderBottom.Name = "pnlBorderBottom";
            this.pnlBorderBottom.Size = new System.Drawing.Size(520, 1);
            this.pnlBorderBottom.TabIndex = 13;
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.SystemColors.Control;
            this.headerPanel.Controls.Add(this.lblDescription);
            this.headerPanel.Controls.Add(this.lblTitle);
            this.headerPanel.Controls.Add(this.pnlBorderTop);
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
            this.lblTitle.UseMnemonic = false;
            // 
            // pnlBorderTop
            // 
            this.pnlBorderTop.BackColor = System.Drawing.SystemColors.Control;
            this.pnlBorderTop.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlBorderTop.Location = new System.Drawing.Point(-1, 63);
            this.pnlBorderTop.Name = "pnlBorderTop";
            this.pnlBorderTop.Size = new System.Drawing.Size(530, 1);
            this.pnlBorderTop.TabIndex = 13;
            // 
            // OperationsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(519, 481);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.txtBoxLog);
            this.Controls.Add(this.lblDetails);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.lblPercent);
            this.Controls.Add(this.overallProgressBar);
            this.Controls.Add(this.lblCurrentTask);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnStart);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "OperationsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Cemu Update Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PreventClosingIfOperationInProgress);
            ((System.ComponentModel.ISupportInitialize)(this.errProviderFolders)).EndInit();
            this.bottomPanel.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Button btnStart;
        protected System.Windows.Forms.Label lblPercent;
        protected System.Windows.Forms.ProgressBar overallProgressBar;
        protected System.Windows.Forms.Label lblCurrentTask;
        protected System.Windows.Forms.Button btnHelp;
        protected System.Windows.Forms.Button btnBack;
        protected System.Windows.Forms.Button btnCancel;
        protected System.Windows.Forms.ErrorProvider errProviderFolders;
        protected System.Windows.Forms.Label lblDetails;
        protected System.Windows.Forms.TextBox txtBoxLog;
        protected System.Windows.Forms.Panel bottomPanel;
        protected System.Windows.Forms.Label pnlBorderBottom;
        protected System.Windows.Forms.Panel headerPanel;
        protected System.Windows.Forms.Label pnlBorderTop;
        protected System.Windows.Forms.Label lblTitle;
        protected System.Windows.Forms.Label lblDescription;
    }
}