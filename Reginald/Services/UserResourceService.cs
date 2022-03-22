namespace Reginald.Services
{
    using Reginald.Data.ShellItems;

    public class UserResourceService
    {
        public UserResourceService()
        {
            Applications = ShellItemFactory.CreateShellItems(Shell.GetApplications());
        }

        public ShellItem[] Applications { get; set; }
    }
}
