using Reginald.Core.AbstractProducts;
using Reginald.Core.DataModels;
using Reginald.Core.Helpers;
using Reginald.Core.Utilities;
using Reginald.Extensions;
using System;
using System.Threading.Tasks;

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

        public override void EnterDown(bool isAltDown, Action action)
        {
            Processes.GoTo(Uri.IsWellFormedUriString(Description, UriKind.Absolute) ? Description : Description.PrependScheme());
        }

        public override Task<bool> EnterDownAsync(bool isAltDown, Action action, object o)
        {
            return Task.FromResult(true);
        }

        public override (string, string) AltDown()
        {
            return (null, null);
        }

        public bool IsLink(string input)
        {
            Description = input;
            return input.StartsWithScheme() || input.ContainsTopLevelDomain();
        }

        public override (string, string) AltUp()
        {
            return (null, null);
        }
    }
}
