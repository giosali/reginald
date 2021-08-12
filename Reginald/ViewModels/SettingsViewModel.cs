using Caliburn.Micro;
using ModernWpf.Controls;
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

        public async Task NavigationView_ItemInvokedAsync(NavigationView sender, NavigationViewItemInvokedEventArgs e)
        {
            string invokedItem = e.InvokedItem.ToString();
            switch (invokedItem)
            {
                case "Themes":
                    await ActivateItemAsync(new ThemesViewModel());
                    break;

                case "Search Box":
                    await ActivateItemAsync(new SearchBoxAppearanceViewModel());
                    break;

                case "Default Keywords":
                    await ActivateItemAsync(new DefaultKeywordViewModel());
                    break;

                case "User Keywords":
                    await ActivateItemAsync(new UserKeywordViewModel());
                    break;

                case "Special Keywords":
                    await ActivateItemAsync(new SpecialKeywordViewModel());
                    break;

                case "Commands":
                    await ActivateItemAsync(new CommandsViewModel());
                    break;

                case "Info":
                    await ActivateItemAsync(new InfoViewModel());
                    break;

                case "Settings":
                    await ActivateItemAsync(new GeneralViewModel());
                    break;

                default:
                    break;
            }
        }
    }
}