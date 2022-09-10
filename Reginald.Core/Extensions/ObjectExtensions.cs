namespace Reginald.Core.Extensions
{
    using Newtonsoft.Json;

    public static class ObjectExtensions
    {
        public static string Serialize(this object o)
        {
            return JsonConvert.SerializeObject(o, Formatting.Indented);
        }
    }
}
