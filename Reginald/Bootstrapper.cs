namespace Reginald
{
    using System.Windows;
    using Caliburn.Micro;
    using Reginald.ViewModels;

    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}
