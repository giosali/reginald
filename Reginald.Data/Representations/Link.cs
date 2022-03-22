namespace Reginald.Data.Representations
{
    using System;
    using System.Threading.Tasks;
    using Reginald.Core.Extensions;
    using Reginald.Core.Helpers;
    using Reginald.Services.Utilities;

    public class Link : Representation, IKeyboardInputProperty
    {
        public const string Filename = "Link.json";

        public Link()
        {
        }

        public Link(IRepresentationDataModel model)
        {
            Guid = Guid.Parse(model.Guid);
            Name = model.Name;
            Icon = BitmapImageHelper.FromUri(model.Icon);
            Caption = model.Caption;
            AltCaption = model.AltCaption;
            IsEnabled = model.IsEnabled;
            LosesFocus = true;
        }

        public override void EnterKeyDown()
        {
            ProcessUtility.GoTo(Uri.IsWellFormedUriString(Description, UriKind.Absolute) ? Description : Description.PrependScheme());
        }

        public override void AltKeyDown()
        {
            IsAltKeyDown = true;
        }

        public override void AltKeyUp()
        {
            IsAltKeyDown = false;
        }

        public Task<bool> IsLink(string input)
        {
            Description = input;
            string tempInput = input.Replace(" ", "%20");
            return Task.FromResult(input.ContainsTopLevelDomain()
                 ? Uri.IsWellFormedUriString(tempInput, UriKind.RelativeOrAbsolute)
                 : input.StartsWithScheme() && Uri.IsWellFormedUriString(tempInput, UriKind.Absolute));
        }
    }
}
