namespace Reginald.Data.ShellItems
{
    using System.Windows;
    using Microsoft.WindowsAPICodePack.Shell;
    using Reginald.Services.Utilities;

    public partial class Application : ShellItem
    {
        private const string ShellAppsFolder = @"shell:AppsFolder\";

        private const string DefaultCaption = "Application";

        public Application(ShellObject shellObject)
        {
            Name = shellObject.Name;
            Icon = shellObject.Thumbnail.MediumBitmapSource;
            Icon.Freeze();
            Caption = DefaultCaption;
            Description = Name;
            Id = shellObject.Properties.System.AppUserModel.GetHashCode();
            Path = shellObject.Properties.System.Link.TargetParsingPath.Value is string path
                 ? path
                 : ShellAppsFolder + shellObject.ParsingName;
            LosesFocus = true;
        }

        public override void EnterKeyDown()
        {
            if (IsAltKeyDown)
            {
                Clipboard.SetText(Path);
            }
            else
            {
                ProcessUtility.OpenFromPath(Path);
            }
        }

        public override void AltKeyDown()
        {
            IsAltKeyDown = true;
            TempCaption = Path;
            TempDescription = Description;
        }

        public override void AltKeyUp()
        {
            IsAltKeyDown = false;
            TempCaption = DefaultCaption;
            TempDescription = Description;
        }
    }
}
