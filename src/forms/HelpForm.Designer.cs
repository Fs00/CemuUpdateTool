﻿namespace CemuUpdateTool
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
            this.lblContact = new System.Windows.Forms.Label();
            this.linkLblDiscord = new System.Windows.Forms.LinkLabel();
            this.lblOr = new System.Windows.Forms.Label();
            this.linkLblForum = new System.Windows.Forms.LinkLabel();
            this.btnClose = new System.Windows.Forms.Button();
            this.richTxtBoxHelp = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // lblContact
            // 
            this.lblContact.AutoSize = true;
            this.lblContact.Location = new System.Drawing.Point(12, 307);
            this.lblContact.Name = "lblContact";
            this.lblContact.Size = new System.Drawing.Size(241, 15);
            this.lblContact.TabIndex = 1;
            this.lblContact.Text = "More questions? Don\'t hesitate to ask me on";
            // 
            // linkLblDiscord
            // 
            this.linkLblDiscord.AutoSize = true;
            this.linkLblDiscord.Location = new System.Drawing.Point(251, 307);
            this.linkLblDiscord.Name = "linkLblDiscord";
            this.linkLblDiscord.Size = new System.Drawing.Size(47, 15);
            this.linkLblDiscord.TabIndex = 2;
            this.linkLblDiscord.TabStop = true;
            this.linkLblDiscord.Text = "Discord";
            this.linkLblDiscord.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelClicked);
            // 
            // lblOr
            // 
            this.lblOr.AutoSize = true;
            this.lblOr.Location = new System.Drawing.Point(295, 307);
            this.lblOr.Name = "lblOr";
            this.lblOr.Size = new System.Drawing.Size(51, 15);
            this.lblOr.TabIndex = 3;
            this.lblOr.Text = "or in the";
            // 
            // linkLblForum
            // 
            this.linkLblForum.AutoSize = true;
            this.linkLblForum.Location = new System.Drawing.Point(344, 307);
            this.linkLblForum.Name = "linkLblForum";
            this.linkLblForum.Size = new System.Drawing.Size(112, 15);
            this.linkLblForum.TabIndex = 4;
            this.linkLblForum.TabStop = true;
            this.linkLblForum.Text = "Cemu forum thread";
            this.linkLblForum.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelClicked);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(489, 300);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(96, 29);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.Exit);
            // 
            // richTxtBoxHelp
            // 
            this.richTxtBoxHelp.BackColor = System.Drawing.Color.White;
            this.richTxtBoxHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTxtBoxHelp.Location = new System.Drawing.Point(12, 12);
            this.richTxtBoxHelp.Name = "richTxtBoxHelp";
            this.richTxtBoxHelp.ReadOnly = true;
            this.richTxtBoxHelp.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTxtBoxHelp.Size = new System.Drawing.Size(573, 277);
            this.richTxtBoxHelp.TabIndex = 6;
            this.richTxtBoxHelp.Text = "";
            // 
            // HelpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(597, 338);
            this.Controls.Add(this.richTxtBoxHelp);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.linkLblForum);
            this.Controls.Add(this.lblOr);
            this.Controls.Add(this.linkLblDiscord);
            this.Controls.Add(this.lblContact);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "HelpForm";
            this.Text = "Help";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblContact;
        private System.Windows.Forms.LinkLabel linkLblDiscord;
        private System.Windows.Forms.Label lblOr;
        private System.Windows.Forms.LinkLabel linkLblForum;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.RichTextBox richTxtBoxHelp;
    }
}