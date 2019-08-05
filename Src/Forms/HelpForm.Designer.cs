namespace CemuUpdateTool.Forms
{
    partial class HelpForm
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
            this.btnClose = new System.Windows.Forms.Button();
            this.richTxtBoxHelp = new System.Windows.Forms.RichTextBox();
            this.toolTipProfileInfo = new System.Windows.Forms.ToolTip(this.components);
            this.linkLblQuestions = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(478, 271);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(96, 29);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.Exit);
            // 
            // richTxtBoxHelp
            // 
            this.richTxtBoxHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTxtBoxHelp.BackColor = System.Drawing.Color.White;
            this.richTxtBoxHelp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTxtBoxHelp.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTxtBoxHelp.Location = new System.Drawing.Point(12, 12);
            this.richTxtBoxHelp.Name = "richTxtBoxHelp";
            this.richTxtBoxHelp.ReadOnly = true;
            this.richTxtBoxHelp.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTxtBoxHelp.Size = new System.Drawing.Size(562, 251);
            this.richTxtBoxHelp.TabIndex = 6;
            this.richTxtBoxHelp.Text = "";
            // 
            // toolTipProfileInfo
            // 
            this.toolTipProfileInfo.AutoPopDelay = 5000;
            this.toolTipProfileInfo.InitialDelay = 300;
            this.toolTipProfileInfo.ReshowDelay = 100;
            // 
            // linkLblQuestions
            // 
            this.linkLblQuestions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLblQuestions.AutoSize = true;
            this.linkLblQuestions.LinkArea = new System.Windows.Forms.LinkArea(83, 0);
            this.linkLblQuestions.Location = new System.Drawing.Point(12, 276);
            this.linkLblQuestions.Name = "linkLblQuestions";
            this.linkLblQuestions.Size = new System.Drawing.Size(457, 21);
            this.linkLblQuestions.TabIndex = 7;
            this.linkLblQuestions.Text = "More questions? Don\'t hesitate to ask me in the official forum thread or on GitHu" +
    "b.";
            this.linkLblQuestions.UseCompatibleTextRendering = true;
            this.linkLblQuestions.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelClicked);
            // 
            // HelpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(587, 309);
            this.Controls.Add(this.linkLblQuestions);
            this.Controls.Add(this.richTxtBoxHelp);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(603, 150);
            this.Name = "HelpForm";
            this.Text = "Help";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.RichTextBox richTxtBoxHelp;
        private System.Windows.Forms.ToolTip toolTipProfileInfo;
        private System.Windows.Forms.LinkLabel linkLblQuestions;
    }
}