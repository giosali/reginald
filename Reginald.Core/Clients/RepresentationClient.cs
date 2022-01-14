namespace Reginald.Core.Clients
{
    using Reginald.Core.AbstractFactories;
    using Reginald.Core.AbstractProducts;
    using Reginald.Core.DataModels;

    public class RepresentationClient
    {
        public RepresentationClient(ProcessedInputFactory factory, InputDataModelBase model)
        {
            Representation = factory.CreateRepresentation(model);
        }

        public Representation Representation { get; set; }
    }
}
