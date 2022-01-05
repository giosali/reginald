using Caliburn.Micro;
using Reginald.Core.AbstractProducts;
using Reginald.Core.Extensions;
using Reginald.Core.IO;
using System.Linq;
using System.Windows;

namespace Reginald.ViewModels
{
    public abstract class KeyphraseViewModelBase : DataViewModelBase
    {
        private string _filename;
        public string Filename
        {
            get => _filename;
            set
            {
                _filename = value;
                NotifyOfPropertyChange(() => Filename);
            }
        }

        private BindableCollection<Keyphrase> _keyphrases = new();
        public BindableCollection<Keyphrase> Keyphrases
        {
            get => _keyphrases;
            set
            {
                _keyphrases = value;
                NotifyOfPropertyChange(() => Keyphrases);
            }
        }

        private Keyphrase _selectedKeyphrase;
        public Keyphrase SelectedKeyphrase
        {
            get => _selectedKeyphrase;
            set
            {
                _selectedKeyphrase = value;
                NotifyOfPropertyChange(() => SelectedKeyphrase);
            }
        }

        public KeyphraseViewModelBase(string filename) : base(false)
        {
            Filename = filename;
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
