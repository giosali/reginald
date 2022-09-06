namespace Reginald.Models.DataModels
{
    using System;
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

        [JsonProperty("defaultWebQueries")]
        public Guid[] DefaultWebQueries { get; set; } = new[] { new Guid("2088a843-cf3f-4bc3-8995-6762d46e4462"), new Guid("125a4696-b458-400c-80db-b167f8ec5632"), new Guid("35af0718-e3c2-4a40-bf32-a96ebc780171") };

        [JsonProperty("isCalculatorEnabled")]
        public bool IsCalculatorEnabled { get; set; } = true;

        [JsonProperty("disabledWebQueries")]
        public List<Guid> DisabledWebQueries { get; set; } = new();

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

        [JsonProperty("themeIdentifier")]
        public Guid ThemeIdentifier { get; set; } = new("fea6b940-29e7-4e02-9f85-154e691789ca");

        public void Save()
        {
            FileOperations.WriteFile(FileName, this.Serialize());
        }
    }
}
