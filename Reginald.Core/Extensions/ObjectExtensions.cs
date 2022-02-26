namespace Reginald.Core.Extensions
{
    using Newtonsoft.Json;

    public static class ObjectExtensions
    {
        public static string Serialize(this object o)
        {
            string json = JsonConvert.SerializeObject(o, Formatting.Indented);
            return json;
        }
    }
}
