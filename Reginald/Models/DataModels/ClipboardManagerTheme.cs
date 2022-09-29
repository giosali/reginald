namespace Reginald.Models.DataModels
{
    using System.Windows;
    using Newtonsoft.Json;

    internal sealed class ClipboardManagerTheme
    {
        [JsonProperty("displayFontSize")]
        public double DisplayFontSize { get; set; }

        [JsonProperty("dockPanelPadding")]
        public Thickness DockPanelPadding { get; set; }

        [JsonProperty("height")]
        public double Height { get; set; }

        [JsonProperty("itemFontSize")]
        public double ItemFontSize { get; set; }

        [JsonProperty("width")]
        public double Width { get; set; }
    }
}
