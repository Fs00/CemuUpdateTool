﻿namespace CemuUpdateTool
{
    partial class ContainerForm
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
            this.formContainer = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // formContainer
            // 
            this.formContainer.AutoSize = true;
            this.formContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.formContainer.BackColor = System.Drawing.SystemColors.Control;
            this.formContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formContainer.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.formContainer.Location = new System.Drawing.Point(0, 0);
            this.formContainer.Margin = new System.Windows.Forms.Padding(0);
            this.formContainer.Name = "formContainer";
            this.formContainer.Size = new System.Drawing.Size(278, 173);
            this.formContainer.TabIndex = 0;
            // 
            // ContainerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(278, 173);
            this.Controls.Add(this.formContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ContainerForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Cemu Update Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CloseDisplayingFormOnClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel formContainer;
    }
}