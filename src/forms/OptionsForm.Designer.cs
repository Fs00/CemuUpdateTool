namespace CemuUpdateTool
{
    partial class OptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            this.chkBoxControllerProfiles = new System.Windows.Forms.CheckBox();
            this.grpBoxMigrationOpts = new System.Windows.Forms.GroupBox();
            this.txtBoxCustomMlc01Path = new System.Windows.Forms.TextBox();
            this.chkBoxCustomMlc01Path = new System.Windows.Forms.CheckBox();
            this.chkBoxCemuSettings = new System.Windows.Forms.CheckBox();
            this.chkBoxDeletePrevContent = new System.Windows.Forms.CheckBox();
            this.chkBoxShaderCaches = new System.Windows.Forms.CheckBox();
            this.chkBoxDLCUpds = new System.Windows.Forms.CheckBox();
            this.chkBoxSavegames = new System.Windows.Forms.CheckBox();
            this.chkBoxGfxPacks = new System.Windows.Forms.CheckBox();
            this.chkBoxGameProfiles = new System.Windows.Forms.CheckBox();
            this.grpBoxProgramOpts = new System.Windows.Forms.GroupBox();
            this.chkBoxSettingsOnFile = new System.Windows.Forms.CheckBox();
            this.btnDeleteSettingsFile = new System.Windows.Forms.Button();
            this.radioBtnAppDataFolder = new System.Windows.Forms.RadioButton();
            this.radioBtnExecFolder = new System.Windows.Forms.RadioButton();
            this.lblFileLocation = new System.Windows.Forms.Label();
            this.btnSaveOpts = new System.Windows.Forms.Button();
            this.btnDiscard = new System.Windows.Forms.Button();
            this.toolTipInfo = new System.Windows.Forms.ToolTip(this.components);
            this.btnRestoreDefaultOpts = new System.Windows.Forms.Button();
            this.errProviderMlcFolder = new System.Windows.Forms.ErrorProvider(this.components);
            this.grpBoxMigrationOpts.SuspendLayout();
            this.grpBoxProgramOpts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderMlcFolder)).BeginInit();
            this.SuspendLayout();
            // 
            // chkBoxControllerProfiles
            // 
            this.chkBoxControllerProfiles.AutoSize = true;
            this.chkBoxControllerProfiles.Location = new System.Drawing.Point(17, 23);
            this.chkBoxControllerProfiles.Name = "chkBoxControllerProfiles";
            this.chkBoxControllerProfiles.Size = new System.Drawing.Size(130, 17);
            this.chkBoxControllerProfiles.TabIndex = 0;
            this.chkBoxControllerProfiles.Text = "Copy controllerProfiles";
            this.chkBoxControllerProfiles.UseVisualStyleBackColor = true;
            // 
            // grpBoxMigrationOpts
            // 
            this.grpBoxMigrationOpts.Controls.Add(this.txtBoxCustomMlc01Path);
            this.grpBoxMigrationOpts.Controls.Add(this.chkBoxCustomMlc01Path);
            this.grpBoxMigrationOpts.Controls.Add(this.chkBoxCemuSettings);
            this.grpBoxMigrationOpts.Controls.Add(this.chkBoxDeletePrevContent);
            this.grpBoxMigrationOpts.Controls.Add(this.chkBoxShaderCaches);
            this.grpBoxMigrationOpts.Controls.Add(this.chkBoxDLCUpds);
            this.grpBoxMigrationOpts.Controls.Add(this.chkBoxSavegames);
            this.grpBoxMigrationOpts.Controls.Add(this.chkBoxGfxPacks);
            this.grpBoxMigrationOpts.Controls.Add(this.chkBoxGameProfiles);
            this.grpBoxMigrationOpts.Controls.Add(this.chkBoxControllerProfiles);
            this.grpBoxMigrationOpts.Location = new System.Drawing.Point(12, 12);
            this.grpBoxMigrationOpts.Name = "grpBoxMigrationOpts";
            this.grpBoxMigrationOpts.Size = new System.Drawing.Size(285, 269);
            this.grpBoxMigrationOpts.TabIndex = 1;
            this.grpBoxMigrationOpts.TabStop = false;
            this.grpBoxMigrationOpts.Text = "Migration options";
            // 
            // txtBoxCustomMlc01Path
            // 
            this.txtBoxCustomMlc01Path.Location = new System.Drawing.Point(36, 235);
            this.txtBoxCustomMlc01Path.Name = "txtBoxCustomMlc01Path";
            this.txtBoxCustomMlc01Path.Size = new System.Drawing.Size(242, 20);
            this.txtBoxCustomMlc01Path.TabIndex = 9;
            this.txtBoxCustomMlc01Path.TextChanged += new System.EventHandler(this.txtBoxCustomMlc01Path_TextChanged);
            // 
            // chkBoxCustomMlc01Path
            // 
            this.chkBoxCustomMlc01Path.AutoSize = true;
            this.chkBoxCustomMlc01Path.Location = new System.Drawing.Point(17, 214);
            this.chkBoxCustomMlc01Path.Name = "chkBoxCustomMlc01Path";
            this.chkBoxCustomMlc01Path.Size = new System.Drawing.Size(211, 17);
            this.chkBoxCustomMlc01Path.TabIndex = 8;
            this.chkBoxCustomMlc01Path.Text = "Use custom mlc01 folder path (v1.10+):";
            this.toolTipInfo.SetToolTip(this.chkBoxCustomMlc01Path, resources.GetString("chkBoxCustomMlc01Path.ToolTip"));
            this.chkBoxCustomMlc01Path.UseVisualStyleBackColor = true;
            this.chkBoxCustomMlc01Path.CheckedChanged += new System.EventHandler(this.chkBoxCustomMlc01Path_CheckedChanged);
            // 
            // chkBoxCemuSettings
            // 
            this.chkBoxCemuSettings.AutoSize = true;
            this.chkBoxCemuSettings.Location = new System.Drawing.Point(17, 161);
            this.chkBoxCemuSettings.Name = "chkBoxCemuSettings";
            this.chkBoxCemuSettings.Size = new System.Drawing.Size(105, 17);
            this.chkBoxCemuSettings.TabIndex = 6;
            this.chkBoxCemuSettings.Text = "Copy settings file";
            this.chkBoxCemuSettings.UseVisualStyleBackColor = true;
            // 
            // chkBoxDeletePrevContent
            // 
            this.chkBoxDeletePrevContent.AutoSize = true;
            this.chkBoxDeletePrevContent.Location = new System.Drawing.Point(17, 191);
            this.chkBoxDeletePrevContent.Name = "chkBoxDeletePrevContent";
            this.chkBoxDeletePrevContent.Size = new System.Drawing.Size(262, 17);
            this.chkBoxDeletePrevContent.TabIndex = 7;
            this.chkBoxDeletePrevContent.Text = "Delete destination folder contents before migration";
            this.chkBoxDeletePrevContent.UseVisualStyleBackColor = true;
            // 
            // chkBoxShaderCaches
            // 
            this.chkBoxShaderCaches.AutoSize = true;
            this.chkBoxShaderCaches.Location = new System.Drawing.Point(17, 138);
            this.chkBoxShaderCaches.Name = "chkBoxShaderCaches";
            this.chkBoxShaderCaches.Size = new System.Drawing.Size(181, 17);
            this.chkBoxShaderCaches.TabIndex = 5;
            this.chkBoxShaderCaches.Text = "Copy shader cache transferables";
            this.chkBoxShaderCaches.UseVisualStyleBackColor = true;
            // 
            // chkBoxDLCUpds
            // 
            this.chkBoxDLCUpds.AutoSize = true;
            this.chkBoxDLCUpds.Location = new System.Drawing.Point(17, 115);
            this.chkBoxDLCUpds.Name = "chkBoxDLCUpds";
            this.chkBoxDLCUpds.Size = new System.Drawing.Size(136, 17);
            this.chkBoxDLCUpds.TabIndex = 4;
            this.chkBoxDLCUpds.Text = "Copy DLC and updates";
            this.chkBoxDLCUpds.UseVisualStyleBackColor = true;
            // 
            // chkBoxSavegames
            // 
            this.chkBoxSavegames.AutoSize = true;
            this.chkBoxSavegames.Location = new System.Drawing.Point(17, 92);
            this.chkBoxSavegames.Name = "chkBoxSavegames";
            this.chkBoxSavegames.Size = new System.Drawing.Size(107, 17);
            this.chkBoxSavegames.TabIndex = 3;
            this.chkBoxSavegames.Text = "Copy savegames";
            this.chkBoxSavegames.UseVisualStyleBackColor = true;
            // 
            // chkBoxGfxPacks
            // 
            this.chkBoxGfxPacks.AutoSize = true;
            this.chkBoxGfxPacks.Location = new System.Drawing.Point(17, 69);
            this.chkBoxGfxPacks.Name = "chkBoxGfxPacks";
            this.chkBoxGfxPacks.Size = new System.Drawing.Size(118, 17);
            this.chkBoxGfxPacks.TabIndex = 2;
            this.chkBoxGfxPacks.Text = "Copy graphicPacks";
            this.chkBoxGfxPacks.UseVisualStyleBackColor = true;
            // 
            // chkBoxGameProfiles
            // 
            this.chkBoxGameProfiles.AutoSize = true;
            this.chkBoxGameProfiles.Location = new System.Drawing.Point(17, 46);
            this.chkBoxGameProfiles.Name = "chkBoxGameProfiles";
            this.chkBoxGameProfiles.Size = new System.Drawing.Size(113, 17);
            this.chkBoxGameProfiles.TabIndex = 1;
            this.chkBoxGameProfiles.Text = "Copy gameProfiles";
            this.chkBoxGameProfiles.UseVisualStyleBackColor = true;
            // 
            // grpBoxProgramOpts
            // 
            this.grpBoxProgramOpts.Controls.Add(this.chkBoxSettingsOnFile);
            this.grpBoxProgramOpts.Controls.Add(this.btnDeleteSettingsFile);
            this.grpBoxProgramOpts.Controls.Add(this.radioBtnAppDataFolder);
            this.grpBoxProgramOpts.Controls.Add(this.radioBtnExecFolder);
            this.grpBoxProgramOpts.Controls.Add(this.lblFileLocation);
            this.grpBoxProgramOpts.Location = new System.Drawing.Point(12, 289);
            this.grpBoxProgramOpts.Name = "grpBoxProgramOpts";
            this.grpBoxProgramOpts.Size = new System.Drawing.Size(284, 124);
            this.grpBoxProgramOpts.TabIndex = 2;
            this.grpBoxProgramOpts.TabStop = false;
            this.grpBoxProgramOpts.Text = "Program options";
            // 
            // chkBoxSettingsOnFile
            // 
            this.chkBoxSettingsOnFile.AutoSize = true;
            this.chkBoxSettingsOnFile.Location = new System.Drawing.Point(17, 24);
            this.chkBoxSettingsOnFile.Name = "chkBoxSettingsOnFile";
            this.chkBoxSettingsOnFile.Size = new System.Drawing.Size(126, 17);
            this.chkBoxSettingsOnFile.TabIndex = 1;
            this.chkBoxSettingsOnFile.Text = "Store settings in a file";
            this.chkBoxSettingsOnFile.UseVisualStyleBackColor = true;
            this.chkBoxSettingsOnFile.CheckedChanged += new System.EventHandler(this.chkBoxSettingsOnFile_CheckedChanged);
            // 
            // btnDeleteSettingsFile
            // 
            this.btnDeleteSettingsFile.Location = new System.Drawing.Point(135, 91);
            this.btnDeleteSettingsFile.Name = "btnDeleteSettingsFile";
            this.btnDeleteSettingsFile.Size = new System.Drawing.Size(143, 23);
            this.btnDeleteSettingsFile.TabIndex = 4;
            this.btnDeleteSettingsFile.Text = "Delete current settings file";
            this.btnDeleteSettingsFile.UseVisualStyleBackColor = true;
            this.btnDeleteSettingsFile.Click += new System.EventHandler(this.btnDeleteSettingsFile_Click);
            // 
            // radioBtnAppDataFolder
            // 
            this.radioBtnAppDataFolder.AutoSize = true;
            this.radioBtnAppDataFolder.Location = new System.Drawing.Point(141, 67);
            this.radioBtnAppDataFolder.Name = "radioBtnAppDataFolder";
            this.radioBtnAppDataFolder.Size = new System.Drawing.Size(112, 17);
            this.radioBtnAppDataFolder.TabIndex = 3;
            this.radioBtnAppDataFolder.Text = "%AppData% folder";
            this.toolTipInfo.SetToolTip(this.radioBtnAppDataFolder, "(%AppData%\\Fs00\\CemuUpdateTool)");
            this.radioBtnAppDataFolder.UseVisualStyleBackColor = true;
            // 
            // radioBtnExecFolder
            // 
            this.radioBtnExecFolder.AutoSize = true;
            this.radioBtnExecFolder.Checked = true;
            this.radioBtnExecFolder.Location = new System.Drawing.Point(31, 67);
            this.radioBtnExecFolder.Name = "radioBtnExecFolder";
            this.radioBtnExecFolder.Size = new System.Drawing.Size(107, 17);
            this.radioBtnExecFolder.TabIndex = 2;
            this.radioBtnExecFolder.TabStop = true;
            this.radioBtnExecFolder.Text = "Executable folder";
            this.radioBtnExecFolder.UseVisualStyleBackColor = true;
            this.radioBtnExecFolder.CheckedChanged += new System.EventHandler(this.radioBtnExecFolder_CheckedChanged);
            // 
            // lblFileLocation
            // 
            this.lblFileLocation.AutoSize = true;
            this.lblFileLocation.Location = new System.Drawing.Point(14, 48);
            this.lblFileLocation.Name = "lblFileLocation";
            this.lblFileLocation.Size = new System.Drawing.Size(104, 13);
            this.lblFileLocation.TabIndex = 0;
            this.lblFileLocation.Text = "Settings file location:";
            // 
            // btnSaveOpts
            // 
            this.btnSaveOpts.Location = new System.Drawing.Point(113, 420);
            this.btnSaveOpts.Name = "btnSaveOpts";
            this.btnSaveOpts.Size = new System.Drawing.Size(81, 25);
            this.btnSaveOpts.TabIndex = 3;
            this.btnSaveOpts.Text = "Save options";
            this.btnSaveOpts.UseVisualStyleBackColor = true;
            this.btnSaveOpts.Click += new System.EventHandler(this.btnSaveOpts_Click);
            // 
            // btnDiscard
            // 
            this.btnDiscard.Location = new System.Drawing.Point(200, 420);
            this.btnDiscard.Name = "btnDiscard";
            this.btnDiscard.Size = new System.Drawing.Size(96, 25);
            this.btnDiscard.TabIndex = 4;
            this.btnDiscard.Text = "Discard changes";
            this.btnDiscard.UseVisualStyleBackColor = true;
            this.btnDiscard.Click += new System.EventHandler(this.btnDiscard_Click);
            // 
            // toolTipInfo
            // 
            this.toolTipInfo.AutoPopDelay = 12000;
            this.toolTipInfo.InitialDelay = 300;
            this.toolTipInfo.ReshowDelay = 100;
            // 
            // btnRestoreDefaultOpts
            // 
            this.btnRestoreDefaultOpts.Location = new System.Drawing.Point(12, 420);
            this.btnRestoreDefaultOpts.Name = "btnRestoreDefaultOpts";
            this.btnRestoreDefaultOpts.Size = new System.Drawing.Size(95, 25);
            this.btnRestoreDefaultOpts.TabIndex = 5;
            this.btnRestoreDefaultOpts.Text = "Restore defaults";
            this.btnRestoreDefaultOpts.UseVisualStyleBackColor = true;
            this.btnRestoreDefaultOpts.Click += new System.EventHandler(this.btnRestoreDefaultOpts_Click);
            // 
            // errProviderMlcFolder
            // 
            this.errProviderMlcFolder.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errProviderMlcFolder.ContainerControl = this;
            this.errProviderMlcFolder.SetIconPadding(txtBoxCustomMlc01Path, -20);
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 453);
            this.Controls.Add(this.btnRestoreDefaultOpts);
            this.Controls.Add(this.btnDiscard);
            this.Controls.Add(this.btnSaveOpts);
            this.Controls.Add(this.grpBoxProgramOpts);
            this.Controls.Add(this.grpBoxMigrationOpts);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "OptionsForm";
            this.Text = "Options";
            this.grpBoxMigrationOpts.ResumeLayout(false);
            this.grpBoxMigrationOpts.PerformLayout();
            this.grpBoxProgramOpts.ResumeLayout(false);
            this.grpBoxProgramOpts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderMlcFolder)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkBoxControllerProfiles;
        private System.Windows.Forms.GroupBox grpBoxMigrationOpts;
        private System.Windows.Forms.CheckBox chkBoxDeletePrevContent;
        private System.Windows.Forms.CheckBox chkBoxShaderCaches;
        private System.Windows.Forms.CheckBox chkBoxDLCUpds;
        private System.Windows.Forms.CheckBox chkBoxSavegames;
        private System.Windows.Forms.CheckBox chkBoxGfxPacks;
        private System.Windows.Forms.CheckBox chkBoxGameProfiles;
        private System.Windows.Forms.GroupBox grpBoxProgramOpts;
        private System.Windows.Forms.Button btnDeleteSettingsFile;
        private System.Windows.Forms.RadioButton radioBtnAppDataFolder;
        private System.Windows.Forms.RadioButton radioBtnExecFolder;
        private System.Windows.Forms.Label lblFileLocation;
        private System.Windows.Forms.Button btnSaveOpts;
        private System.Windows.Forms.Button btnDiscard;
        private System.Windows.Forms.ToolTip toolTipInfo;
        private System.Windows.Forms.CheckBox chkBoxSettingsOnFile;
        private System.Windows.Forms.CheckBox chkBoxCemuSettings;
        private System.Windows.Forms.TextBox txtBoxCustomMlc01Path;
        private System.Windows.Forms.CheckBox chkBoxCustomMlc01Path;
        private System.Windows.Forms.Button btnRestoreDefaultOpts;
        private System.Windows.Forms.ErrorProvider errProviderMlcFolder;
    }
}