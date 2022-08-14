namespace Reginald.Core.Extensions
{
    using Caliburn.Micro;

    public static class BindableCollectionExtensions
    {
        public static void PrependFrom<T>(this BindableCollection<T> collection, int index)
        {
            T item = collection[index];
            for (int i = index; i > 0; i--)
            {
                collection[i] = collection[i - 1];
            }

            collection[0] = item;
        }
    }
}
