using Reginald.Core.AbstractProducts;
using Reginald.Core.DataModels;
using Reginald.Core.Helpers;
using Reginald.Core.Utilities;
using Reginald.Extensions;
using System;

namespace Reginald.Core.Products
{
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

        public override void EnterDown(Representation representation, bool isAltDown, Action action, object sender)
        {
            Link link = representation as Link;
            string uri = Uri.IsWellFormedUriString(link.Description, UriKind.Absolute)
                       ? link.Description
                       : link.Description.PrependScheme();
            Processes.GoTo(uri);
        }

        public override (string Description, string Caption) AltDown(Representation representation)
        {
            return (null, null);
        }

        public bool IsLink(string input)
        {
            Description = input;
            return input.StartsWithScheme() || input.ContainsTopLevelDomain();
        }

        public override (string Description, string Caption) AltUp(Representation representation)
        {
            return (null, null);
        }
    }
}
