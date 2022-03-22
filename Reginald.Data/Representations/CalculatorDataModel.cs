namespace Reginald.Data.Representations
{
    public class CalculatorDataModel : DataModelBase, IRepresentationDataModel
    {
        public string Name { get; set; }

        public string Icon { get; set; }

        public string Caption { get; set; }

        public string AltCaption { get; set; }

        public bool IsEnabled { get; set; }
    }
}
