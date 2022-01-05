﻿using Caliburn.Micro;
using System;
using System.Windows.Media;

namespace Reginald.Core.AbstractProducts
{
    public abstract class Representation : PropertyChangedBase
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

        private string _altCaption;
        public string AltCaption
        {
            get => _altCaption;
            set
            {
                _altCaption = value;
                NotifyOfPropertyChange(() => AltCaption);
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

        public abstract void EnterDown(Representation representation, bool isAltDown, Action action, object sender);

        public abstract (string Description, string Caption) AltDown(Representation representation);

        public abstract (string Description, string Caption) AltUp(Representation representation);
    }
}
