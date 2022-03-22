namespace Reginald.Data
{
    public abstract class DataModelBase
    {
        public string Guid { get; set; }

        public static bool operator ==(DataModelBase a, DataModelBase b)
        {
            return a is not null && b is not null && a.Guid == b.Guid;
        }

        public static bool operator !=(DataModelBase a, DataModelBase b)
        {
            return a is not null && b is not null && a.Guid != b.Guid;
        }

        public override bool Equals(object obj)
        {
            return obj is DataModelBase model && Guid == model.Guid;
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }
    }
}
