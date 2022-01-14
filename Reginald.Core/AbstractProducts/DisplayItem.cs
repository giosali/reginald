namespace Reginald.Core.AbstractProducts
{
    using System;
    using System.Windows.Media;

    public abstract class DisplayItem : InteractiveAbstractProductBase
    {
        private Guid _guid;

        private string _name;

        private ImageSource _icon;

        private string _caption;

        private string _description;

        public Guid Guid
        {
            get => _guid;
            set
            {
                _guid = value;
                NotifyOfPropertyChange(() => Guid);
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public ImageSource Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                NotifyOfPropertyChange(() => Icon);
            }
        }

        public string Caption
        {
            get => _caption;
            set
            {
                _caption = value;
                NotifyOfPropertyChange(() => Caption);
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                NotifyOfPropertyChange(() => Description);
            }
        }

        public static bool operator ==(DisplayItem a, DisplayItem b)
        {
            return a is not null && b is not null && a.Guid == b.Guid;
        }

        public static bool operator !=(DisplayItem a, DisplayItem b)
        {
            return a is not null && b is not null && a.Guid != b.Guid;
        }

        public override bool Equals(object obj)
        {
            return obj is not null && obj is DisplayItem item && Guid == item.Guid;
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        public abstract bool Predicate();
    }
}
