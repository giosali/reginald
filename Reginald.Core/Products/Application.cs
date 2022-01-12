using Microsoft.WindowsAPICodePack.Shell;
using Reginald.Core.AbstractProducts;
using Reginald.Core.Base;
using Reginald.Core.Utilities;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Reginald.Core.Products
{
    public class Application : ShellItem
    {
        private const string DefaultCaption = "Application";

        public Application(ShellObject shellObject)
        {
            Name = shellObject.Name;
            Icon = shellObject.Thumbnail.MediumBitmapSource;
            Icon.Freeze();
            Caption = DefaultCaption;
            Description = Name;
            Path = shellObject.Properties.System.Link.TargetParsingPath.Value is string path
                 ? path
                 : Constants.ShellAppsFolder + shellObject.ParsingName;
        }

        public override void EnterDown(bool isAltDown, Action action)
        {
            action();
            if (isAltDown)
            {
                Clipboard.SetText(Path);
            }
            else
            {
                Processes.OpenFromPath(Path);
            }
        }

        public override Task<bool> EnterDownAsync(bool isAltDown, Action action, object o)
        {
            return Task.FromResult(true);
        }

        public override (string, string) AltDown()
        {
            return (null, Path);
        }

        public override (string, string) AltUp()
        {
            return (null, DefaultCaption);
        }
    }
}
