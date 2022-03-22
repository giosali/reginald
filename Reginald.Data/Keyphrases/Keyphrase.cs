namespace Reginald.Data.Keyphrases
{
    using System;
    using System.Text.RegularExpressions;

    public abstract partial class Keyphrase : Item
    {
        public const string KeyphraseRegexFormat = @"\b(?<!\S){0}";

        private bool _isEnabled;

        public override bool IsAltKeyDown { get; set; }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        public string Phrase { get; set; }

        public virtual bool Predicate(Keyphrase keyphrase, Regex rx, string input)
        {
            throw new NotImplementedException();
        }
    }
}
