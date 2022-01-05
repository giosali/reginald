using Caliburn.Micro;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Reginald.Core.AbstractProducts
{
    public abstract class Keyphrase : PropertyChangedBase
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

        private string _phrase;
        public string Phrase
        {
            get => _phrase;
            set
            {
                _phrase = value;
                NotifyOfPropertyChange(() => Phrase);
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

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
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

        public abstract bool Predicate(Keyphrase keyphrase, Regex rx, string input);

        public abstract Task<bool> EnterDown(Keyphrase keyphrase, bool isAltDown, bool isPrompted, Action action);

        public abstract (string Description, string Caption) AltDown(Keyphrase keyphrase);

        public abstract (string Description, string Caption) AltUp(Keyphrase keyphrase);
    }
}
