namespace CemuUpdateTool
{
    partial class HomeForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.lnklblOpts = new System.Windows.Forms.LinkLabel();
            this.lnklblAbout = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(92, 100);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(140, 138);
            this.button1.TabIndex = 0;
            this.button1.Text = "Migrate";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ShowMigrateForm);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(287, 100);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(140, 138);
            this.button2.TabIndex = 1;
            this.button2.Text = "Download and Migrate";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.ShowDownloadMigrateForm);
            // 
            // lnklblOpts
            // 
            this.lnklblOpts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnklblOpts.AutoSize = true;
            this.lnklblOpts.Location = new System.Drawing.Point(431, 329);
            this.lnklblOpts.Name = "lnklblOpts";
            this.lnklblOpts.Size = new System.Drawing.Size(49, 15);
            this.lnklblOpts.TabIndex = 2;
            this.lnklblOpts.TabStop = true;
            this.lnklblOpts.Text = "Options";
            this.lnklblOpts.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ShowOptionsForm);
            // 
            // lnklblAbout
            // 
            this.lnklblAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnklblAbout.AutoSize = true;
            this.lnklblAbout.Location = new System.Drawing.Point(362, 329);
            this.lnklblAbout.Name = "lnklblAbout";
            this.lnklblAbout.Size = new System.Drawing.Size(49, 15);
            this.lnklblAbout.TabIndex = 3;
            this.lnklblAbout.TabStop = true;
            this.lnklblAbout.Text = "About...";
            this.lnklblAbout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ShowAboutForm);
            // 
            // HomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 363);
            this.Controls.Add(this.lnklblAbout);
            this.Controls.Add(this.lnklblOpts);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "HomeForm";
            this.Text = "HomeForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.LinkLabel lnklblOpts;
        private System.Windows.Forms.LinkLabel lnklblAbout;
    }
}