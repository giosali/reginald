namespace Reginald.Data.DataModels
{
    using System;
    using Caliburn.Micro;
    using Newtonsoft.Json;

    public abstract class DataModel : PropertyChangedBase
    {
        private string _iconPath;

        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("guid")]
        public Guid Guid { get; set; }

        [JsonProperty("iconPath")]
        public string IconPath
        {
            get => _iconPath;
            set
            {
                _iconPath = value;
                NotifyOfPropertyChange(() => IconPath);
            }
        }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
