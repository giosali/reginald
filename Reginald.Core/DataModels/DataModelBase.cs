using Newtonsoft.Json;

namespace Reginald.Core.DataModels
{
    public class DataModelBase
    {
        public string Serialize()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            return json;
        }
    }
}
