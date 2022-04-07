namespace Reginald.Data.Settings
{
    using Newtonsoft.Json;
    using Reginald.Core.Extensions;
    using Reginald.Core.IO;

    [JsonObject(MemberSerialization.OptIn)]
    public class SettingsDataModel
    {
        public const string Filename = "Settings.json";

        public SettingsDataModel(string filePath)
        {
            SettingsDataModel protoSettings = FileOperations.DeserializeFile<SettingsDataModel>(filePath);

            IncludeInstalledApplications = protoSettings?.IncludeInstalledApplications ?? true;
            IncludeDefaultKeywords = protoSettings?.IncludeDefaultKeywords ?? true;
            IncludeHttpKeywords = protoSettings?.IncludeHttpKeywords ?? true;
            IncludeCommands = protoSettings?.IncludeCommands ?? true;
            IncludeUtilities = protoSettings?.IncludeUtilities ?? true;
            IncludeSettingsPages = protoSettings?.IncludeSettingsPages ?? true;
            AreExpansionsEnabled = protoSettings?.AreExpansionsEnabled ?? true;
            LaunchOnStartup = protoSettings?.LaunchOnStartup ?? true;
            ReginaldKey = protoSettings?.ReginaldKey ?? "Space";
            ReginaldModifiers = protoSettings?.ReginaldModifiers ?? "Alt";
            IsClipboardManagerEnabled = protoSettings?.IsClipboardManagerEnabled ?? true;
            ClipboardManagerKey = protoSettings?.ClipboardManagerKey ?? "V";
            ClipboardManagerModifiers = protoSettings?.ClipboardManagerModifiers ?? "Alt, Shift";
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

        [JsonProperty("reginaldKey")]
        public string ReginaldKey { get; set; }

        [JsonProperty("reginaldModifiers")]
        public string ReginaldModifiers { get; set; }

        [JsonProperty("isClipboardManagerEnabled")]
        public bool IsClipboardManagerEnabled { get; set; }

        [JsonProperty("clipboardManagerKey")]
        public string ClipboardManagerKey { get; set; }

        [JsonProperty("clipboardManagerModifiers")]
        public string ClipboardManagerModifiers { get; set; }

        [JsonProperty("themeIdentifier")]
        public string ThemeIdentifier { get; set; }

        public void Save()
        {
            FileOperations.WriteFile(Filename, this.Serialize());
        }
    }
}
