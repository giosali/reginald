namespace Reginald.Core.Products
{
    using System;
    using System.Threading.Tasks;
    using Reginald.Core.AbstractProducts;
    using Reginald.Core.DataModels;
    using Reginald.Core.Helpers;
    using Reginald.Core.Utilities;
    using Reginald.Extensions;

    public class Link : Representation
    {
        public Link(InputDataModelBase model)
        {
            if (Guid.TryParse(model.Guid, out Guid guid))
            {
                Guid = guid;
            }

            Name = model.Name;
            Icon = BitmapImageHelper.FromUri(model.Icon);
            Caption = model.Caption;
            AltCaption = model.AltCaption;
            IsEnabled = model.IsEnabled;
        }

        public override void EnterDown(bool isAltDown, Action action)
        {
            ProcessUtility.GoTo(Uri.IsWellFormedUriString(Description, UriKind.Absolute) ? Description : Description.PrependScheme());
        }

        public override Task<bool> EnterDownAsync(bool isAltDown, Action action, object o)
        {
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

        public bool IsLink(string input)
        {
            Description = input;
            return input.StartsWithScheme() || input.ContainsTopLevelDomain();
        }
    }
}
