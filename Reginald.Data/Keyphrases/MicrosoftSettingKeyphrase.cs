namespace Reginald.Data.Keyphrases
{
    using System;
    using System.Text.RegularExpressions;
    using Reginald.Core.Helpers;
    using Reginald.Services.Utilities;

    public class MicrosoftSettingKeyphrase : Keyphrase
    {
        public const string Filename = "MicrosoftSettings.json";

        public MicrosoftSettingKeyphrase(MicrosoftSettingKeyphraseDataModel model)
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
            LosesFocus = true;
        }

        public string Uri { get; set; }

        public override void EnterKeyDown()
        {
            ProcessUtility.OpenFromPath(Uri);
        }

        public override void AltKeyDown()
        {
            IsAltKeyDown = true;
            TempCaption = Caption;
            TempDescription = Description;
        }

        public override void AltKeyUp()
        {
            IsAltKeyDown = false;
            TempCaption = Caption;
            TempDescription = Description;
        }

        public override bool Predicate(Keyphrase keyphrase, Regex rx, string input)
        {
            return input.Length > 2 && rx.IsMatch(keyphrase.Phrase);
        }
    }
}
