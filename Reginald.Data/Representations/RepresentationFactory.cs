namespace Reginald.Data.Representations
{
    using System;

    public class RepresentationFactory : ProcessedInputFactory
    {
        public override Representation CreateRepresentation(RepresentationDataModelBase model)
        {
            Type type = model.GetType();
            return type switch
            {
                Type when type == typeof(CalculatorDataModel) => new Calculator(model),
                Type when type == typeof(LinkDataModel) => new Link(model),
                _ => null,
            };
        }
    }
}
