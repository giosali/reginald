using System.Windows.Media;

namespace Reginald.Models
{
    public class SearchResultModel
    {
        public enum Category
        {
            Application,
            Math,
            HTTP,
            Keyword,
            SearchEngine
        }

        public string Name { get; set; }
        public Category CategoryName { get; set; }
        public ImageSource Icon { get; set; }
        public string ParsingName { get; set; }
        public int ID { get; set; }
        public string Keyword { get; set; }
        public string Separator { get; set; }
        public string URL { get; set; }
        public string Text { get; set; }
        public string Format { get; set; }
        public string DefaultText { get; set; }
        public string Description { get; set; }
        public string Alt { get; set; }
        public string Count { get; set; }
        public bool IsEnabled { get; set; }
    }
}