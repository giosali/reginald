using Caliburn.Micro;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Reginald.ViewModels
{
    public class SettingsViewModel : Conductor<object>
    {
        public SettingsViewModel()
        {

        }

        public void OnSettingsButtonClick(UIElement element, RoutedEventArgs e)
        {
            if (element is StackPanel stackPnl)
            {
                foreach (object child in stackPnl.Children)
                {
                    if (child is Button btn)
                    {
                        if (btn.Content is StackPanel btnStackPnl)
                        {
                            if (btnStackPnl.Children.Count == 2)
                                btnStackPnl.Children.RemoveAt(0);
                        }
                    }
                }
            }

            Button sourceBtn = (Button)e.Source;
            if (sourceBtn.Content is StackPanel sourceStackPnl)
            {
                if (sourceStackPnl.Children.Count == 1)
                {
                    Rectangle rectangle = new()
                    {
                        Stroke = Brushes.DarkOrange,
                        Width = 2,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    sourceStackPnl.Children.Insert(0, rectangle);
                }
            }
        }

        public async Task LoadUtilitiesViewAsync(object sender, RoutedEventArgs e)
        {
            await ActivateItemAsync(new UtilitiesViewModel());
        }

        public async Task LoadAppearanceViewAsync(object sender, RoutedEventArgs e)
        {
            await ActivateItemAsync(new AppearanceViewModel());
        }
    }
}