namespace Reginald.Data.Expansions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Reginald.Core.Helpers;
    using Reginald.Core.IO;
    using Reginald.Data.Settings;
    using Reginald.Services.Hooks;
    using Reginald.Services.Input;

    public class TextExpansionManager
    {
        private readonly FileSystemWatcher[] _watchers;

        public TextExpansionManager(bool areExpansionsEnabled)
        {
            AreExpansionsEnabled = areExpansionsEnabled;
            UpdateTextExpansions();
            _watchers = new FileSystemWatcher[]
            {
                FileSystemWatcherHelper.Initialize(FileOperations.ApplicationAppDataDirectoryPath, SettingsDataModel.Filename, OnSettingsChanged),
                FileSystemWatcherHelper.Initialize(FileOperations.ApplicationAppDataDirectoryPath, TextExpansion.Filename, OnTextExpansionsChanged),
            };
        }

        protected bool AreExpansionsEnabled { get; set; }

        private List<TextExpansion> TextExpansions { get; set; } = new();

        private StringBuilder Input { get; set; } = new();

        public async void KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (AreExpansionsEnabled && e.IsDown)
            {
                TextExpansion textExpansion = GetTextExpansionFromVirtualKeyCode(e.VirtualKeyCode);
                if (textExpansion is not null)
                {
                    await Task.Run(async () =>
                    {
                        await Task.Delay(50);
                        KeyboardInputInjector.InjectInput(InjectedKeyboardInput.FromTextExpansion(textExpansion.Trigger, textExpansion.Replacement));
                    });
                }
            }
        }

        private TextExpansion GetTextExpansionFromVirtualKeyCode(int vkCode)
        {
            // Handles the Backspace key
            if (vkCode == 8 && Input.Length > 0)
            {
                Input.Length--;
            }
            else
            {
                char c = VirtualKeyCodeConverter.ConvertToChar(vkCode);

                // Ignores '\0'
                if (!char.IsControl(c))
                {
                    _ = Input.Append(c);

                    // This helps keep track of whether the input currently
                    // entered is close to fully matching a trigger
                    bool startsWithInput = false;
                    string input = Input.ToString();

                    // Gives the user some leeway when typing, specifically when
                    // the user mistypes the final character of a trigger
                    string shortenedInput = input.Length > 1 ? input[0..^1] : input;
                    for (int i = 0; i < TextExpansions.Count; i++)
                    {
                        string trigger = TextExpansions[i].Trigger;
                        if (trigger == input)
                        {
                            _ = Input.Clear();
                            return TextExpansions[i];
                        }
                        else if (!startsWithInput)
                        {
                            startsWithInput = trigger.StartsWith(input) || trigger.StartsWith(shortenedInput);
                        }
                    }

                    // Resets input if there's no match
                    if (!startsWithInput)
                    {
                        _ = Input.Clear();
                    }
                }
            }

            return null;
        }

        private void OnTextExpansionsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                UpdateTextExpansions();
            }
        }

        private void OnSettingsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                string filePath = FileOperations.GetFilePath(SettingsDataModel.Filename, false);
                SettingsDataModel settings = FileOperations.DeserializeFile<SettingsDataModel>(filePath);
                AreExpansionsEnabled = settings.AreExpansionsEnabled;
            }
        }

        private void UpdateTextExpansions()
        {
            string expansionsFilePath = FileOperations.GetFilePath(TextExpansion.Filename, false);
            IEnumerable<TextExpansion> textExpansions = FileOperations.GetGenericData<TextExpansion>(expansionsFilePath);
            foreach (TextExpansion textExpansion in textExpansions)
            {
                textExpansion.Replacement = textExpansion.Replacement.Replace("\r", string.Empty);
            }

            TextExpansions.Clear();
            TextExpansions.AddRange(textExpansions);
        }
    }
}
