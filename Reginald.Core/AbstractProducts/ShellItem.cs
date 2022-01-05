﻿using Caliburn.Micro;
using System;
using System.Windows.Media;

namespace Reginald.Core.AbstractProducts
{
    public abstract class ShellItem : PropertyChangedBase
    {
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

        private string _path;
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                NotifyOfPropertyChange(() => Path);
            }
        }

        public abstract void EnterDown(ShellItem item, bool isAltDown, Action action);

        public abstract (string Description, string Caption) AltDown(ShellItem item);

        public abstract (string Description, string Caption) AltUp(ShellItem item);
    }
}
