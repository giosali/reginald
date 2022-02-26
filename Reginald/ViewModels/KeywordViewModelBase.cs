namespace Reginald.ViewModels
{
    using System.Linq;
    using System.Windows;
    using Caliburn.Micro;
    using Reginald.Core.Extensions;
    using Reginald.Core.IO;
    using Reginald.Data.Keywords;

    public class KeywordViewModelBase : ViewViewModelBase
    {
        private BindableCollection<Keyword> _keywords = new();

        private Keyword _selectedKeyword;

        public KeywordViewModelBase(string filename, bool isResource)
        {
            Filename = filename;
            IsResource = isResource;
        }

        public string Filename { get; set; }

        public bool IsResource { get; set; }

        public string FilePath => FileOperations.GetFilePath(Filename, IsResource);

        public BindableCollection<Keyword> Keywords
        {
            get => _keywords;
            set
            {
                _keywords = value;
                NotifyOfPropertyChange(() => Keywords);
            }
        }

        public Keyword SelectedKeyword
        {
            get => _selectedKeyword;
            set
            {
                _selectedKeyword = value;
                NotifyOfPropertyChange(() => SelectedKeyword);
            }
        }

        public virtual void Include_Click(object sender, RoutedEventArgs e)
        {
            FileOperations.WriteFile(ApplicationPaths.SettingsFilename, Settings.Serialize());
        }

        public virtual void KeywordIsEnabled_Click(object sender, RoutedEventArgs e)
        {
            FileOperations.WriteFile(Filename, Keywords.Where(k => !k.IsEnabled)
                                                       .Serialize());
        }
    }
}
