using Reginald.Core.AbstractFactories;
using Reginald.Core.AbstractProducts;
using Reginald.Core.DataModels;

namespace Reginald.Core.Clients
{
    public class RepresentationClient
    {
        public Representation Representation { get; set; }

        public RepresentationClient(ProcessedInputFactory factory, InputDataModelBase model)
        {
            Representation = factory.CreateRepresentation(model);
        }
    }
}
