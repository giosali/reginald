using Microsoft.WindowsAPICodePack.Shell;
using Reginald.Core.AbstractProducts;
using Reginald.Core.Base;
using Reginald.Core.Utilities;
using System;
using System.Windows;

namespace Reginald.Core.Products
{
    public class Application : ShellItem
    {
        public Application(ShellObject shellObject)
        {
            Name = shellObject.Name;
            Icon = shellObject.Thumbnail.MediumBitmapSource;
            Icon.Freeze();
            Caption = Constants.ApplicationCaption;
            Description = Name;
            Path = shellObject.Properties.System.Link.TargetParsingPath.Value is string path
                 ? path
                 : Constants.ShellAppsFolder + shellObject.ParsingName;
        }

        public override void EnterDown(ShellItem item, bool isAltDown, Action action)
        {
            action();
            Application application = item as Application;
            if (isAltDown)
            {
                Clipboard.SetText(application.Path);
            }
            else
            {
                Processes.OpenFromPath(application.Path);
            }
        }

        public override (string Description, string Caption) AltDown(ShellItem item)
        {
            Application application = item as Application;
            string description = null;
            string caption = application.Path;
            return (description, caption);
        }

        public override (string Description, string Caption) AltUp(ShellItem item)
        {
            string description = null;
            string caption = Constants.ApplicationCaption;
            return (description, caption);
        }
    }
}
