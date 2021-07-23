using Microsoft.WindowsAPICodePack.Shell;
using Reginald.Core.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Reginald.Core.IO
{
    public static class FileOperations
    {
        /// <summary>
        /// Returns the XML structure for default keywords.
        /// </summary>
        /// <returns>The XML structure for default keywords.</returns>
        public static string GetDefaultKeywordsXml()
        {
            string xml = "<?xml version=\"1.0\"?> \n" +
                "<Searches> \n" +
                "    <Namespace Name=\"__math\" ID=\"1\">" +
                "        <Name>Math</Name> \n" +
                "        <Keyword></Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/calculator.png</Icon> \n" +
                "        <URL></URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>{0}</Format> \n" +
                "        <Alt>Copy to clipboard</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"__http\" ID=\"2\">" +
                "        <Name>HTTP</Name> \n" +
                "        <Keyword></Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/edge.png</Icon> \n" +
                "        <URL></URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Go to '{0}'</Format> \n" +
                "        <Alt>Website</Alt> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"g\" ID=\"3\">" +
                "        <Name>Google</Name> \n" +
                "        <Keyword>g</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/google.png</Icon> \n" +
                "        <URL>https://google.com/search?q={0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search Google for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>Google</Alt> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"ddg\" ID=\"4\">" +
                "        <Name>DuckDuckGo</Name> \n" +
                "        <Keyword>ddg</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/duckduckgo.png</Icon> \n" +
                "        <URL>https://duckduckgo.com/?q={0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search DuckDuckGo for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>DuckDuckGo</Alt> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"amazon\" ID=\"5\">" +
                "        <Name>Amazon</Name> \n" +
                "        <Keyword>amazon</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/amazon.png</Icon> \n" +
                "        <URL>https://www.amazon.com/s?k={0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search Amazon for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>Amazon</Alt> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"ebay\" ID=\"6\">" +
                "        <Name>eBay</Name> \n" +
                "        <Keyword>ebay</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/ebay.png</Icon> \n" +
                "        <URL>https://ebay.com/sch/i.html?_nkw={0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search eBay for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>eBay</Alt> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"fb\" ID=\"7\">" +
                "        <Name>Facebook</Name> \n" +
                "        <Keyword>fb</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/facebook.png</Icon> \n" +
                "        <URL>https://facebook.com/search/top?q={0}</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Search Facebook for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>Facebook</Alt> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"git\" ID=\"8\">" +
                "        <Name>GitHub</Name> \n" +
                "        <Keyword>git</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/github.png</Icon> \n" +
                "        <URL>https://github.com/search?q={0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search GitHub for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>GitHub</Alt> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"ig\" ID=\"9\">" +
                "        <Name>Instagram</Name> \n" +
                "        <Keyword>ig</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/instagram.png</Icon> \n" +
                "        <URL>https://instagram.com/{0}</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Go to {0}'s profile</Format> \n" +
                "        <DefaultText>_</DefaultText> \n" +
                "        <Alt>Instagram</Alt> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"imdb\" ID=\"10\">" +
                "        <Name>IMDb</Name> \n" +
                "        <Keyword>imdb</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/imdb.png</Icon> \n" +
                "        <URL>https://imdb.com/find?q={0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search IMDb for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>IMDb</Alt> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"pinterest\" ID=\"11\">" +
                "        <Name>Pinterest</Name> \n" +
                "        <Keyword>pinterest</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/pinterest.png</Icon> \n" +
                "        <URL>https://pinterest.com/search/pins/?q={0}</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Search Pinterest for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>Pinterest</Alt> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"r/\" ID=\"12\">" +
                "        <Name>Reddit</Name> \n" +
                "        <Keyword>r/</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/reddit.png</Icon> \n" +
                "        <URL>https://reddit.com/r/{0}</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Go to r/{0}</Format> \n" +
                "        <DefaultText></DefaultText> \n" +
                "        <Alt>Reddit</Alt> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"tiktok\" ID=\"13\">" +
                "        <Name>TikTok</Name> \n" +
                "        <Keyword>tiktok</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/tiktok.png</Icon> \n" +
                "        <URL>https://tiktok.com/@{0}</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Go to {0}'s profile</Format> \n" +
                "        <DefaultText>_</DefaultText> \n" +
                "        <Alt>TikTok</Alt> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"tiktok\" ID=\"14\">" +
                "        <Name>TikTok</Name> \n" +
                "        <Keyword>tiktok</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/tiktok.png</Icon> \n" +
                "        <URL>https://tiktok.com/search?q={0}</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Search TikTok for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>TikTok</Alt> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"tumblr\" ID=\"15\">" +
                "        <Name>tumblr</Name> \n" +
                "        <Keyword>tumblr</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/tumblr.png</Icon> \n" +
                "        <URL>https://{0}.tumblr.com</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Go to {0}'s blog</Format> \n" +
                "        <DefaultText>_</DefaultText> \n" +
                "        <Alt>tumblr</Alt> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"tumblr\" ID=\"16\">" +
                "        <Name>tumblr</Name> \n" +
                "        <Keyword>tumblr</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/tumblr.png</Icon> \n" +
                "        <URL>https://tumblr.com/search/{0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search tumblr for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>tumblr</Alt> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"twitch\" ID=\"17\">" +
                "        <Name>Twitch</Name> \n" +
                "        <Keyword>twitch</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/twitch.png</Icon> \n" +
                "        <URL>https://twitch.tv/{0}</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Go to {0}'s channel</Format> \n" +
                "        <DefaultText>_</DefaultText> \n" +
                "        <Alt>Twitch</Alt> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"twitter\" ID=\"18\">" +
                "        <Name>Twitter</Name> \n" +
                "        <Keyword>twitter</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/twitter.png</Icon> \n" +
                "        <URL>https://twitter.com/{0}</URL> \n" +
                "        <Separator></Separator> \n" +
                "        <Format>Go to {0}'s profile</Format> \n" +
                "        <DefaultText>_</DefaultText> \n" +
                "        <Alt>Twitter</Alt> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"twitter\" ID=\"19\">" +
                "        <Name>Twitter</Name> \n" +
                "        <Keyword>twitter</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/twitter.png</Icon> \n" +
                "        <URL>https://twitter.com/{0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search Twitter for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>Twitter</Alt> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"wiki\" ID=\"20\">" +
                "        <Name>Wikipedia</Name> \n" +
                "        <Keyword>wiki</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/wikipedia.png</Icon> \n" +
                "        <URL>https://wikipedia.org/wiki/Special:Search?search={0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search Wikipedia for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>Wikipedia</Alt> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "    <Namespace Name=\"yt\" ID=\"21\">" +
                "        <Name>YouTube</Name> \n" +
                "        <Keyword>yt</Keyword> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/youtube.png</Icon> \n" +
                "        <URL>https://youtube.com/results?search_query={0}</URL> \n" +
                "        <Separator>+</Separator> \n" +
                "        <Format>Search YouTube for '{0}'</Format> \n" +
                "        <DefaultText>...</DefaultText> \n" +
                "        <Alt>YouTube</Alt> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "</Searches>";
            return xml;
        }

        /// <summary>
        /// Creates a XML file on disk from provided xml content.
        /// </summary>
        /// <param name="xml">The XML content.</param>
        /// <param name="filename">The name of the file to be saved in the application's %AppData% directory.</param>
        public static void MakeXmlFile(string xml, string filename)
        {
            XmlDocument doc = new();
            doc.LoadXml(xml);
            string path = Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName, filename);
            if (!File.Exists(path))
            {
                doc.Save(path);
            }
        }

        /// <summary>
        /// Updates a XML file on disk.
        /// </summary>
        /// <param name="xml">The XML content to compare against.</param>
        /// <param name="filename">The name of the XML file to update in the application's %AppData% directory.</param>
        public static void UpdateXmlFile(string xml, string filename)
        {
            XmlDocument doc = new();
            doc.LoadXml(xml);
            string path = Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName, filename);
            if (File.Exists(path))
            {
                XmlDocument docOnDisk = new();
                docOnDisk.Load(path);

                XmlNodeList docOnDiskNamespaceNodes = docOnDisk.SelectNodes(Constants.NamespacesXpath);
                XmlNodeList docNamespaceNodes = doc.SelectNodes(Constants.NamespacesXpath);
                for (int i = 0; i < docNamespaceNodes.Count; i++)
                {
                    try
                    {
                        XmlNode isEnabledNode = docNamespaceNodes[i].SelectSingleNode("IsEnabled");
                        if (isEnabledNode is not null)
                        {
                            isEnabledNode.InnerText = docOnDiskNamespaceNodes[i].SelectSingleNode("IsEnabled").InnerText;
                        }
                    }
                    catch (ArgumentOutOfRangeException) { }
                }
                doc.Save(path);
            }
        }

        /// <summary>
        /// Creates a XML file for user keywords.
        /// </summary>
        public static void MakeUserKeywordsXmlFile()
        {
            string path = Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName, ApplicationPaths.XmlUserKeywordFilename);
            if (!File.Exists(path))
            {
                XmlDocument doc = new();
                string xmlStructure = "<?xml version=\"1.0\"?> \n" +
                "<Searches> \n" +
                "</Searches>";
                doc.LoadXml(xmlStructure);
                doc.Save(path);
            }
        }

        /// <summary>
        /// Creates the "Reginald/ApplicationIcons" directory in %AppData% and creates PNG files containing images from installed applications.
        /// </summary>
        public static void CacheApplicationIcons()
        {
            string path = Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName, ApplicationPaths.IconsDirectoryName);
            Directory.CreateDirectory(path);

            //Guid applicationsFolderGuid = new Guid("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");
            ShellObject applicationsFolder = (ShellObject)KnownFolderHelper.FromKnownFolderId(Constants.ApplicationsGuid);
            foreach (ShellObject application in (IKnownFolder)applicationsFolder)
            {
                if (!application.Name.EndsWith(".url") && !application.ParsingName.EndsWith("url"))
                {
                    string filename = Path.Combine(path, application.Name + ".png");
                    if (!File.Exists(filename))
                    {
                        BitmapSource source = application.Thumbnail.MediumBitmapSource;
                        PngBitmapEncoder encoder = new();
                        BitmapFrame frame = BitmapFrame.Create(source);
                        encoder.Frames.Add(frame);
                        using (FileStream stream = new(filename, FileMode.Create))
                        {
                            encoder.Save(stream);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a text file containing a list of names of installed applications.
        /// </summary>
        public async static void MakeApplicationsTextFile()
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
            string path = Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName, ApplicationPaths.TxtFilename);
            await File.WriteAllLinesAsync(path, applicationNames);
        }

        public static string GetSpecialKeywordsXml()
        {
            string xml = "<?xml version=\"1.0\"?> \n" +
                "<Searches> \n" +
                "    <Namespace Name=\"stock\" ID=\"0\">" +
                "        <Name>Stock</Name> \n" +
                "        <Keyword>stock</Keyword> \n" +
                "        <API>Styvio</API> \n" +
                "        <Icon>pack://application:,,,/Reginald;component/Images/calculator.png</Icon> \n" +
                "        <Description>Stock information for '{0}'</Description> \n" +
                "        <SubOneFormat>MAX: {0}</SubOneFormat> \n" +
                "        <SubTwoFormat>MIN: {0}</SubTwoFormat> \n" +
                "        <CanHaveSpaces>false</CanHaveSpaces> \n" +
                "        <IsEnabled>true</IsEnabled> \n" +
                "    </Namespace>" +
                "</Searches>";
            return xml;
        }
    }
}
