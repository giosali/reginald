using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Shell;
using Reginald.Commands;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace Reginald.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            MakeReginaldAppDataDirectory();
            MakeSearchXmlFile();
            CacheApplicationIcons();
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            foreach (Window window in Application.Current.Windows)
                window.Close();

            return base.OnDeactivateAsync(close, cancellationToken);
        }

        public ShellViewModel()
        {
            OpenWindowCommand = new OpenWindowCommand(ExecuteMethod, CanExecuteMethod);
        }

        private readonly SearchViewModel _searchViewModel = new();
        public SearchViewModel SearchViewModel
        {
            get => _searchViewModel;
        }

        public ICommand OpenWindowCommand { get; set; }

        private bool CanExecuteMethod(object parameter)
        {
            return true;
        }

        private void ExecuteMethod(object parameter)
        {
            if (SearchViewModel.IsActive)
            {
                SearchViewModel.TryCloseAsync();
            }
            else
            {
                IWindowManager manager = new WindowManager();
                manager.ShowWindowAsync(SearchViewModel);
            }
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

        private void TestButton(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button btn)
            {
                if (btn.Name == String.Empty)
                    MessageBox.Show("Yes it's empty");
                else
                    MessageBox.Show("No it's not empty");
            }
        }

        private void TestListBoxItem(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                MessageBox.Show("ListBoxItem interaction");
        }

        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            var list = sender as ListBox;
            switch (e.Key)
            {
                case Key.Right:
                    if (!list.Items.MoveCurrentToNext()) list.Items.MoveCurrentToLast();
                    break;

                case Key.Left:
                    if (!list.Items.MoveCurrentToPrevious()) list.Items.MoveCurrentToFirst();
                    break;
            }

            e.Handled = true;
            if (list.SelectedItem != null)
            {
                (Keyboard.FocusedElement as UIElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        public void TestEmptyListBoxItem(object sender, RoutedEventArgs e)
        {
            string uri = "https://google.com";
            MessageBox.Show(uri);
        }

        public void ShowMessage()
        {
            MessageBox.Show("Message");
        }

        private static void MakeReginaldAppDataDirectory()
        {
            string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string applicationName = "Reginald";
            Directory.CreateDirectory(System.IO.Path.Combine(appDataDirectory, applicationName));
        }

        private static void MakeSearchXmlFile()
        {
            string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string applicationName = "Reginald";
            string searchXml = "Search.xml";
            string path = System.IO.Path.Combine(appDataDirectory, applicationName, searchXml);

            XmlDocument doc = new();
            doc.LoadXml(GetSearchXmlStructure());
            doc.Save(path);
        }

        private static void CacheApplicationIcons()
        {
            string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string applicationName = "Reginald";
            string applicationImagesDirectory = "ApplicationIcons";
            string path = System.IO.Path.Combine(appDataDirectory, applicationName, applicationImagesDirectory);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            Guid applicationsFolderGuid = new Guid("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");
            ShellObject applicationsFolder = (ShellObject)KnownFolderHelper.FromKnownFolderId(applicationsFolderGuid);
            foreach (ShellObject application in (IKnownFolder)applicationsFolder)
            {
                if (!application.Name.EndsWith("url") && !application.ParsingName.EndsWith(".url"))
                {
                    string filename = System.IO.Path.Combine(path, application.Name + ".png");
                    if (!File.Exists(filename))
                    {
                        BitmapSource source = application.Thumbnail.MediumBitmapSource;
                        PngBitmapEncoder encoder = new();
                        BitmapFrame frame = BitmapFrame.Create(source);
                        encoder.Frames.Add(frame);
                        using (var stream = new FileStream(filename, FileMode.Create))
                        {
                            encoder.Save(stream);
                        }
                    }
                }
            }
        }

        private static string GetSearchXmlStructure()
        {
            string xmlFrame = "<?xml version=\"1.0\"?> \n" +
                "<Searches> \n" +
                "    <Namespace Name=\"__math\">" +
                "        <Name>Math</Name> \n" +
                "        <Keyword></Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/calculator.png</Icon> \n" +
                "        <URL></URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>{0}</Format> \n" +
                "        <Alt>Copy to clipboard</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"__http\">" +
                "        <Name>HTTP</Name> \n" +
                "        <Keyword></Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/edge.png</Icon> \n" +
                "        <URL></URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Go to '{0}'</Format> \n" +
                "        <Alt>Website</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"g\">" +
                "        <Name>Google</Name> \n" +
                "        <Keyword>g</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/google.png</Icon> \n" +
                "        <URL>https://google.com/search?q={0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search Google for '{0}'</Format> \n" +
                "        <Alt>Google</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"ddg\">" +
                "        <Name>DuckDuckGo</Name> \n" +
                "        <Keyword>ddg</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/duckduckgo.png</Icon> \n" +
                "        <URL>https://duckduckgo.com/?q={0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search DuckDuckGo for '{0}'</Format> \n" +
                "        <Alt>DuckDuckGo</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"r/\">" +
                "        <Name>Reddit</Name> \n" +
                "        <Keyword>r/</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/reddit.png</Icon> \n" +
                "        <URL>https://reddit.com/r/{0}</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Go to r/{0}</Format> \n" +
                "        <Alt>Reddit</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"twitter\">" +
                "        <Name>Twitter</Name> \n" +
                "        <Keyword>twitter</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/twitter.png</Icon> \n" +
                "        <URL>https://twitter.com/{0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search Twitter for '{0}'</Format> \n" +
                "        <Alt>Twitter</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"twitter\">" +
                "        <Name>Twitter</Name> \n" +
                "        <Keyword>twitter</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/twitter.png</Icon> \n" +
                "        <URL>https://twitter.com/{0}</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Go to {0}'s profile</Format> \n" +
                "        <Alt>Twitter</Alt> \n" +
                "    </Namespace>" +
                "</Searches>";
            return xmlFrame;
        }

        private static XmlNode MakeXmlNode(string keyword, string name, string icon,
                                           string url, string separator, string description)
        {
            string xml = $"<Namespace Name=\"{keyword}\">" +
               $"    <Name>{name}</Name> \n" +
               $"    <Keyword>{keyword}</Keyword> \n" +
               $"    <Icon>{icon}</Icon> \n" +
               $"    <URL>{url}</URL> \n" +
               $"    <Separator>{separator}</Separator> \n" +
               $"    <Description>{description}</Description> \n" +
               "</Namespace>";

            XmlDocument newDoc = new();
            newDoc.LoadXml(xml);
            XmlNode newNode = newDoc.DocumentElement;
            return newNode;
        }
    }
}