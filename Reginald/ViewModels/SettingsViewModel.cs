﻿using Caliburn.Micro;
using HandyControl.Controls;
using HandyControl.Data;
using System.Threading.Tasks;

namespace Reginald.ViewModels
{
    public class SettingsViewModel : Conductor<object>
    {
        public SettingsViewModel()
        {
            SetUpViewModel();
        }

        private async void SetUpViewModel()
        {
            await ActivateItemAsync(new GeneralViewModel());
        }

        public async Task SideMenu_SelectionChangedAsync(object sender, FunctionEventArgs<object> e)
        {
            SideMenuItem sideMenuItem = sender as SideMenuItem;
            switch (sideMenuItem.Name)
            {
                case "Themes":
                    await ActivateItemAsync(new ThemesViewModel());
                    break;

                case "SearchBox":
                    await ActivateItemAsync(new SearchBoxAppearanceViewModel());
                    break;

                case "DefaultKeywords":
                    await ActivateItemAsync(new DefaultKeywordViewModel());
                    break;

                case "UserKeywords":
                    await ActivateItemAsync(new UserKeywordViewModel());
                    break;

                case "SpecialKeywords":
                    await ActivateItemAsync(new SpecialKeywordViewModel());
                    break;

                case "Commands":
                    await ActivateItemAsync(new CommandsViewModel());
                    break;

                case "Utilities":
                    await ActivateItemAsync(new UtilitiesViewModel());
                    break;

                case "Miscellaneous":
                    await ActivateItemAsync(new MiscellaneousViewModel());
                    break;

                case "Info":
                    await ActivateItemAsync(new InfoViewModel());
                    break;

                case "General":
                    await ActivateItemAsync(new GeneralViewModel());
                    break;

                default:
                    break;
            }
        }
    }
}