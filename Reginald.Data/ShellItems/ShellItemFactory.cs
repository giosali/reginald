namespace Reginald.Data.ShellItems
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.WindowsAPICodePack.Shell;

    public static class ShellItemFactory
    {
        public static ShellItem[] CreateShellItems(IEnumerable<ShellObject> shellObjects)
        {
            return shellObjects.Select(shellObject => new Application(shellObject))
                               .ToArray();
        }
    }
}
