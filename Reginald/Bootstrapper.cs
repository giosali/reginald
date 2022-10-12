namespace Reginald
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Windows;
    using Caliburn.Micro;
    using Reginald.Core.Services;
    using Reginald.Services;
    using Reginald.ViewModels;

    internal sealed class Bootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new();

        private IntPtr _hMutex = IntPtr.Zero;

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void Configure()
        {
            _ = _container.Singleton<IWindowManager, WindowManager>()
                          .Singleton<IEventAggregator, EventAggregator>()
                          .Singleton<DataModelService>()
                          .Singleton<ObjectModelService>()
                          .Singleton<MainViewModel>()
                          .Singleton<ClipboardManagerPopupViewModel>()
                          .Singleton<ShellViewModel>()
                          .Singleton<WebQueriesViewModel>()
                          .Singleton<YourWebQueriesViewModel>()
                          .Singleton<FileSearchViewModel>()
                          .Singleton<ClipboardManagerViewModel>()
                          .Singleton<ExpansionsViewModel>()
                          .Singleton<ApplicationsViewModel>()
                          .Singleton<CalculatorViewModel>()
                          .Singleton<UrlViewModel>()
                          .Singleton<MicrosoftSettingsViewModel>()
                          .Singleton<RecycleViewModel>()
                          .Singleton<TimerViewModel>()
                          .Singleton<QuitViewModel>();
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            if (_hMutex != IntPtr.Zero)
            {
                ApplicationService.UnregisterInstance(_hMutex);
            }

            base.OnExit(sender, e);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            _hMutex = ApplicationService.RegisterInstance("Global\\Reginald");

            // 0xB7 = ERROR_ALREADY_EXISTS.
            if (_hMutex == IntPtr.Zero || Marshal.GetLastWin32Error() == 0xB7)
            {
                Application.Current.Shutdown();
            }

            _ = DisplayRootViewFor<ShellViewModel>();
        }
    }
}
