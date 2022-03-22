namespace Reginald.Data.Units
{
    using System;
    using Caliburn.Micro;

    public abstract class Unit : PropertyChangedBase
    {
        public Guid Guid { get; set; }

        public string Name { get; set; }
    }
}
