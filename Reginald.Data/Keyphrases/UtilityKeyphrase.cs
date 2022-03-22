namespace Reginald.Data.Keyphrases
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Reginald.Core.Helpers;
    using Reginald.Services.Utilities;

    /// <summary>
    /// Specifies the type of utility.
    /// </summary>
    public enum Utility
    {
        /// <summary>
        /// Specifies utility related to the Recycle Bin.
        /// </summary>
        Recycle,
    }

    public class UtilityKeyphrase : Keyphrase
    {
        public const string Filename = "Utilities.json";

        public UtilityKeyphrase(UtilityKeyphraseDataModel model)
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
            AltDescription = model.AltDescription;
            RequiresPrompt = model.RequiresPrompt;
            if (Enum.TryParse(model.Utility, true, out Utility utility))
            {
                Utility = utility;
            }
        }

        public string AltDescription { get; set; }

        public Utility Utility { get; set; }

        public override async void EnterKeyDown()
        {
            switch (Utility)
            {
                case Utility.Recycle:
                    await Task.Run(() => RecycleBin.Empty());
                    break;
            }
        }

        public override void AltKeyDown()
        {
            IsAltKeyDown = true;
            TempCaption = Caption;
            TempDescription = AltDescription;
        }

        public override void AltKeyUp()
        {
            IsAltKeyDown = false;
            TempCaption = Caption;
            TempDescription = Description;
        }

        public override bool Predicate(Keyphrase keyphrase, Regex rx, string input)
        {
            return rx.IsMatch(keyphrase.Phrase);
        }
    }
}
