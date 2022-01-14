namespace Reginald.Core.Products
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Reginald.Core.AbstractProducts;
    using Reginald.Core.DataModels;
    using Reginald.Core.Helpers;
    using Reginald.Core.Utilities;

    public class MicrosoftSettingKeyphrase : Keyphrase
    {
        private string _uri;

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

        public string Uri
        {
            get => _uri;
            set
            {
                _uri = value;
                NotifyOfPropertyChange(() => Uri);
            }
        }

        public override void EnterDown(bool isAltDown, Action action)
        {
        }

        public override Task<bool> EnterDownAsync(bool isAltDown, Action action, object o)
        {
            ProcessUtility.OpenFromPath(Uri);
            return Task.FromResult(true);
        }

        public override (string Description, string Caption) AltDown()
        {
            return (null, null);
        }

        public override (string Description, string Caption) AltUp()
        {
            return (null, null);
        }

        public override bool Predicate(Keyphrase keyphrase, Regex rx, string input)
        {
            return input.Length > 2 && rx.IsMatch(keyphrase.Phrase);
        }
    }
}
