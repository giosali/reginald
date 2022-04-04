namespace Reginald.Messages
{
    public sealed class UpdateItemsMessage
    {
        public UpdateItemsMessage(string filename, bool isResource)
        {
            Filename = filename;
            IsResource = isResource;
        }

        public string Filename { get; set; }

        public bool IsResource { get; set; }
    }
}
