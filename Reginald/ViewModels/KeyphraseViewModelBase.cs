namespace Reginald.ViewModels
{
    using System.Linq;
    using System.Windows;
    using Caliburn.Micro;
    using Reginald.Core.AbstractProducts;
    using Reginald.Core.Extensions;
    using Reginald.Core.IO;

    public abstract class KeyphraseViewModelBase : DataViewModelBase
    {
        private string _filename;

        private BindableCollection<Keyphrase> _keyphrases = new();

        private Keyphrase _selectedKeyphrase;

        public KeyphraseViewModelBase(string filename)
            : base(false)
        {
            Filename = filename;
        }

        public string Filename
        {
            get => _filename;
            set
            {
                _filename = value;
                NotifyOfPropertyChange(() => Filename);
            }
        }

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
