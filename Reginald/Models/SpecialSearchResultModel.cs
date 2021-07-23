using System.Windows.Media;

namespace Reginald.Models
{
    public class SpecialSearchResultModel
    {
        public SpecialSearchResultModel() { }

        // XML
        public string Name { get; set; }
        public int ID { get; set; }
        public string Keyword { get; set; }
        public string API { get; set; }
        public ImageSource Icon { get; set; }
        public string Description { get; set; }
        public string SubOneFormat { get; set; }
        public string SubTwoFormat { get; set; }
        public bool CanHaveSpaces { get; set; }
        public bool IsEnabled { get; set; }

        // API
        public string Major { get; set; }
        public string Minor { get; set; }
        public string SubOne { get; set; }
        public string SubTwo { get; set; }
    }
}
