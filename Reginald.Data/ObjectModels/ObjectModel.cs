namespace Reginald.Data.ObjectModels
{
    using System;
    using Caliburn.Micro;
    using Newtonsoft.Json;

    public abstract class ObjectModel : PropertyChangedBase
    {
        private string _caption;

        private string _description;

        private string _icon;

        [JsonProperty("caption")]
        public string Caption
        {
            get => _caption;
            set
            {
                _caption = value;
                NotifyOfPropertyChange(() => Caption);
            }
        }

        [JsonProperty("description")]
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                NotifyOfPropertyChange(() => Description);
            }
        }

        [JsonProperty("guid")]
        public Guid Guid { get; set; }

        [JsonProperty("icon")]
        public string Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                NotifyOfPropertyChange(() => Icon);
            }
        }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
