using Reginald.Core.AbstractProducts;
using Reginald.Core.DataModels;
using Reginald.Core.Helpers;
using Reginald.Core.Utilities;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Reginald.Core.Products
{
    public class MicrosoftSettingKeyphrase : Keyphrase
    {
        private string _uri;
        public string Uri
        {
            get => _uri;
            set
            {
                _uri = value;
                NotifyOfPropertyChange(() => Uri);
            }
        }

        public MicrosoftSettingKeyphrase(MicrosoftSettingDataModel model)
        {
            if (Guid.TryParse(model.Guid, out Guid guid))
            {
                Guid = guid;
            }
            Name = model.Name;
            Phrase = model.Keyphrase;
            Icon = BitmapImageHelper.FromUri(model.Icon);
            Caption = model.Caption;
            IsEnabled = model.IsEnabled;
            Description = model.Description;
            Uri = model.Uri;
        }

        public override void EnterDown(bool isAltDown, Action action)
        {

        }

        public override Task<bool> EnterDownAsync(bool isAltDown, Action action, object o)
        {
            Processes.OpenFromPath(Uri);
            return Task.FromResult(true);
        }

        public override (string, string) AltDown()
        {
            return (null, null);
        }

        public override (string, string) AltUp()
        {
            return (null, null);
        }

        public override bool Predicate(Keyphrase keyphrase, Regex rx, string input)
        {
            return input.Length > 2 && rx.IsMatch(keyphrase.Phrase);
        }
    }
}
