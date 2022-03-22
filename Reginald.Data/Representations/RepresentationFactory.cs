namespace Reginald.Data.Representations
{
    using System;

    public static class RepresentationFactory
    {
        public static Representation CreateRepresentation(IRepresentationDataModel model)
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
