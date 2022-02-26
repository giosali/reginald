namespace Reginald.Data.Representations
{
    public class RepresentationClient
    {
        public RepresentationClient(ProcessedInputFactory factory, RepresentationDataModelBase model)
        {
            Representation = factory.CreateRepresentation(model);
        }

        public Representation Representation { get; set; }
    }
}
