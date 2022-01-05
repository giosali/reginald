using Reginald.Core.AbstractProducts;
using Reginald.Core.DataModels;

namespace Reginald.Core.AbstractFactories
{
    public abstract class ProcessedInputFactory
    {
        public abstract Representation CreateRepresentation(InputDataModelBase model);
    }
}
