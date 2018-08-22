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
            this.grpBoxProgramOpts = new System.Windows.Forms.GroupBox();
            this.chkBoxSettingsOnFile = new System.Windows.Forms.CheckBox();
            this.btnDeleteSettingsFile = new System.Windows.Forms.Button();
            this.radioBtnAppDataFolder = new System.Windows.Forms.RadioButton();
            this.radioBtnExecFolder = new System.Windows.Forms.RadioButton();
            this.lblFileLocation = new System.Windows.Forms.Label();
            this.btnSaveOpts = new System.Windows.Forms.Button();
            this.btnDiscard = new System.Windows.Forms.Button();
            this.toolTipInfo = new System.Windows.Forms.ToolTip(this.components);
            this.chkBoxCustomMlc01Path = new System.Windows.Forms.CheckBox();
            this.btnRestoreDefaultOpts = new System.Windows.Forms.Button();
            this.errProviderMlcFolder = new System.Windows.Forms.ErrorProvider(this.components);
            this.txtBoxCustomMlc01Path = new System.Windows.Forms.TextBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.foldersTab = new System.Windows.Forms.TabPage();
            this.grpBoxCustomEntries = new System.Windows.Forms.GroupBox();
            this.lblTotalCustomFiles = new System.Windows.Forms.Label();
            this.lblCustomFilesCnt = new System.Windows.Forms.Label();
            this.btnManageFolders = new System.Windows.Forms.Button();
            this.lblTotalCustomFolders = new System.Windows.Forms.Label();
            this.lblCustomFoldersCnt = new System.Windows.Forms.Label();
            this.btnManageFiles = new System.Windows.Forms.Button();
            this.chkBoxControllerProfiles = new System.Windows.Forms.CheckBox();
            this.chkBoxCemuSettings = new System.Windows.Forms.CheckBox();
            this.chkBoxGameProfiles = new System.Windows.Forms.CheckBox();
            this.chkBoxGfxPacks = new System.Windows.Forms.CheckBox();
            this.chkBoxShaderCaches = new System.Windows.Forms.CheckBox();
            this.chkBoxSavegames = new System.Windows.Forms.CheckBox();
            this.chkBoxDLCUpds = new System.Windows.Forms.CheckBox();
            this.featuresTab = new System.Windows.Forms.TabPage();
            this.grpBoxMigration = new System.Windows.Forms.GroupBox();
            this.chkBoxNoFullscreenOptimiz = new System.Windows.Forms.CheckBox();
            this.chkBoxOverrideHiDPIBehaviour = new System.Windows.Forms.CheckBox();
            this.chkBoxRunAsAdmin = new System.Windows.Forms.CheckBox();
            this.chkBoxCompatOpts = new System.Windows.Forms.CheckBox();
            this.chkBoxDeletePrevContent = new System.Windows.Forms.CheckBox();
            this.chkBoxDesktopShortcut = new System.Windows.Forms.CheckBox();
            this.downloadTab = new System.Windows.Forms.TabPage();
            this.lblHttp = new System.Windows.Forms.Label();
            this.lblUriError = new System.Windows.Forms.Label();
            this.lblSampleVersion = new System.Windows.Forms.Label();
            this.txtBoxUrlSuffix = new System.Windows.Forms.TextBox();
            this.txtBoxBaseUrl = new System.Windows.Forms.TextBox();
            this.lblUrl = new System.Windows.Forms.Label();
            this.lblWarning = new System.Windows.Forms.Label();
            this.btnHelp = new System.Windows.Forms.Button();
            this.grpBoxProgramOpts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderMlcFolder)).BeginInit();
            this.tabControl.SuspendLayout();
            this.foldersTab.SuspendLayout();
            this.grpBoxCustomEntries.SuspendLayout();
            this.featuresTab.SuspendLayout();
            this.grpBoxMigration.SuspendLayout();
            this.downloadTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpBoxProgramOpts
            // 
            this.grpBoxProgramOpts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grpBoxProgramOpts.Controls.Add(this.chkBoxSettingsOnFile);
            this.grpBoxProgramOpts.Controls.Add(this.btnDeleteSettingsFile);
            this.grpBoxProgramOpts.Controls.Add(this.radioBtnAppDataFolder);
            this.grpBoxProgramOpts.Controls.Add(this.radioBtnExecFolder);
            this.grpBoxProgramOpts.Controls.Add(this.lblFileLocation);
            this.grpBoxProgramOpts.Location = new System.Drawing.Point(14, 216);
            this.grpBoxProgramOpts.Name = "grpBoxProgramOpts";
            this.grpBoxProgramOpts.Size = new System.Drawing.Size(342, 143);
            this.grpBoxProgramOpts.TabIndex = 2;
            this.grpBoxProgramOpts.TabStop = false;
            this.grpBoxProgramOpts.Text = "Program";
            // 
            // chkBoxSettingsOnFile
            // 
            this.chkBoxSettingsOnFile.AutoSize = true;
            this.chkBoxSettingsOnFile.Location = new System.Drawing.Point(20, 28);
            this.chkBoxSettingsOnFile.Name = "chkBoxSettingsOnFile";
            this.chkBoxSettingsOnFile.Size = new System.Drawing.Size(138, 19);
            this.chkBoxSettingsOnFile.TabIndex = 1;
            this.chkBoxSettingsOnFile.Text = "Store settings in a file";
            this.chkBoxSettingsOnFile.UseVisualStyleBackColor = true;
            this.chkBoxSettingsOnFile.CheckedChanged += new System.EventHandler(this.UpdateOptionsFileLocationRadioButtons);
            // 
            // btnDeleteSettingsFile
            // 
            this.btnDeleteSettingsFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteSettingsFile.Location = new System.Drawing.Point(167, 105);
            this.btnDeleteSettingsFile.Name = "btnDeleteSettingsFile";
            this.btnDeleteSettingsFile.Size = new System.Drawing.Size(167, 27);
            this.btnDeleteSettingsFile.TabIndex = 4;
            this.btnDeleteSettingsFile.Text = "Delete current settings file";
            this.btnDeleteSettingsFile.UseVisualStyleBackColor = true;
            this.btnDeleteSettingsFile.Click += new System.EventHandler(this.DeleteSettingsFile);
            // 
            // radioBtnAppDataFolder
            // 
            this.radioBtnAppDataFolder.AutoSize = true;
            this.radioBtnAppDataFolder.Location = new System.Drawing.Point(164, 77);
            this.radioBtnAppDataFolder.Name = "radioBtnAppDataFolder";
            this.radioBtnAppDataFolder.Size = new System.Drawing.Size(125, 19);
            this.radioBtnAppDataFolder.TabIndex = 3;
            this.radioBtnAppDataFolder.Text = "%AppData% folder";
            this.toolTipInfo.SetToolTip(this.radioBtnAppDataFolder, "(%AppData%\\Fs00\\CemuUpdateTool)");
            this.radioBtnAppDataFolder.UseVisualStyleBackColor = true;
            // 
            // radioBtnExecFolder
            // 
            this.radioBtnExecFolder.AutoSize = true;
            this.radioBtnExecFolder.Checked = true;
            this.radioBtnExecFolder.Location = new System.Drawing.Point(36, 77);
            this.radioBtnExecFolder.Name = "radioBtnExecFolder";
            this.radioBtnExecFolder.Size = new System.Drawing.Size(115, 19);
            this.radioBtnExecFolder.TabIndex = 2;
            this.radioBtnExecFolder.TabStop = true;
            this.radioBtnExecFolder.Text = "Executable folder";
            this.radioBtnExecFolder.UseVisualStyleBackColor = true;
            this.radioBtnExecFolder.CheckedChanged += new System.EventHandler(this.CheckIfOptionsFileLocationHasChanged);
            // 
            // lblFileLocation
            // 
            this.lblFileLocation.AutoSize = true;
            this.lblFileLocation.Location = new System.Drawing.Point(16, 55);
            this.lblFileLocation.Name = "lblFileLocation";
            this.lblFileLocation.Size = new System.Drawing.Size(117, 15);
            this.lblFileLocation.TabIndex = 0;
            this.lblFileLocation.Text = "Settings file location:";
            // 
            // btnSaveOpts
            // 
            this.btnSaveOpts.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSaveOpts.Location = new System.Drawing.Point(157, 418);
            this.btnSaveOpts.Name = "btnSaveOpts";
            this.btnSaveOpts.Size = new System.Drawing.Size(94, 29);
            this.btnSaveOpts.TabIndex = 3;
            this.btnSaveOpts.Text = "Save options";
            this.btnSaveOpts.UseVisualStyleBackColor = true;
            this.btnSaveOpts.Click += new System.EventHandler(this.SaveOptionsAndClose);
            // 
            // btnDiscard
            // 
            this.btnDiscard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDiscard.Location = new System.Drawing.Point(281, 418);
            this.btnDiscard.Name = "btnDiscard";
            this.btnDiscard.Size = new System.Drawing.Size(112, 29);
            this.btnDiscard.TabIndex = 4;
            this.btnDiscard.Text = "Discard changes";
            this.btnDiscard.UseVisualStyleBackColor = true;
            this.btnDiscard.Click += new System.EventHandler(this.DiscardAndClose);
            // 
            // toolTipInfo
            // 
            this.toolTipInfo.AutoPopDelay = 12000;
            this.toolTipInfo.InitialDelay = 300;
            this.toolTipInfo.ReshowDelay = 100;
            // 
            // chkBoxCustomMlc01Path
            // 
            this.chkBoxCustomMlc01Path.AutoSize = true;
            this.chkBoxCustomMlc01Path.Location = new System.Drawing.Point(18, 201);
            this.chkBoxCustomMlc01Path.Name = "chkBoxCustomMlc01Path";
            this.chkBoxCustomMlc01Path.Size = new System.Drawing.Size(233, 19);
            this.chkBoxCustomMlc01Path.TabIndex = 8;
            this.chkBoxCustomMlc01Path.Text = "Use custom mlc01 folder path (v1.10+):";
            this.toolTipInfo.SetToolTip(this.chkBoxCustomMlc01Path, resources.GetString("chkBoxCustomMlc01Path.ToolTip"));
            this.chkBoxCustomMlc01Path.UseVisualStyleBackColor = true;
            this.chkBoxCustomMlc01Path.CheckedChanged += new System.EventHandler(this.UpdateCustomMlc01PathTextboxState);
            // 
            // btnRestoreDefaultOpts
            // 
            this.btnRestoreDefaultOpts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRestoreDefaultOpts.Location = new System.Drawing.Point(13, 418);
            this.btnRestoreDefaultOpts.Name = "btnRestoreDefaultOpts";
            this.btnRestoreDefaultOpts.Size = new System.Drawing.Size(111, 29);
            this.btnRestoreDefaultOpts.TabIndex = 5;
            this.btnRestoreDefaultOpts.Text = "Restore defaults";
            this.btnRestoreDefaultOpts.UseVisualStyleBackColor = true;
            this.btnRestoreDefaultOpts.Click += new System.EventHandler(this.RestoreDefaultOptions);
            // 
            // errProviderMlcFolder
            // 
            this.errProviderMlcFolder.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errProviderMlcFolder.ContainerControl = this;
            // 
            // txtBoxCustomMlc01Path
            // 
            this.errProviderMlcFolder.SetIconPadding(this.txtBoxCustomMlc01Path, -20);
            this.txtBoxCustomMlc01Path.Location = new System.Drawing.Point(38, 226);
            this.txtBoxCustomMlc01Path.Name = "txtBoxCustomMlc01Path";
            this.txtBoxCustomMlc01Path.Size = new System.Drawing.Size(315, 23);
            this.txtBoxCustomMlc01Path.TabIndex = 9;
            this.txtBoxCustomMlc01Path.TextChanged += new System.EventHandler(this.CheckCustomMlc01PathForInvalidChars);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.foldersTab);
            this.tabControl.Controls.Add(this.featuresTab);
            this.tabControl.Controls.Add(this.downloadTab);
            this.tabControl.Location = new System.Drawing.Point(14, 9);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(381, 401);
            this.tabControl.TabIndex = 6;
            // 
            // foldersTab
            // 
            this.foldersTab.Controls.Add(this.grpBoxCustomEntries);
            this.foldersTab.Controls.Add(this.txtBoxCustomMlc01Path);
            this.foldersTab.Controls.Add(this.chkBoxCustomMlc01Path);
            this.foldersTab.Controls.Add(this.chkBoxControllerProfiles);
            this.foldersTab.Controls.Add(this.chkBoxCemuSettings);
            this.foldersTab.Controls.Add(this.chkBoxGameProfiles);
            this.foldersTab.Controls.Add(this.chkBoxGfxPacks);
            this.foldersTab.Controls.Add(this.chkBoxShaderCaches);
            this.foldersTab.Controls.Add(this.chkBoxSavegames);
            this.foldersTab.Controls.Add(this.chkBoxDLCUpds);
            this.foldersTab.Location = new System.Drawing.Point(4, 24);
            this.foldersTab.Name = "foldersTab";
            this.foldersTab.Padding = new System.Windows.Forms.Padding(3);
            this.foldersTab.Size = new System.Drawing.Size(373, 373);
            this.foldersTab.TabIndex = 0;
            this.foldersTab.Text = "Folders";
            this.foldersTab.UseVisualStyleBackColor = true;
            // 
            // grpBoxCustomEntries
            // 
            this.grpBoxCustomEntries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grpBoxCustomEntries.Controls.Add(this.lblTotalCustomFiles);
            this.grpBoxCustomEntries.Controls.Add(this.lblCustomFilesCnt);
            this.grpBoxCustomEntries.Controls.Add(this.btnManageFolders);
            this.grpBoxCustomEntries.Controls.Add(this.lblTotalCustomFolders);
            this.grpBoxCustomEntries.Controls.Add(this.lblCustomFoldersCnt);
            this.grpBoxCustomEntries.Controls.Add(this.btnManageFiles);
            this.grpBoxCustomEntries.Location = new System.Drawing.Point(18, 267);
            this.grpBoxCustomEntries.Name = "grpBoxCustomEntries";
            this.grpBoxCustomEntries.Size = new System.Drawing.Size(335, 92);
            this.grpBoxCustomEntries.TabIndex = 18;
            this.grpBoxCustomEntries.TabStop = false;
            this.grpBoxCustomEntries.Text = "Custom folders and files";
            // 
            // lblTotalCustomFiles
            // 
            this.lblTotalCustomFiles.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTotalCustomFiles.AutoSize = true;
            this.lblTotalCustomFiles.Location = new System.Drawing.Point(216, 62);
            this.lblTotalCustomFiles.Name = "lblTotalCustomFiles";
            this.lblTotalCustomFiles.Size = new System.Drawing.Size(36, 15);
            this.lblTotalCustomFiles.TabIndex = 17;
            this.lblTotalCustomFiles.Text = "Total:";
            // 
            // lblCustomFilesCnt
            // 
            this.lblCustomFilesCnt.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCustomFilesCnt.AutoSize = true;
            this.lblCustomFilesCnt.Location = new System.Drawing.Point(261, 62);
            this.lblCustomFilesCnt.Name = "lblCustomFilesCnt";
            this.lblCustomFilesCnt.Size = new System.Drawing.Size(13, 15);
            this.lblCustomFilesCnt.TabIndex = 18;
            this.lblCustomFilesCnt.Text = "0";
            // 
            // btnManageFolders
            // 
            this.btnManageFolders.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnManageFolders.Location = new System.Drawing.Point(20, 28);
            this.btnManageFolders.Name = "btnManageFolders";
            this.btnManageFolders.Size = new System.Drawing.Size(142, 27);
            this.btnManageFolders.TabIndex = 14;
            this.btnManageFolders.Text = "Manage custom folders";
            this.btnManageFolders.UseVisualStyleBackColor = true;
            this.btnManageFolders.Click += new System.EventHandler(this.OpenManageFoldersDialog);
            // 
            // lblTotalCustomFolders
            // 
            this.lblTotalCustomFolders.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTotalCustomFolders.AutoSize = true;
            this.lblTotalCustomFolders.Location = new System.Drawing.Point(60, 62);
            this.lblTotalCustomFolders.Name = "lblTotalCustomFolders";
            this.lblTotalCustomFolders.Size = new System.Drawing.Size(36, 15);
            this.lblTotalCustomFolders.TabIndex = 12;
            this.lblTotalCustomFolders.Text = "Total:";
            // 
            // lblCustomFoldersCnt
            // 
            this.lblCustomFoldersCnt.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCustomFoldersCnt.AutoSize = true;
            this.lblCustomFoldersCnt.Location = new System.Drawing.Point(105, 62);
            this.lblCustomFoldersCnt.Name = "lblCustomFoldersCnt";
            this.lblCustomFoldersCnt.Size = new System.Drawing.Size(13, 15);
            this.lblCustomFoldersCnt.TabIndex = 16;
            this.lblCustomFoldersCnt.Text = "0";
            // 
            // btnManageFiles
            // 
            this.btnManageFiles.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnManageFiles.Location = new System.Drawing.Point(174, 28);
            this.btnManageFiles.Name = "btnManageFiles";
            this.btnManageFiles.Size = new System.Drawing.Size(142, 27);
            this.btnManageFiles.TabIndex = 15;
            this.btnManageFiles.Text = "Manage custom files";
            this.btnManageFiles.UseVisualStyleBackColor = true;
            this.btnManageFiles.Click += new System.EventHandler(this.OpenManageFilesDialog);
            // 
            // chkBoxControllerProfiles
            // 
            this.chkBoxControllerProfiles.AutoSize = true;
            this.chkBoxControllerProfiles.Location = new System.Drawing.Point(18, 15);
            this.chkBoxControllerProfiles.Name = "chkBoxControllerProfiles";
            this.chkBoxControllerProfiles.Size = new System.Drawing.Size(147, 19);
            this.chkBoxControllerProfiles.TabIndex = 0;
            this.chkBoxControllerProfiles.Text = "Copy controllerProfiles";
            this.chkBoxControllerProfiles.UseVisualStyleBackColor = true;
            // 
            // chkBoxCemuSettings
            // 
            this.chkBoxCemuSettings.AutoSize = true;
            this.chkBoxCemuSettings.Location = new System.Drawing.Point(18, 165);
            this.chkBoxCemuSettings.Name = "chkBoxCemuSettings";
            this.chkBoxCemuSettings.Size = new System.Drawing.Size(117, 19);
            this.chkBoxCemuSettings.TabIndex = 6;
            this.chkBoxCemuSettings.Text = "Copy settings file";
            this.chkBoxCemuSettings.UseVisualStyleBackColor = true;
            // 
            // chkBoxGameProfiles
            // 
            this.chkBoxGameProfiles.AutoSize = true;
            this.chkBoxGameProfiles.Location = new System.Drawing.Point(18, 40);
            this.chkBoxGameProfiles.Name = "chkBoxGameProfiles";
            this.chkBoxGameProfiles.Size = new System.Drawing.Size(126, 19);
            this.chkBoxGameProfiles.TabIndex = 1;
            this.chkBoxGameProfiles.Text = "Copy gameProfiles";
            this.chkBoxGameProfiles.UseVisualStyleBackColor = true;
            // 
            // chkBoxGfxPacks
            // 
            this.chkBoxGfxPacks.AutoSize = true;
            this.chkBoxGfxPacks.Location = new System.Drawing.Point(18, 65);
            this.chkBoxGfxPacks.Name = "chkBoxGfxPacks";
            this.chkBoxGfxPacks.Size = new System.Drawing.Size(127, 19);
            this.chkBoxGfxPacks.TabIndex = 2;
            this.chkBoxGfxPacks.Text = "Copy graphicPacks";
            this.chkBoxGfxPacks.UseVisualStyleBackColor = true;
            // 
            // chkBoxShaderCaches
            // 
            this.chkBoxShaderCaches.AutoSize = true;
            this.chkBoxShaderCaches.Location = new System.Drawing.Point(18, 140);
            this.chkBoxShaderCaches.Name = "chkBoxShaderCaches";
            this.chkBoxShaderCaches.Size = new System.Drawing.Size(196, 19);
            this.chkBoxShaderCaches.TabIndex = 5;
            this.chkBoxShaderCaches.Text = "Copy shader cache transferables";
            this.chkBoxShaderCaches.UseVisualStyleBackColor = true;
            // 
            // chkBoxSavegames
            // 
            this.chkBoxSavegames.AutoSize = true;
            this.chkBoxSavegames.Location = new System.Drawing.Point(18, 90);
            this.chkBoxSavegames.Name = "chkBoxSavegames";
            this.chkBoxSavegames.Size = new System.Drawing.Size(115, 19);
            this.chkBoxSavegames.TabIndex = 3;
            this.chkBoxSavegames.Text = "Copy savegames";
            this.chkBoxSavegames.UseVisualStyleBackColor = true;
            // 
            // chkBoxDLCUpds
            // 
            this.chkBoxDLCUpds.AutoSize = true;
            this.chkBoxDLCUpds.Location = new System.Drawing.Point(18, 115);
            this.chkBoxDLCUpds.Name = "chkBoxDLCUpds";
            this.chkBoxDLCUpds.Size = new System.Drawing.Size(147, 19);
            this.chkBoxDLCUpds.TabIndex = 4;
            this.chkBoxDLCUpds.Text = "Copy DLC and updates";
            this.chkBoxDLCUpds.UseVisualStyleBackColor = true;
            // 
            // featuresTab
            // 
            this.featuresTab.Controls.Add(this.grpBoxMigration);
            this.featuresTab.Controls.Add(this.grpBoxProgramOpts);
            this.featuresTab.Location = new System.Drawing.Point(4, 24);
            this.featuresTab.Name = "featuresTab";
            this.featuresTab.Padding = new System.Windows.Forms.Padding(3);
            this.featuresTab.Size = new System.Drawing.Size(373, 373);
            this.featuresTab.TabIndex = 1;
            this.featuresTab.Text = "Features";
            this.featuresTab.UseVisualStyleBackColor = true;
            // 
            // grpBoxMigration
            // 
            this.grpBoxMigration.Controls.Add(this.chkBoxNoFullscreenOptimiz);
            this.grpBoxMigration.Controls.Add(this.chkBoxOverrideHiDPIBehaviour);
            this.grpBoxMigration.Controls.Add(this.chkBoxRunAsAdmin);
            this.grpBoxMigration.Controls.Add(this.chkBoxCompatOpts);
            this.grpBoxMigration.Controls.Add(this.chkBoxDeletePrevContent);
            this.grpBoxMigration.Controls.Add(this.chkBoxDesktopShortcut);
            this.grpBoxMigration.Location = new System.Drawing.Point(14, 13);
            this.grpBoxMigration.Name = "grpBoxMigration";
            this.grpBoxMigration.Size = new System.Drawing.Size(342, 196);
            this.grpBoxMigration.TabIndex = 10;
            this.grpBoxMigration.TabStop = false;
            this.grpBoxMigration.Text = "Migration";
            // 
            // chkBoxNoFullscreenOptimiz
            // 
            this.chkBoxNoFullscreenOptimiz.AutoSize = true;
            this.chkBoxNoFullscreenOptimiz.Location = new System.Drawing.Point(36, 137);
            this.chkBoxNoFullscreenOptimiz.Name = "chkBoxNoFullscreenOptimiz";
            this.chkBoxNoFullscreenOptimiz.Size = new System.Drawing.Size(193, 19);
            this.chkBoxNoFullscreenOptimiz.TabIndex = 13;
            this.chkBoxNoFullscreenOptimiz.Text = "Disable fullscreen optimizations";
            this.chkBoxNoFullscreenOptimiz.UseVisualStyleBackColor = true;
            // 
            // chkBoxOverrideHiDPIBehaviour
            // 
            this.chkBoxOverrideHiDPIBehaviour.AutoSize = true;
            this.chkBoxOverrideHiDPIBehaviour.Location = new System.Drawing.Point(36, 162);
            this.chkBoxOverrideHiDPIBehaviour.Name = "chkBoxOverrideHiDPIBehaviour";
            this.chkBoxOverrideHiDPIBehaviour.Size = new System.Drawing.Size(200, 19);
            this.chkBoxOverrideHiDPIBehaviour.TabIndex = 12;
            this.chkBoxOverrideHiDPIBehaviour.Text = "Override HiDPI scaling behaviour";
            this.chkBoxOverrideHiDPIBehaviour.UseVisualStyleBackColor = true;
            // 
            // chkBoxRunAsAdmin
            // 
            this.chkBoxRunAsAdmin.AutoSize = true;
            this.chkBoxRunAsAdmin.Location = new System.Drawing.Point(36, 112);
            this.chkBoxRunAsAdmin.Name = "chkBoxRunAsAdmin";
            this.chkBoxRunAsAdmin.Size = new System.Drawing.Size(98, 19);
            this.chkBoxRunAsAdmin.TabIndex = 11;
            this.chkBoxRunAsAdmin.Text = "Run as admin";
            this.chkBoxRunAsAdmin.UseVisualStyleBackColor = true;
            // 
            // chkBoxCompatOpts
            // 
            this.chkBoxCompatOpts.AutoSize = true;
            this.chkBoxCompatOpts.Location = new System.Drawing.Point(15, 88);
            this.chkBoxCompatOpts.Name = "chkBoxCompatOpts";
            this.chkBoxCompatOpts.Size = new System.Drawing.Size(300, 19);
            this.chkBoxCompatOpts.TabIndex = 10;
            this.chkBoxCompatOpts.Text = "Set compatibility options for new Cemu installation:";
            this.chkBoxCompatOpts.UseVisualStyleBackColor = true;
            this.chkBoxCompatOpts.CheckedChanged += new System.EventHandler(this.UpdateCompatOptsCheckboxesState);
            // 
            // chkBoxDeletePrevContent
            // 
            this.chkBoxDeletePrevContent.AutoSize = true;
            this.chkBoxDeletePrevContent.Location = new System.Drawing.Point(15, 28);
            this.chkBoxDeletePrevContent.Name = "chkBoxDeletePrevContent";
            this.chkBoxDeletePrevContent.Size = new System.Drawing.Size(296, 19);
            this.chkBoxDeletePrevContent.TabIndex = 8;
            this.chkBoxDeletePrevContent.Text = "Delete destination folder contents before migration";
            this.chkBoxDeletePrevContent.UseVisualStyleBackColor = true;
            // 
            // chkBoxDesktopShortcut
            // 
            this.chkBoxDesktopShortcut.AutoSize = true;
            this.chkBoxDesktopShortcut.Location = new System.Drawing.Point(15, 53);
            this.chkBoxDesktopShortcut.Name = "chkBoxDesktopShortcut";
            this.chkBoxDesktopShortcut.Size = new System.Drawing.Size(310, 19);
            this.chkBoxDesktopShortcut.TabIndex = 9;
            this.chkBoxDesktopShortcut.Text = "Ask for desktop shortcut once migration is completed";
            this.chkBoxDesktopShortcut.UseVisualStyleBackColor = true;
            // 
            // downloadTab
            // 
            this.downloadTab.Controls.Add(this.lblHttp);
            this.downloadTab.Controls.Add(this.lblUriError);
            this.downloadTab.Controls.Add(this.lblSampleVersion);
            this.downloadTab.Controls.Add(this.txtBoxUrlSuffix);
            this.downloadTab.Controls.Add(this.txtBoxBaseUrl);
            this.downloadTab.Controls.Add(this.lblUrl);
            this.downloadTab.Controls.Add(this.lblWarning);
            this.downloadTab.Location = new System.Drawing.Point(4, 24);
            this.downloadTab.Name = "downloadTab";
            this.downloadTab.Padding = new System.Windows.Forms.Padding(3);
            this.downloadTab.Size = new System.Drawing.Size(373, 373);
            this.downloadTab.TabIndex = 2;
            this.downloadTab.Text = "Download";
            this.downloadTab.UseVisualStyleBackColor = true;
            // 
            // lblHttp
            // 
            this.lblHttp.AutoSize = true;
            this.lblHttp.Location = new System.Drawing.Point(25, 121);
            this.lblHttp.Name = "lblHttp";
            this.lblHttp.Size = new System.Drawing.Size(42, 15);
            this.lblHttp.TabIndex = 6;
            this.lblHttp.Text = "http://";
            // 
            // lblUriError
            // 
            this.lblUriError.ForeColor = System.Drawing.Color.Red;
            this.lblUriError.Location = new System.Drawing.Point(17, 162);
            this.lblUriError.Name = "lblUriError";
            this.lblUriError.Size = new System.Drawing.Size(334, 34);
            this.lblUriError.TabIndex = 5;
            this.lblUriError.Text = "The URL you entered isn\'t valid, thus won\'t be saved when you close the window.";
            // 
            // lblSampleVersion
            // 
            this.lblSampleVersion.AutoSize = true;
            this.lblSampleVersion.Location = new System.Drawing.Point(256, 121);
            this.lblSampleVersion.Name = "lblSampleVersion";
            this.lblSampleVersion.Size = new System.Drawing.Size(34, 15);
            this.lblSampleVersion.TabIndex = 4;
            this.lblSampleVersion.Text = "X.Y.Z";
            // 
            // txtBoxUrlSuffix
            // 
            this.txtBoxUrlSuffix.Location = new System.Drawing.Point(292, 118);
            this.txtBoxUrlSuffix.Name = "txtBoxUrlSuffix";
            this.txtBoxUrlSuffix.Size = new System.Drawing.Size(59, 23);
            this.txtBoxUrlSuffix.TabIndex = 3;
            this.txtBoxUrlSuffix.TextChanged += new System.EventHandler(this.CheckIfDownloadUrlIsValid);
            // 
            // txtBoxBaseUrl
            // 
            this.txtBoxBaseUrl.Location = new System.Drawing.Point(67, 118);
            this.txtBoxBaseUrl.Name = "txtBoxBaseUrl";
            this.txtBoxBaseUrl.Size = new System.Drawing.Size(187, 23);
            this.txtBoxBaseUrl.TabIndex = 2;
            this.txtBoxBaseUrl.TextChanged += new System.EventHandler(this.CheckIfDownloadUrlIsValid);
            // 
            // lblUrl
            // 
            this.lblUrl.AutoSize = true;
            this.lblUrl.Location = new System.Drawing.Point(14, 96);
            this.lblUrl.Name = "lblUrl";
            this.lblUrl.Size = new System.Drawing.Size(122, 15);
            this.lblUrl.TabIndex = 1;
            this.lblUrl.Text = "Cemu download URL:";
            // 
            // lblWarning
            // 
            this.lblWarning.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWarning.Location = new System.Drawing.Point(17, 15);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(334, 81);
            this.lblWarning.TabIndex = 0;
            this.lblWarning.Text = "NOTE: Do not edit the address below unless Cemu repository has changed.\r\nChanging" +
    " this option when not necessary will make the program unable to download new Cem" +
    "u versions!";
            // 
            // btnHelp
            // 
            this.btnHelp.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnHelp.FlatAppearance.BorderSize = 0;
            this.btnHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHelp.Location = new System.Drawing.Point(371, 7);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(23, 23);
            this.btnHelp.TabIndex = 7;
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.OpenHelpForm);
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(407, 457);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnRestoreDefaultOpts);
            this.Controls.Add(this.btnDiscard);
            this.Controls.Add(this.btnSaveOpts);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "OptionsForm";
            this.Text = "Options";
            this.grpBoxProgramOpts.ResumeLayout(false);
            this.grpBoxProgramOpts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errProviderMlcFolder)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.foldersTab.ResumeLayout(false);
            this.foldersTab.PerformLayout();
            this.grpBoxCustomEntries.ResumeLayout(false);
            this.grpBoxCustomEntries.PerformLayout();
            this.featuresTab.ResumeLayout(false);
            this.grpBoxMigration.ResumeLayout(false);
            this.grpBoxMigration.PerformLayout();
            this.downloadTab.ResumeLayout(false);
            this.downloadTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox grpBoxProgramOpts;
        private System.Windows.Forms.Button btnDeleteSettingsFile;
        private System.Windows.Forms.RadioButton radioBtnAppDataFolder;
        private System.Windows.Forms.RadioButton radioBtnExecFolder;
        private System.Windows.Forms.Label lblFileLocation;
        private System.Windows.Forms.Button btnSaveOpts;
        private System.Windows.Forms.Button btnDiscard;
        private System.Windows.Forms.ToolTip toolTipInfo;
        private System.Windows.Forms.CheckBox chkBoxSettingsOnFile;
        private System.Windows.Forms.Button btnRestoreDefaultOpts;
        private System.Windows.Forms.ErrorProvider errProviderMlcFolder;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage foldersTab;
        private System.Windows.Forms.TextBox txtBoxCustomMlc01Path;
        private System.Windows.Forms.CheckBox chkBoxCustomMlc01Path;
        private System.Windows.Forms.CheckBox chkBoxCemuSettings;
        private System.Windows.Forms.CheckBox chkBoxShaderCaches;
        private System.Windows.Forms.CheckBox chkBoxDLCUpds;
        private System.Windows.Forms.CheckBox chkBoxSavegames;
        private System.Windows.Forms.CheckBox chkBoxGfxPacks;
        private System.Windows.Forms.CheckBox chkBoxGameProfiles;
        private System.Windows.Forms.CheckBox chkBoxControllerProfiles;
        private System.Windows.Forms.TabPage featuresTab;
        private System.Windows.Forms.TabPage downloadTab;
        private System.Windows.Forms.CheckBox chkBoxDesktopShortcut;
        private System.Windows.Forms.CheckBox chkBoxDeletePrevContent;
        private System.Windows.Forms.GroupBox grpBoxMigration;
        private System.Windows.Forms.CheckBox chkBoxNoFullscreenOptimiz;
        private System.Windows.Forms.CheckBox chkBoxOverrideHiDPIBehaviour;
        private System.Windows.Forms.CheckBox chkBoxRunAsAdmin;
        private System.Windows.Forms.CheckBox chkBoxCompatOpts;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.Label lblSampleVersion;
        private System.Windows.Forms.TextBox txtBoxUrlSuffix;
        private System.Windows.Forms.TextBox txtBoxBaseUrl;
        private System.Windows.Forms.Label lblUrl;
        private System.Windows.Forms.Button btnManageFiles;
        private System.Windows.Forms.Button btnManageFolders;
        private System.Windows.Forms.Label lblTotalCustomFolders;
        private System.Windows.Forms.Label lblCustomFoldersCnt;
        private System.Windows.Forms.GroupBox grpBoxCustomEntries;
        private System.Windows.Forms.Label lblHttp;
        private System.Windows.Forms.Label lblUriError;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Label lblTotalCustomFiles;
        private System.Windows.Forms.Label lblCustomFilesCnt;
    }
}