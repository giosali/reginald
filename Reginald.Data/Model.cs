namespace Reginald.Data
{
    using System;
    using System.Windows.Media;
    using Caliburn.Micro;
    using Newtonsoft.Json;

    public abstract class Model : PropertyChangedBase
    {
        private string _caption;

        private string _description;

        private ImageSource _icon;

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
        public ImageSource Icon
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