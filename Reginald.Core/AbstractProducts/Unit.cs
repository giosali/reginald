using Caliburn.Micro;
using System;

namespace Reginald.Core.AbstractProducts
{
    public abstract class Unit : PropertyChangedBase
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
    }
}
