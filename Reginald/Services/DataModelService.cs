namespace Reginald.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Reginald.Core.Extensions;
    using Reginald.Core.IO;
    using Reginald.Core.IO.Hooks;
    using Reginald.Core.IO.Injection;
    using Reginald.Models.DataModels;
    using Reginald.Models.Producers;
    using Reginald.Models.Products;

    internal sealed class DataModelService
    {
        private const string CursorVariable = "{{__cursor__}}";

#pragma warning disable IDE0052
        private readonly FileSystemWatcher[] _fsws;
#pragma warning restore IDE0052

        private readonly StringBuilder _input = new();

        private bool _areApplicationsEnabled;

        private bool _isFileSearchEnabled;

        public DataModelService()
        {
            Settings = FileOperations.GetGenericDatum<Settings>(Settings.FileName, false);
            _areApplicationsEnabled = Settings.AreApplicationsEnabled;
            _isFileSearchEnabled = Settings.IsFileSearchEnabled;
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
            keyboardHook.KeyPress += OnKeyPress;
        }

        public IMultipleProducer<SearchResult>[] CpuIntensiveMultipleProducers { get; private set; }

        public IEnumerable<WebQuery> DefaultWebQueries { get; private set; }

        public FileSystemEntrySearch FileSystemEntrySearch { get; private set; }

        public IMultipleProducer<SearchResult>[] MultipleProducers { get; private set; }

        public Settings Settings { get; private set; }

        public ISingleProducer<SearchResult>[] SingleProducers { get; private set; }

        public TextExpansion[] TextExpansions { get; private set; }

        public Theme Theme { get; private set; }

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

        private async void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Settings.AreExpansionsEnabled || !e.IsDown)
            {
                return;
            }

            if (GetTextExpansionFromVirtualKeyCode(e.VirtualKeyCode) is not TextExpansion te)
            {
                return;
            }

            await Task.Run(async () =>
            {
                if (te.Replacement is not string replacement)
                {
                    return;
                }

                await Task.Delay(50);

                // Simulates backspace to delete the trigger.
                InjectedInputKeyboardInfo backInfo = InjectedInputKeyboardInfo.FromRepeatVirtualKey(VK.BACK, te.Trigger.Length);

                int cursorIndex = replacement.IndexOf(CursorVariable);
                int leftArrowCount = 0;
                if (cursorIndex > 0)
                {
                    replacement = replacement.Replace(CursorVariable, string.Empty, 1);
                    leftArrowCount = replacement.Length - cursorIndex;
                }

                InjectedInputKeyboardInfo replacementInfo = InjectedInputKeyboardInfo.FromString(replacement);

                // Simulates left arrow key to set cursor position.
                InjectedInputKeyboardInfo leftArrowInfo = InjectedInputKeyboardInfo.FromRepeatVirtualKey(VK.LEFT, leftArrowCount);

                InputInjector.InjectKeyboardInput(backInfo);
                InputInjector.InjectKeyboardInput(replacementInfo);
                InputInjector.InjectKeyboardInput(leftArrowInfo);
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

                    ObjectModelService oms = IoC.Get<ObjectModelService>();
                    if (_areApplicationsEnabled != Settings.AreApplicationsEnabled)
                    {
                        _areApplicationsEnabled = !_areApplicationsEnabled;
                        oms.SetSingleProducers();
                    }

                    if (_isFileSearchEnabled != Settings.IsFileSearchEnabled)
                    {
                        _isFileSearchEnabled = !_isFileSearchEnabled;
                        oms.ManageFileSystemEntries(_isFileSearchEnabled);
                    }

                    break;
            }
        }

        private void SetMultipleProducers()
        {
            // Handles those that receive key inputs.
            Quit quit = FileOperations.GetGenericDatum<Quit>("Quit.json", true);
            quit.IsEnabled = Settings.IsQuitEnabled;

            ForceQuit forceQuit = FileOperations.GetGenericDatum<ForceQuit>("ForceQuit.json", true);
            forceQuit.IsEnabled = Settings.IsForceQuitEnabled;

            CpuIntensiveMultipleProducers = new IMultipleProducer<SearchResult>[]
            {
                quit,
                forceQuit,
            };

            Timers timers = FileOperations.GetGenericDatum<Timers>("Timers.json", true);
            timers.IsEnabled = Settings.AreTimersEnabled;

            MultipleProducers = new IMultipleProducer<SearchResult>[] { timers };
        }

        private void SetSingleProducers()
        {
            List<ISingleProducer<SearchResult>> singleProducers = new();

            // Handles those that receive key inputs.
            WebQuery[] webQueries = FileOperations.GetGenericData<WebQuery>(WebQuery.FileName, true);
            WebQuery[] yourWebQueries = FileOperations.GetGenericData<WebQuery>(WebQuery.UserFileName, false);
            DefaultWebQueries = webQueries.Concat(yourWebQueries)
                                          .Where(wq => Array.Exists(Settings.DefaultWebQueries, i => i == wq.Id))
                                          .Take(3);
            if (Settings.AreWebQueriesEnabled)
            {
                for (int i = 0; i < Settings.DisabledWebQueries.Count; i++)
                {
                    int id = Settings.DisabledWebQueries[i];
                    for (int j = 0; j < webQueries.Length; j++)
                    {
                        WebQuery wq = webQueries[j];
                        if (id == wq.Id)
                        {
                            wq.IsEnabled = false;
                        }
                    }
                }

                singleProducers.AddRange(webQueries);
            }

            for (int i = 0; i < Settings.DisabledWebQueries.Count; i++)
            {
                int id = Settings.DisabledWebQueries[i];
                for (int j = 0; j < yourWebQueries.Length; j++)
                {
                    WebQuery wq = yourWebQueries[j];
                    if (id == wq.Id)
                    {
                        wq.IsEnabled = false;
                    }
                }
            }

            singleProducers.AddRange(yourWebQueries);

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

            ClearClipboard clearClipboard = FileOperations.GetGenericDatum<ClearClipboard>("ClearClipboard.json", true);
            clearClipboard.IsEnabled = Settings.IsClearClipboardEnabled;
            singleProducers.Add(clearClipboard);

            Url url = FileOperations.GetGenericDatum<Url>("Url.json", true);
            url.IsEnabled = Settings.IsUrlEnabled;
            singleProducers.Add(url);

            if (Settings.AreMicrosoftSettingsEnabled)
            {
                singleProducers.AddRange(FileOperations.GetGenericData<MicrosoftSetting>("MicrosoftSettings.json", true));
            }

            FileSystemEntrySearch = FileOperations.GetGenericDatum<FileSystemEntrySearch>("FileSystemEntrySearch.json", true);

            SingleProducers = singleProducers.ToArray();
        }

        private void SetTextExpansions()
        {
            TextExpansions = FileOperations.GetGenericData<TextExpansion>(TextExpansion.FileName, false);
        }

        private void SetTheme()
        {
            Theme[] themes = FileOperations.GetGenericData<Theme>(Theme.FileName, true);
            Theme theme = themes.FirstOrDefault(t => t.Id == Settings.ThemeId, themes[0]);
            if (Theme?.Id != theme.Id)
            {
                Theme = theme;
            }
        }
    }
}
