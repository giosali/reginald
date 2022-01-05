using Reginald.Core.AbstractFactories;
using Reginald.Core.AbstractProducts;
using Reginald.Core.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace Reginald.Core.Clients
{
    public class KeyphraseClient
    {
        public Keyphrase Keyphrase { get; set; }

        public IEnumerable<Keyphrase> Keyphrases { get; set; }

        public KeyphraseClient(KeyFactory factory, KeyphraseDataModelBase model)
        {
            Keyphrase = factory.CreateKeyphrase(model);
        }

        public KeyphraseClient(KeyFactory factory, IEnumerable<KeyphraseDataModelBase> models)
        {
            List<Keyphrase> keywords = new(models.Count());
            foreach (KeyphraseDataModelBase model in models)
            {
                keywords.Add(factory.CreateKeyphrase(model));
            }
            Keyphrases = keywords;
        }
    }
}
