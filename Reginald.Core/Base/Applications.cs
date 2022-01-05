using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reginald.Core.Base
{
    public static class Applications
    {
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
            IKnownFolder applicationsFolder = KnownFolderHelper.FromKnownFolderId(Constants.ApplicationsGuid);
            return applicationsFolder.Where(application =>
            {
                return !application.Name.EndsWith(".url", StringComparison.InvariantCulture) &&
                       !application.ParsingName.EndsWith("url", StringComparison.InvariantCulture);
            });
        }
    }
}
