using Microsoft.WindowsAPICodePack.Shell;
using Reginald.Core.AbstractFactories;
using Reginald.Core.AbstractProducts;
using System.Collections.Generic;

namespace Reginald.Core.Clients
{
    public class ShellItemClient
    {
        public ShellItem ShellItem { get; set; }

        public IEnumerable<ShellItem> ShellItems { get; set; }

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
    }
}
