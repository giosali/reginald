namespace Reginald.Messages
{
    public sealed class UpdatePageMessage
    {
        public UpdatePageMessage(string title)
        {
            Title = title;
        }

        public string Title { get; private set; }
    }
}
