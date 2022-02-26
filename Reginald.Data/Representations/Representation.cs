namespace Reginald.Data.Representations
{
    using System;
    using System.Windows.Media;
    using Reginald.Data.Base;

    public abstract class Representation : InteractiveObjectBase
    {
        private Guid _guid;

        private string _name;

        private ImageSource _icon;

        private string _caption;

        private string _altCaption;

        private bool _isEnabled;

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

        public string AltCaption
        {
            get => _altCaption;
            set
            {
                _altCaption = value;
                NotifyOfPropertyChange(() => AltCaption);
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
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
    }
}
