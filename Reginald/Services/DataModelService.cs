namespace Reginald.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Reginald.Core.IO;
    using Reginald.Data.DataModels;
    using Reginald.Data.Producers;
    using Reginald.Data.Products;
    using Reginald.Services.Hooks;
    using Reginald.Services.Input;

    internal class DataModelService
    {
#pragma warning disable IDE0052
        private readonly FileSystemWatcher[] _fsws;
#pragma warning restore IDE0052

        private readonly StringBuilder _input = new();

        public DataModelService()
        {
            Settings = FileOperations.GetGenericDatum<Settings>(Settings.FileName, false);
            SetTheme();
            SetTextExpansions();
            SetSingleProducers();
            SetMultipleProducers();

            FileSystemWatcher fsw = new(FileOperations.ApplicationAppDataDirectoryPath, "*.json");
            fsw.NotifyFilter = NotifyFilters.Attributes
                             | NotifyFilters.CreationTime
                             | NotifyFilters.LastAccess
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Size;
            fsw.Changed += OnChanged;
            fsw.EnableRaisingEvents = true;
            _fsws = new FileSystemWatcher[] { fsw };

            // Adds a low-level hook for text expansions.
            KeyboardHook keyboardHook = new();
            keyboardHook.Add();
            keyboardHook.KeyPressed += OnKeyPressed;
        }

        public IMultipleProducer<SearchResult>[] MultipleProducers { get; set; }

        public Settings Settings { get; set; }

        public ISingleProducer<SearchResult>[] SingleProducers { get; set; }

        public TextExpansion[] TextExpansions { get; private set; }

        public Theme Theme { get; set; }

        private TextExpansion GetTextExpansionFromVirtualKeyCode(int vkCode)
        {
            // Handles the Backspace key.
            if (vkCode == 8 && _input.Length > 0)
            {
                _input.Length--;
                return null;
            }

            char c = VirtualKeyCodeConverter.ConvertToChar(vkCode);

            // Ignores '\0'.
            if (char.IsControl(c))
            {
                return null;
            }

            _ = _input.Append(c);

            // This helps keep track of whether the input currently
            // entered is close to fully matching a trigger.
            bool startsWithInput = false;
            string input = _input.ToString();

            // Gives the user some leeway when typing, specifically when
            // the user mistypes the final character of a trigger.
            string shortenedInput = input.Length > 1 ? input[0..^1] : input;
            for (int i = 0; i < TextExpansions.Length; i++)
            {
                string trigger = TextExpansions[i].Trigger;
                if (trigger == input)
                {
                    _ = _input.Clear();
                    return TextExpansions[i];
                }

                if (!startsWithInput)
                {
                    startsWithInput = trigger.StartsWith(input) || trigger.StartsWith(shortenedInput);
                }
            }

            // Resets input if there's no match.
            if (!startsWithInput)
            {
                _ = _input.Clear();
            }

            return null;
        }

        private async void OnKeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (!Settings.AreExpansionsEnabled || !e.IsDown)
            {
                return;
            }

            TextExpansion te = GetTextExpansionFromVirtualKeyCode(e.VirtualKeyCode);
            if (te is null)
            {
                return;
            }

            await Task.Run(async () =>
            {
                await Task.Delay(50);
                KeyboardInputInjector.InjectInput(InjectedKeyboardInput.FromTextExpansion(te.Trigger, te.Replacement));
            });
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                case WatcherChangeTypes.Deleted:
                case WatcherChangeTypes.Changed:
                    SetTheme();
                    SetTextExpansions();
                    SetSingleProducers();
                    SetMultipleProducers();
                    break;
            }
        }

        private void SetMultipleProducers()
        {
            List<IMultipleProducer<SearchResult>> multipleProducers = new();

            // Handles those that receive key inputs.
            Quit quit = FileOperations.GetGenericDatum<Quit>("Quit.json", true);
            quit.IsEnabled = Settings.IsQuitEnabled;
            multipleProducers.Add(quit);

            multipleProducers.Add(FileOperations.GetGenericDatum<Timers>("Timers.json", true));

            MultipleProducers = multipleProducers.ToArray();
        }

        private void SetSingleProducers()
        {
            List<ISingleProducer<SearchResult>> singleProducers = new();

            // Handles those that receive key inputs.
            if (Settings.AreWebQueriesEnabled)
            {
                singleProducers.AddRange(FileOperations.GetGenericData<WebQuery>(WebQuery.FileName, true));
            }

            singleProducers.AddRange(FileOperations.GetGenericData<WebQuery>(WebQuery.UserFileName, false));

            Recycle recycle = FileOperations.GetGenericDatum<Recycle>("Recycle.json", true);
            recycle.IsEnabled = Settings.IsRecycleEnabled;
            singleProducers.Add(recycle);

            Timer timer = FileOperations.GetGenericDatum<Timer>("Timer.json", true);
            timer.IsEnabled = Settings.IsTimerEnabled;
            singleProducers.Add(timer);

            // Handles those that receive inputs.
            Calculator calculator = FileOperations.GetGenericDatum<Calculator>("Calculator.json", true);
            calculator.IsEnabled = Settings.IsCalculatorEnabled;
            singleProducers.Add(calculator);

            Url url = FileOperations.GetGenericDatum<Url>("Url.json", true);
            url.IsEnabled = Settings.IsUrlEnabled;
            singleProducers.Add(url);

            if (Settings.AreMicrosoftSettingsEnabled)
            {
                singleProducers.AddRange(FileOperations.GetGenericData<MicrosoftSetting>("MicrosoftSettings.json", true));
            }

            SingleProducers = singleProducers.ToArray();
        }

        private void SetTextExpansions()
        {
            TextExpansions = FileOperations.GetGenericData<TextExpansion>(TextExpansion.FileName, false);
        }

        private void SetTheme()
        {
            Theme[] themes = FileOperations.GetGenericData<Theme>(Theme.FileName, true);
            Theme = themes.FirstOrDefault(t => t.Guid == Settings.ThemeIdentifier, themes[0]);
        }
    }
}
