namespace Reginald.Data
{
    using System;
    using System.Windows.Media;

    public interface IItem
    {
        public Guid Guid { get; set; }

        public string Name { get; set; }

        public ImageSource Icon { get; set; }

        public string Caption { get; set; }

        public string Description { get; set; }
    }
}
