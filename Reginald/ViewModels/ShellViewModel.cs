using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Shell;
using Reginald.Commands;
using System;
using System.Collections.Generic;
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
            SetUpApplication();
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

        private async static void SetUpApplication()
        {
            string appDataDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string applicationName = "Reginald";
            string iconsDirectoryName = "ApplicationIcons";
            string xmlFilename = "Search.xml";
            string xmlSearchContent = GetSearchXmlStructure();
            string txtFilename = "Applications.txt";

            MakeAppDataDirectory(appDataDirectoryPath, applicationName);
            await MakeXmlFile(appDataDirectoryPath, applicationName, xmlFilename, xmlSearchContent);
            await CacheApplicationIcons(appDataDirectoryPath, applicationName, iconsDirectoryName);
            await MakeApplicationsTextFile(appDataDirectoryPath, applicationName, txtFilename);
        }

        private static void MakeAppDataDirectory(string appDataDirectoryPath, string applicationName)
        {
            Directory.CreateDirectory(System.IO.Path.Combine(appDataDirectoryPath, applicationName));
        }

        private static Task MakeXmlFile(string appDataDirectoryPath, string applicationName, string xmlFilename, string xmlContent)
        {
            Task task = Task.Run(() =>
            {
                XmlDocument doc = new();
                doc.LoadXml(xmlContent);
                string path = System.IO.Path.Combine(appDataDirectoryPath, applicationName, xmlFilename);
                doc.Save(path);
            });
            return task;
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
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>Google</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"ddg\">" +
                "        <Name>DuckDuckGo</Name> \n" +
                "        <Keyword>ddg</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/duckduckgo.png</Icon> \n" +
                "        <URL>https://duckduckgo.com/?q={0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search DuckDuckGo for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>DuckDuckGo</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"amazon\">" +
                "        <Name>Amazon</Name> \n" +
                "        <Keyword>amazon</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/amazon.png</Icon> \n" +
                "        <URL>https://www.amazon.com/s?k={0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search Amazon for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>Amazon</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"ebay\">" +
                "        <Name>eBay</Name> \n" +
                "        <Keyword>yt</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/ebay.png</Icon> \n" +
                "        <URL>https://ebay.com/sch/i.html?_nkw={0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search eBay for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>eBay</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"fb\">" +
                "        <Name>Facebook</Name> \n" +
                "        <Keyword>fb</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/facebook.png</Icon> \n" +
                "        <URL>https://facebook.com/search/top?q={0}</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Search Facebook for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>Facebook</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"git\">" +
                "        <Name>GitHub</Name> \n" +
                "        <Keyword>git</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/github.png</Icon> \n" +
                "        <URL>https://github.com/search?q={0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search GitHub for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>GitHub</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"ig\">" +
                "        <Name>Instagram</Name> \n" +
                "        <Keyword>ig</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/instagram.png</Icon> \n" +
                "        <URL>https://instagram.com/{0}</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Go to \"{0}\"'s profile</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>Instagram</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"imdb\">" +
                "        <Name>IMDb</Name> \n" +
                "        <Keyword>imdb</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/imdb.png</Icon> \n" +
                "        <URL>https://imdb.com/find?q={0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search IMDb for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>IMDb</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"pinterest\">" +
                "        <Name>Pinterest</Name> \n" +
                "        <Keyword>pinterest</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/pinterest.png</Icon> \n" +
                "        <URL>https://pinterest.com/search/pins/?q={0}</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Search Pinterest for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>Pinterest</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"r/\">" +
                "        <Name>Reddit</Name> \n" +
                "        <Keyword>r/</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/reddit.png</Icon> \n" +
                "        <URL>https://reddit.com/r/{0}</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Go to r/{0}</Format> \n" +
                "        <DefaultText></DefaultText> \n" +
                "        <Alt>Reddit</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"tiktok\">" +
                "        <Name>TikTok</Name> \n" +
                "        <Keyword>tiktok</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/tiktok.png</Icon> \n" +
                "        <URL>https://tiktok.com/@{0}</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Go to \"{0}\"'s profile</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>TikTok</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"tiktok\">" +
                "        <Name>TikTok</Name> \n" +
                "        <Keyword>tiktok</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/tiktok.png</Icon> \n" +
                "        <URL>https://tiktok.com/search?q={0}</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Search TikTok for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>TikTok</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"tumblr\">" +
                "        <Name>tumblr</Name> \n" +
                "        <Keyword>tumblr</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/tumblr.png</Icon> \n" +
                "        <URL>https://{0}.tumblr.com</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Go to \"{0}\"'s profile</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>tumblr</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"tumblr\">" +
                "        <Name>tumblr</Name> \n" +
                "        <Keyword>tumblr</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/tumblr.png</Icon> \n" +
                "        <URL>https://tumblr.com/search/{0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search tumblr for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>tumblr</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"twitch\">" +
                "        <Name>Twitch</Name> \n" +
                "        <Keyword>twitch</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/twitch.png</Icon> \n" +
                "        <URL>https://twitch.tv/{0}</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Go to \"{0}\"'s channel</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>Twitch</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"twitter\">" +
                "        <Name>Twitter</Name> \n" +
                "        <Keyword>twitter</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/twitter.png</Icon> \n" +
                "        <URL>https://twitter.com/{0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search Twitter for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>Twitter</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"twitter\">" +
                "        <Name>Twitter</Name> \n" +
                "        <Keyword>twitter</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/twitter.png</Icon> \n" +
                "        <URL>https://twitter.com/{0}</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Go to \"{0}\"'s profile</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>Twitter</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"wiki\">" +
                "        <Name>Wikipedia</Name> \n" +
                "        <Keyword>wiki</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/wikipedia.png</Icon> \n" +
                "        <URL>https://wikipedia.org/wiki/Special:Search?search={0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search Wikipedia for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>Wikipedia</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"yt\">" +
                "        <Name>YouTube</Name> \n" +
                "        <Keyword>yt</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/youtube.png</Icon> \n" +
                "        <URL>https://youtube.com/results?search_query={0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search YouTube for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>YouTube</Alt> \n" +
                "    </Namespace>" +
                "</Searches>";
            return xmlFrame;
        }

        private static Task CacheApplicationIcons(string appDataDirectory, string applicationName, string iconsDirectoryName)
        {
            Task task = Task.Run(() =>
            {
                string path = System.IO.Path.Combine(appDataDirectory, applicationName, iconsDirectoryName);
                Directory.CreateDirectory(path);

                Guid applicationsFolderGuid = new Guid("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");
                ShellObject applicationsFolder = (ShellObject)KnownFolderHelper.FromKnownFolderId(applicationsFolderGuid);
                foreach (ShellObject application in (IKnownFolder)applicationsFolder)
                {
                    if (!application.Name.EndsWith(".url") && !application.ParsingName.EndsWith("url"))
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
            });
            return task;
        }

        private static Task MakeApplicationsTextFile(string appDataDirectory, string applicationName, string txtFilename)
        {
            Task task = Task.Run(async () =>
            {
                List<string> applicationNames = new();
                Guid applicationsFolderGuid = new Guid("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");
                ShellObject applicationsFolder = (ShellObject)KnownFolderHelper.FromKnownFolderId(applicationsFolderGuid);
                foreach (ShellObject application in (IKnownFolder)applicationsFolder)
                {
                    if (!application.Name.EndsWith(".url") && !application.ParsingName.EndsWith("url"))
                    {
                        applicationNames.Add(application.Name);
                    }
                }
                string path = System.IO.Path.Combine(appDataDirectory, applicationName, txtFilename);
                await File.WriteAllLinesAsync(path, applicationNames);
            });
            return task;
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