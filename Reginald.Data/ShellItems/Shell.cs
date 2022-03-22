namespace Reginald.Data.ShellItems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.WindowsAPICodePack.Shell;

    public static class Shell
    {
        public static readonly Guid ApplicationsFolderGuid = new("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");

        /// <summary>
        /// Returns a known folder from the specified globally unique identifier.
        /// </summary>
        /// <param name="guid">A globally unique identifier.</param>
        /// <returns>An <see cref="IKnownFolder"/> related to the specified globally unique indentifier.</returns>
        public static IKnownFolder GetKnownFolder(Guid guid)
        {
            IKnownFolder folder = KnownFolderHelper.FromKnownFolderId(guid);
            return folder;
        }

        /// <summary>
        /// Returns a sequence of applications represented by shell objects.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{ShellObject}"/> containing applications whose names and parsing names don't contain "url".</returns>
        public static IEnumerable<ShellObject> GetApplications()
        {
            IKnownFolder applicationsFolder = KnownFolderHelper.FromKnownFolderId(ApplicationsFolderGuid);
            return applicationsFolder.Where(application => !application.Name.EndsWith(".url", StringComparison.InvariantCulture) && !application.ParsingName.EndsWith("url", StringComparison.InvariantCulture));
        }
    }
}
