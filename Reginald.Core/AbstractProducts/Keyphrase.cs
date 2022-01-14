namespace Reginald.Core.AbstractProducts
{
    using System;
    using System.Text.RegularExpressions;
    using System.Windows.Media;

    public abstract class Keyphrase : InteractiveAbstractProductBase
    {
        private Guid _guid;

        private string _name;

        private string _phrase;

        private ImageSource _icon;

        private string _caption;

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

        public string Phrase
        {
            get => _phrase;
            set
            {
                _phrase = value;
                NotifyOfPropertyChange(() => Phrase);
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

        public abstract bool Predicate(Keyphrase keyphrase, Regex rx, string input);
    }
}
