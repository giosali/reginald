namespace Reginald.Core.AbstractFactories
{
    using Reginald.Core.AbstractProducts;
    using Reginald.Core.DataModels;

    public abstract class ProcessedInputFactory
    {
        public abstract Representation CreateRepresentation(InputDataModelBase model);
    }
}
