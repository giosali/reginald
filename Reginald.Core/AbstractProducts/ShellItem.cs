﻿namespace Reginald.Core.AbstractProducts
{
    using System.Windows.Media;

    public abstract class ShellItem : InteractiveAbstractProductBase
    {
        private string _name;

        private ImageSource _icon;

        private string _caption;

        private string _description;

        private string _path;

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

        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                NotifyOfPropertyChange(() => Path);
            }
        }
    }
}
