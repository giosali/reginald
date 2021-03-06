namespace Reginald
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Caliburn.Micro;
    using Reginald.Services;
    using Reginald.Services.Utilities;
    using Reginald.ViewModels;

    public class Bootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new();

        private IntPtr _hMutext = IntPtr.Zero;

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            IntPtr hMutex = WindowUtility.RegisterInstance("Reginald Single Instance Mutex");
            if (hMutex == IntPtr.Zero)
            {
                Application.Current.Shutdown();
            }

            _hMutext = hMutex;
            _ = DisplayRootViewFor<ShellViewModel>();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            if (_hMutext != IntPtr.Zero)
            {
                WindowUtility.UnregisterInstance(_hMutext);
            }

            base.OnExit(sender, e);
        }

        protected override void Configure()
        {
            _ = _container.Singleton<IWindowManager, WindowManager>()
                          .Singleton<IEventAggregator, EventAggregator>()
                          .Singleton<ConfigurationService>()
                          .Singleton<DataFileService>()
                          .Singleton<UserResourceService>()
                          .Singleton<ShellViewModel>()
                          .Singleton<SettingsViewModel>()
                          .Singleton<GeneralViewModel>()
                          .Singleton<ThemesViewModel>()
                          .Singleton<KeywordsViewModel>()
                          .Singleton<DefaultKeywordViewModel>()
                          .Singleton<UserKeywordViewModel>()
                          .Singleton<CommandKeywordViewModel>()
                          .Singleton<HttpKeywordViewModel>()
                          .Singleton<KeyphrasesViewModel>()
                          .Singleton<UtilityKeyphraseViewModel>()
                          .Singleton<ExpansionsViewModel>()
                          .Singleton<ClipboardManagerViewModel>()
                          .Singleton<AboutViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
