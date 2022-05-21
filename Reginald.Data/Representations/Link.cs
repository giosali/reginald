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
            string temp = input.Trim().Replace(" ", "%20");
            if (!Uri.IsWellFormedUriString(temp, UriKind.Absolute))
            {
                if (!Uri.TryCreate("//" + temp, UriKind.Absolute, out Uri uri))
                {
                    return Task.FromResult(false);
                }

                temp = uri.ToString();
            }

            return Task.FromResult(temp.ContainsTopLevelDomain());
        }
    }
}
