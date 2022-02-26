namespace Reginald.Data.Keywords
{
    using System.Collections.Generic;
    using System.Linq;
    using Reginald.Core.IO;

    public static class KeywordUtilities
    {
        public static IEnumerable<KeywordDataModelBase> GetKeywordData<T>(string filename, bool isResource)
        {
            string resourceFilePath = FileOperations.GetFilePath(filename, isResource);
            IEnumerable<KeywordDataModelBase> models = FileOperations.DeserializeFile<IEnumerable<T>>(resourceFilePath) as IEnumerable<KeywordDataModelBase> ?? Enumerable.Empty<KeywordDataModelBase>();

            // Disable models that aren't supposed to be enabled
            if (isResource)
            {
                string localFilePath = FileOperations.GetFilePath(filename, false);
                IEnumerable<KeywordDataModelBase> disabledModels = FileOperations.DeserializeFile<IEnumerable<T>>(localFilePath) as IEnumerable<KeywordDataModelBase> ?? Enumerable.Empty<KeywordDataModelBase>();

                foreach (KeywordDataModelBase disabledModel in disabledModels)
                {
                    foreach (KeywordDataModelBase model in models)
                    {
                        if (model == disabledModel)
                        {
                            model.IsEnabled = false;
                            break;
                        }
                    }
                }
            }

            return models;
        }
    }
}
