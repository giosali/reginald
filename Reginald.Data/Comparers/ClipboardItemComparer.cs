namespace Reginald.Data.Comparers
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Reginald.Data.DisplayItems;

    public class ClipboardItemComparer : IEqualityComparer<ClipboardItem>
    {
        public bool Equals(ClipboardItem x, ClipboardItem y)
        {
            // Checks whether the items are both of type ClipboardItemType.Text
            // and whether the products' Description properties are equal
            return x.ClipboardItemType == ClipboardItemType.Text && y.ClipboardItemType == ClipboardItemType.Text && x.Description == y.Description;
        }

        public int GetHashCode([DisallowNull] ClipboardItem obj)
        {
            // Checks whether the object or the Name property are null
            // If either is null, returns 0; otherwise, returns the hash code for the Name property
            return obj.Description == null || ReferenceEquals(obj, null) ? 0 : obj.Description.GetHashCode();
        }
    }
}
