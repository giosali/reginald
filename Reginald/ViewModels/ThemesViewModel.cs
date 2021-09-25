using Caliburn.Micro;
using Reginald.Core.Base;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using Reginald.Extensions;
using Reginald.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Xml;

namespace Reginald.ViewModels
{
    public class ThemesViewModel : Screen
    {
        public ThemesViewModel(System.Action action)
        {
            Action = action;

            Themes.AddRange(GetThemes(ApplicationPaths.XmlThemesFileLocation, true));
            SelectedTheme = GetThemeFromIdentifier(Themes, Identifier);
        }

        private System.Action Action { get; set; }

        private BindableCollection<ThemeModel> _themes = new();
        public BindableCollection<ThemeModel> Themes
        {
            get => _themes;
            set
            {
                _themes = value;
                NotifyOfPropertyChange(() => Themes);
            }
        }

        private ThemeModel _selectedTheme;
        public ThemeModel SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;
                NotifyOfPropertyChange(() => SelectedTheme);
            }
        }

        private Guid _identifier = Properties.Settings.Default.ThemeIdentifier;
        public Guid Identifier
        {
            get => _identifier;
            set
            {
                _identifier = value;
                NotifyOfPropertyChange(() => Identifier);
            }
        }

        public void Themes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Properties.Settings.Default.ThemeIdentifier = SelectedTheme.Identifier;
            Properties.Settings.Default.Save();
            Action();
        }

        private static List<ThemeModel> GetThemes(string name, bool isLocal = false)
        {
            XmlDocument doc =  XmlHelper.GetXmlDocument(ApplicationPaths.XmlThemesFileLocation, isLocal);
            XmlNodeList nodes = doc.GetNodes(Constants.ThemesXpath);
            List<ThemeModel> themes = new();
            for (int i = 0; i < nodes.Count; i++)
            {
                themes.Add(new ThemeModel(nodes[i]));
            }
            return themes;
        }

        private static ThemeModel GetThemeFromIdentifier(BindableCollection<ThemeModel> themes, Guid identifier)
        {
            for (int i = 0; i < themes.Count; i++)
            {
                ThemeModel theme = themes[i];
                if (theme.Identifier == identifier)
                {
                    return theme;
                }
            }
            return null;
        }
    }
}
