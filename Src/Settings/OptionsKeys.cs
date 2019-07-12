namespace CemuUpdateTool.Settings
{
    static class OptionsKeys
    {
        // Migration options
        public const string DeleteDestinationFolderContents = "deleteDestFolderContents";
        public const string UseCustomMlcFolderIfSupported = "dontCopyMlcFolderFor1.10+";
        public const string AskForDesktopShortcut = "askForDesktopShortcut";
        public const string SetCompatibilityOptions = "setCompatibilityOptions";
        public const string CompatibilityRunAsAdmin = "compat_runAsAdmin";
        public const string CompatibilityNoFullscreenOptimizations = "compat_noFullscreenOptimizations";
        public const string CompatibilityOverrideHiDPIBehaviour = "compat_overrideHiDPIBehaviour";

        // Download options
        public const string CemuBaseUrl = "cemuBaseUrl";
        public const string CemuUrlSuffix = "cemuUrlSuffix";
        public const string LastKnownCemuVersion = "lastKnownCemuVersion";
    }

    static class OptionsFolder
    {
        public const string ControllerProfiles = "controllerProfiles";
        public const string GameProfiles = "gameProfiles";
        public const string GraphicPacks = "graphicPacks";
        public const string OldSavegames = @"mlc01\emulatorSave";       // savegame directory before 1.11
        public const string Savegames = @"mlc01\usr\save";              // savegame directory since 1.11
        public const string DLCUpdates = @"mlc01\usr\title";
        public const string TransferableCaches = @"shaderCache\transferable";
    }

    static class OptionsFile
    {
        public const string SettingsBin = "settings.bin";
        public const string SettingsXml = "settings.xml";
    }
}
