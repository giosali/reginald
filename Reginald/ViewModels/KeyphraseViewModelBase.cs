namespace Reginald.ViewModels
{
    using System.Linq;
    using System.Windows;
    using Caliburn.Micro;
    using Reginald.Core.Extensions;
    using Reginald.Core.IO;
    using Reginald.Data.Keyphrases;

    public abstract class KeyphraseViewModelBase : DataViewModelBase
    {
        private BindableCollection<Keyphrase> _keyphrases = new();

        private Keyphrase _selectedKeyphrase;

        public KeyphraseViewModelBase(string filename, bool isResource)
            : base(false)
        {
            Filename = filename;
            IsResource = isResource;
        }

        public string Filename { get; set; }

        public bool IsResource { get; set; }

        public string FilePath => FileOperations.GetFilePath(Filename, IsResource);

        public BindableCollection<Keyphrase> Keyphrases
        {
            get => _keyphrases;
            set
            {
                _keyphrases = value;
                NotifyOfPropertyChange(() => Keyphrases);
            }
        }

        public Keyphrase SelectedKeyphrase
        {
            get => _selectedKeyphrase;
            set
            {
                _selectedKeyphrase = value;
                NotifyOfPropertyChange(() => SelectedKeyphrase);
            }
        }

        public virtual void Include_Click(object sender, RoutedEventArgs e)
        {
            FileOperations.WriteFile(ApplicationPaths.SettingsFilename, Settings.Serialize());
        }

        public virtual void KeyphraseIsEnabled_Click(object sender, RoutedEventArgs e)
        {
            FileOperations.WriteFile(Filename, Keyphrases.Where(k => !k.IsEnabled)
                                                         .Serialize());
        }
    }
}
