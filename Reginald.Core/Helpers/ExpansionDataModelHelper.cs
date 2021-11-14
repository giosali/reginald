using Caliburn.Micro;
using Newtonsoft.Json;
using Reginald.Core.DataModels;
using Reginald.Core.IO;
using System.Collections.Generic;
using System.Linq;

namespace Reginald.Core.Helpers
{
    public static class ExpansionDataModelHelper
    {
        public static void Save(BindableCollection<ExpansionDataModel> models)
        {
            string json = JsonConvert.SerializeObject(models.ToList(), Formatting.Indented);
            FileOperations.WriteFile(ApplicationPaths.ExpansionsJsonFilename, json);
        }

        public static void Save(List<ExpansionDataModel> models)
        {
            string json = JsonConvert.SerializeObject(models, Formatting.Indented);
            FileOperations.WriteFile(ApplicationPaths.ExpansionsJsonFilename, json);
        }

        public static int GetLongestTriggerLength(List<ExpansionDataModel> models)
        {
            int maxLength = 0;
            for (int i = 0; i < models.Count; i++)
            {
                ExpansionDataModel model = models[i];
                int triggerLength = model.Trigger.Length;
                if (triggerLength > maxLength)
                {
                    maxLength = triggerLength;
                }
            }
            return maxLength;
        }
    }
}
