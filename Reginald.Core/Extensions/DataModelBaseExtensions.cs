namespace Reginald.Core.Extensions
{
    using Newtonsoft.Json;
    using Reginald.Core.DataModels;

    public static class DataModelBaseExtensions
    {
        public static string Serialize(this DataModelBase model)
        {
            string json = JsonConvert.SerializeObject(model, Formatting.Indented);
            return json;
        }
    }
}
