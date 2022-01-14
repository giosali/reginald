namespace Reginald.Core.DataModels
{
    using System;
    using Newtonsoft.Json;

    [Serializable]
    public class SettingsDataModel : DataModelBase
    {
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
    }
}
