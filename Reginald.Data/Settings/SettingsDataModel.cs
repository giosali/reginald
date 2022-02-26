﻿namespace Reginald.Data.Settings
{
    using System.Windows.Input;
    using Newtonsoft.Json;
    using Reginald.Core.Extensions;
    using Reginald.Core.Helpers;
    using Reginald.Core.IO;

    [JsonObject(MemberSerialization.OptIn)]
    public class SettingsDataModel
    {
        public SettingsDataModel(string filePath)
        {
            SettingsDataModel protoSettings = FileOperations.DeserializeFile<SettingsDataModel>(filePath);

            // Assign properties
            IncludeInstalledApplications = protoSettings?.IncludeInstalledApplications ?? true;
            IncludeDefaultKeywords = protoSettings?.IncludeDefaultKeywords ?? true;
            IncludeHttpKeywords = protoSettings?.IncludeHttpKeywords ?? true;
            IncludeCommands = protoSettings?.IncludeCommands ?? true;
            IncludeUtilities = protoSettings?.IncludeUtilities ?? true;
            IncludeSettingsPages = protoSettings?.IncludeSettingsPages ?? true;
            AreExpansionsEnabled = protoSettings?.AreExpansionsEnabled ?? true;
            LaunchOnStartup = protoSettings?.LaunchOnStartup ?? true;
            SearchBoxKey = protoSettings?.SearchBoxKey ?? "Space";
            SearchBoxModifierOne = protoSettings?.SearchBoxModifierOne ?? "Alt";
            SearchBoxModifierTwo = protoSettings?.SearchBoxModifierTwo ?? "None";
            ThemeIdentifier = protoSettings?.ThemeIdentifier ?? "553a4cdf-11c6-49ce-b634-7ce6945f6958";
        }

        [JsonProperty("includeInstalledApplications")]
        public bool IncludeInstalledApplications { get; set; }

        [JsonProperty("includeDefaultKeywords")]
        public bool IncludeDefaultKeywords { get; set; }

        [JsonProperty("includeHttpKeywords")]
        public bool IncludeHttpKeywords { get; set; }

        [JsonProperty("includeCommands")]
        public bool IncludeCommands { get; set; }

        [JsonProperty("includeUtilities")]
        public bool IncludeUtilities { get; set; }

        [JsonProperty("includeSettingsPages")]
        public bool IncludeSettingsPages { get; set; }

        [JsonProperty("areExpansionsEnabled")]
        public bool AreExpansionsEnabled { get; set; }

        [JsonProperty("launchOnStartup")]
        public bool LaunchOnStartup { get; set; }

        [JsonProperty("searchBoxKey")]
        public string SearchBoxKey { get; set; }

        [JsonProperty("searchBoxModifierOne")]
        public string SearchBoxModifierOne { get; set; }

        [JsonProperty("searchBoxModifierTwo")]
        public string SearchBoxModifierTwo { get; set; }

        [JsonProperty("themeIdentifier")]
        public string ThemeIdentifier { get; set; }

        public void Save()
        {
            FileOperations.WriteFile(ApplicationPaths.SettingsFilename, this.Serialize());
        }
    }
}
