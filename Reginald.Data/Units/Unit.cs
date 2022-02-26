namespace Reginald.Data.Units
{
    using System;
    using Caliburn.Micro;

    public abstract class Unit : PropertyChangedBase
    {
        private Guid _guid;

        private string _name;

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
    }
}
