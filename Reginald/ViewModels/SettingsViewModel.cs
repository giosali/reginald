namespace Reginald.ViewModels
{
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using HandyControl.Controls;
    using HandyControl.Data;
    using Reginald.Services;

    public class SettingsViewModel : Conductor<object>
    {
        private readonly GeneralViewModel _generalViewModel;

        private readonly ThemesViewModel _themesViewModel;

        private readonly DefaultKeywordViewModel _defaultKeywordViewModel;

        private readonly HttpKeywordsViewModel _httpKeywordsViewModel;

        private readonly CommandsViewModel _commandsViewModel;

        private readonly UtilitiesViewModel _utilitiesViewModel;

        private readonly ExpansionsViewModel _expansionsViewModel;

        private readonly MiscellaneousViewModel _miscellaneousViewModel;

        public SettingsViewModel(ConfigurationService configurationService)
        {
            _generalViewModel = new(configurationService);
            _themesViewModel = new(configurationService);
            _defaultKeywordViewModel = new(configurationService);
            _httpKeywordsViewModel = new(configurationService);
            _commandsViewModel = new(configurationService);
            _utilitiesViewModel = new(configurationService);
            _expansionsViewModel = new(configurationService);
            _miscellaneousViewModel = new(configurationService);
            _ = ActivateItemAsync(_generalViewModel);
        }

        public async Task SideMenu_SelectionChangedAsync(object sender, FunctionEventArgs<object> e)
        {
            SideMenuItem sideMenuItem = sender as SideMenuItem;
            switch (sideMenuItem.Name)
            {
                case "Themes":
                    await ActivateItemAsync(_themesViewModel);
                    break;

                case "DefaultKeywords":
                    await ActivateItemAsync(_defaultKeywordViewModel);
                    break;

                case "UserKeywords":
                    await ActivateItemAsync(new UserKeywordsViewModel());
                    break;

                case "HttpKeywords":
                    await ActivateItemAsync(_httpKeywordsViewModel);
                    break;

                case "Commands":
                    await ActivateItemAsync(_commandsViewModel);
                    break;

                case "Utilities":
                    await ActivateItemAsync(_utilitiesViewModel);
                    break;

                case "Expansions":
                    await ActivateItemAsync(_expansionsViewModel);
                    break;

                case "Miscellaneous":
                    await ActivateItemAsync(_miscellaneousViewModel);
                    break;

                case "Info":
                    await ActivateItemAsync(new InfoViewModel());
                    break;

                case "General":
                    await ActivateItemAsync(_generalViewModel);
                    break;
            }
        }
    }
}
