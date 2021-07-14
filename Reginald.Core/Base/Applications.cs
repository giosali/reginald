using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;

namespace Reginald.Core.Base
{
    public static class Applications
    {
        public static Dictionary<string, string> MakeDictionary()
        {
            ShellObject applicationsFolder = (ShellObject)KnownFolderHelper.FromKnownFolderId(Constants.ApplicationsGuid);
            Dictionary<string, string> applications = new();

            foreach (ShellObject app in (IKnownFolder)applicationsFolder)
            {
                if (!app.Name.EndsWith(".url") && !app.ParsingName.EndsWith("url"))
                {
                    applications.TryAdd(app.Name, app.ParsingName);
                }
            }
            return applications;
        }
    }
}
