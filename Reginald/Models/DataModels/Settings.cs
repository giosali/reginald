namespace Reginald.Models.DataModels
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Reginald.Core.Extensions;
    using Reginald.Core.IO;

    public class Settings
    {
        public const string FileName = "Settings.json";

        [JsonProperty("areApplicationsEnabled")]
        public bool AreApplicationsEnabled { get; set; } = true;

        [JsonProperty("areExpansionsEnabled")]
        public bool AreExpansionsEnabled { get; set; } = true;

        [JsonProperty("areMicrosoftSettingsEnabled")]
        public bool AreMicrosoftSettingsEnabled { get; set; } = true;

        [JsonProperty("areTimersEnabled")]
        public bool AreTimersEnabled { get; set; } = true;

        [JsonProperty("areWebQueriesEnabled")]
        public bool AreWebQueriesEnabled { get; set; } = true;

        [JsonProperty("clipboardManagerKey")]
        public string ClipboardManagerKey { get; set; } = "V";

        [JsonProperty("clipboardManagerModifiers")]
        public string ClipboardManagerModifiers { get; set; } = "Alt, Shift";

        [JsonProperty("decimalSeparator")]
        public char DecimalSeparator { get; set; } = '\0';

        [JsonProperty("defaultWebQueries")]
        public int[] DefaultWebQueries { get; set; } = new[] { 0x756442, 0xd6958a, 0x6d81bf };

        [JsonProperty("disabledWebQueries")]
        public List<int> DisabledWebQueries { get; set; } = new();

        [JsonProperty("isCalculatorEnabled")]
        public bool IsCalculatorEnabled { get; set; } = true;

        [JsonProperty("isClearClipboardEnabled")]
        public bool IsClearClipboardEnabled { get; set; } = true;

        [JsonProperty("isClipboardManagerEnabled")]
        public bool IsClipboardManagerEnabled { get; set; } = true;

        [JsonProperty("isQuitEnabled")]
        public bool IsQuitEnabled { get; set; } = true;

        [JsonProperty("isRecycleEnabled")]
        public bool IsRecycleEnabled { get; set; } = true;

        [JsonProperty("isTimerEnabled")]
        public bool IsTimerEnabled { get; set; } = true;

        [JsonProperty("isUrlEnabled")]
        public bool IsUrlEnabled { get; set; } = true;

        [JsonProperty("mainKey")]
        public string MainKey { get; set; } = "Space";

        [JsonProperty("mainModifiers")]
        public string MainModifiers { get; set; } = "Alt";

        [JsonProperty("runAtStartup")]
        public bool RunAtStartup { get; set; } = true;

        [JsonProperty("themeId")]
        public int ThemeId { get; set; } = 0x676dbc;

        public void Save()
        {
            FileOperations.WriteFile(FileName, this.Serialize());
        }
    }
}
