namespace Reginald.Data.Representations
{
    public abstract class ProcessedInputFactory
    {
        public abstract Representation CreateRepresentation(RepresentationDataModelBase model);
    }
}
