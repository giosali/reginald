namespace Reginald.Data.Representations
{
    public class LinkDataModel : DataModelBase, IRepresentationDataModel
    {
        public string Name { get; set; }

        public string Icon { get; set; }

        public string Caption { get; set; }

        public bool IsEnabled { get; set; }
    }
}
