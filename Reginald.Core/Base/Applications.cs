using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;

namespace Reginald.Core.Base
{
    public static class Applications
    {
        /// <summary>
        /// Returns a dictionary consisting of the user's installed applications' names and parsing names.
        /// </summary>
        /// <returns>A dictionary.</returns>
        public static Dictionary<string, string> MakeDictionary()
        {
            IKnownFolder applicationsFolder = KnownFolderHelper.FromKnownFolderId(Constants.ApplicationsGuid);
            Dictionary<string, string> applications = new();

            foreach (ShellObject app in applicationsFolder)
            {
                if (app.Name.EndsWith(".url") || app.ParsingName.EndsWith("url"))
                {
                    continue;
                }
                _ = applications.TryAdd(app.Name, app.ParsingName);
            }
            return applications;
        }

        public static IKnownFolder GetKnownFolder(Guid guid)
        {
            IKnownFolder folder = KnownFolderHelper.FromKnownFolderId(guid);
            return folder;
        }
    }
}
