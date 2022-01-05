using Caliburn.Micro;
using System;
using System.Windows.Media;

namespace Reginald.Core.AbstractProducts
{
    public abstract class DisplayItem : PropertyChangedBase
    {
        private Guid _guid;
        public Guid Guid
        {
            get => _guid;
            set
            {
                _guid = value;
                NotifyOfPropertyChange(() => Guid);
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        private ImageSource _icon;
        public ImageSource Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                NotifyOfPropertyChange(() => Icon);
            }
        }

        private string _caption;
        public string Caption
        {
            get => _caption;
            set
            {
                _caption = value;
                NotifyOfPropertyChange(() => Caption);
            }
        }

        private string _description;
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
    }
}
