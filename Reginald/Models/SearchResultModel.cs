using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Reginald.Models
{
    public class SearchResultModel
    {
        public enum Category
        {
            Math,
            HTTP,
            Keyword,
            SearchEngine
        }

        public string Name { get; set; }
        public Category CategoryName { get; set; }
        public string Icon { get; set; }
        public string Keyword { get; set; }
        public string Separator { get; set; }
        public string URL { get; set; }
        public string Format { get; set; }
        public string Description { get; set; }
        public string Alt { get; set; }
        public string Count { get; set; }
    }
}