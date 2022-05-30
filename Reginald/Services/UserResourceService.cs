namespace Reginald.Services
{
    using Reginald.Data.ShellItems;

    public class UserResourceService
    {
        public UserResourceService()
        {
            UpdateApplications();
        }

        public ShellItem[] Applications { get; set; }

        public void UpdateApplications()
        {
            Applications = ShellItemFactory.CreateShellItems(Shell.GetApplications());
        }
    }
}
