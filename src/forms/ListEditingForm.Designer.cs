namespace CemuUpdateTool
{
    partial class ListEditingForm<T>
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
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnEnableAll = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.lstView = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(283, 12);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Add...";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.AddElement);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Location = new System.Drawing.Point(283, 41);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Text = "Remove...";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.RemoveElement);
            // 
            // btnEnableAll
            // 
            this.btnEnableAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEnableAll.Location = new System.Drawing.Point(283, 70);
            this.btnEnableAll.Name = "btnEnableAll";
            this.btnEnableAll.Size = new System.Drawing.Size(75, 23);
            this.btnEnableAll.TabIndex = 2;
            this.btnEnableAll.Text = "Enable all";
            this.btnEnableAll.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(283, 99);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 3;
            this.button4.Text = "Disable all";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // lstView
            // 
            this.lstView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstView.Location = new System.Drawing.Point(12, 12);
            this.lstView.MultiSelect = false;
            this.lstView.Name = "lstView";
            this.lstView.Size = new System.Drawing.Size(265, 199);
            this.lstView.TabIndex = 4;
            this.lstView.UseCompatibleStateImageBehavior = false;
            // 
            // ListEditingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(370, 223);
            this.Controls.Add(this.lstView);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.btnEnableAll);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(275, 175);
            this.Name = "ListEditingForm";
            this.Text = "Manage custom entries";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnEnableAll;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ListView lstView;
    }
}