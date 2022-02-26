namespace Reginald.Data.ShellItems
{
    using System.Collections.Generic;
    using Microsoft.WindowsAPICodePack.Shell;
    using Reginald.Data.Base;

    public class ShellItemClient
    {
        public ShellItemClient(KeyFactory factory, ShellObject shellObject)
        {
            ShellItem = factory.CreateShellItem(shellObject);
        }

        public ShellItemClient(KeyFactory factory, IEnumerable<ShellObject> shellObjects)
        {
            List<ShellItem> applications = new();
            foreach (ShellObject shellObject in shellObjects)
            {
                applications.Add(factory.CreateShellItem(shellObject));
            }

            ShellItems = applications;
        }

        public ShellItem ShellItem { get; set; }

        public IEnumerable<ShellItem> ShellItems { get; set; }
    }
}
