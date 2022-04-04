namespace Reginald
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Caliburn.Micro;
    using Reginald.Services;
    using Reginald.ViewModels;

    public class Bootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            _ = DisplayRootViewFor<ShellViewModel>();
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
                          .Singleton<KeyphrasesViewModel>()
                          .Singleton<DefaultKeywordViewModel>()
                          .Singleton<UserKeywordViewModel>()
                          .Singleton<HttpKeywordViewModel>()
                          .Singleton<CommandKeywordViewModel>()
                          .Singleton<UtilityKeyphraseViewModel>()
                          .Singleton<ExpansionsViewModel>()
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
