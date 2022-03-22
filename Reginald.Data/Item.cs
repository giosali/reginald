namespace Reginald.Data
{
    using System;
    using System.Windows.Media;
    using Caliburn.Micro;
    using Newtonsoft.Json;

    public abstract class Item : PropertyChangedBase, IItem, IKeyboardInputProperty
    {
        private ImageSource _icon;

        private string _caption;

        private string _description;

        [JsonProperty("guid")]
        public Guid Guid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

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

        public int Id { get; set; }

        public string TempCaption { get; set; }

        public string TempDescription { get; set; }

        public bool RequiresPrompt { get; set; }

        public bool CanReceiveKeyboardInput { get; set; } = true;

        public bool LosesFocus { get; set; }

        public abstract bool IsAltKeyDown { get; set; }

        public static bool operator ==(Item a, Item b)
        {
            return a is not null && b is not null && a.Guid == b.Guid && a.Id == b.Id;
        }

        public static bool operator !=(Item a, Item b)
        {
            return a is not null && b is not null && a.Guid != b.Guid && a.Id != b.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is Item item && Guid == item.Guid && Id == item.Id;
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode() ^ Id;
        }

        public abstract void EnterKeyDown();

        public abstract void AltKeyDown();

        public abstract void AltKeyUp();
    }
}
