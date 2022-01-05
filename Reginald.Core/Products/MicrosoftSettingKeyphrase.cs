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

        public override bool Predicate(Keyphrase keyphrase, Regex rx, string input)
        {
            return input.Length > 2 && rx.IsMatch(keyphrase.Phrase);
        }

        public override Task<bool> EnterDown(Keyphrase keyphrase, bool isAltDown, bool isPrompted, Action action)
        {
            MicrosoftSettingKeyphrase microsoftSetting = keyphrase as MicrosoftSettingKeyphrase;
            Processes.OpenFromPath(microsoftSetting.Uri);
            return Task.FromResult(true);
        }

        public override (string Description, string Caption) AltDown(Keyphrase keyphrase)
        {
            return (null, null);
        }

        public override (string Description, string Caption) AltUp(Keyphrase keyphrase)
        {
            return (null, null);
        }
    }
}
