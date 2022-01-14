namespace Reginald.Core.Clients
{
    using System.Collections.Generic;
    using System.Linq;
    using Reginald.Core.AbstractFactories;
    using Reginald.Core.AbstractProducts;
    using Reginald.Core.DataModels;

    public class KeyphraseClient
    {
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

        public Keyphrase Keyphrase { get; set; }

        public IEnumerable<Keyphrase> Keyphrases { get; set; }
    }
}
