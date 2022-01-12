using Reginald.Core.AbstractProducts;
using System;
using System.Threading.Tasks;

namespace Reginald.Core.Products
{
    public class SearchResult : DisplayItem
    {
        private Keyword _keyword;
        public Keyword Keyword
        {
            get => _keyword;
            set
            {
                _keyword = value;
                NotifyOfPropertyChange(() => Keyword);
            }
        }

        private ShellItem _shellItem;
        public ShellItem ShellItem
        {
            get => _shellItem;
            set
            {
                _shellItem = value;
                NotifyOfPropertyChange(() => ShellItem);
            }
        }

        private Representation _representation;
        public Representation Representation
        {
            get => _representation;
            set
            {
                _representation = value;
                NotifyOfPropertyChange(() => Representation);
            }
        }

        private Keyphrase _keyphrase;
        public Keyphrase Keyphrase
        {
            get => _keyphrase;
            set
            {
                _keyphrase = value;
                NotifyOfPropertyChange(() => Keyphrase);
            }
        }

        private bool _isAltDown;
        public bool IsAltDown
        {
            get => _isAltDown;
            set
            {
                _isAltDown = value;
                NotifyOfPropertyChange(() => IsAltDown);
            }
        }

        private bool _isPrompted;
        public bool IsPrompted
        {
            get => _isPrompted;
            set
            {
                _isPrompted = value;
                NotifyOfPropertyChange(() => IsPrompted);
            }
        }

        public SearchResult()
        {

        }

        public SearchResult(Keyword keyword)
        {
            Guid = keyword.Guid;
            Name = keyword.Name;
            Icon = keyword.Icon;
            Caption = keyword.Caption;
            Description = keyword.Description;
            Keyword = keyword;
        }

        public SearchResult(ShellItem item)
        {
            Guid = Guid.NewGuid();
            Name = item.Name; 
            Icon = item.Icon;
            Caption = item.Caption;
            Description = item.Description;
            ShellItem = item;
        }

        public SearchResult(Representation representation)
        {
            Guid = representation.Guid;
            Name = representation.Name;
            Icon = representation.Icon;
            Caption = representation.Caption;
            Representation = representation;
            Description = representation.Description;
        }

        public SearchResult(Keyphrase phrase)
        {
            Guid = phrase.Guid;
            Name = phrase.Name;
            Icon = phrase.Icon;
            Caption = phrase.Caption;
            Keyphrase = phrase;
            Description = phrase.Description;
        }

        public override bool Predicate()
        {
            throw new NotImplementedException();
        }

        public override void EnterDown(bool isAltDown, Action action)
        {

        }

        public override async Task<bool> EnterDownAsync(bool isAltDown, Action action, object o)
        {
            if (Keyword is not null)
            {
                Keyword.EnterDown(isAltDown, action);
            }
            else if (ShellItem is not null)
            {
                ShellItem.EnterDown(isAltDown, action);
            }
            else if (Representation is not null)
            {
                Representation.EnterDown(isAltDown, action);
            }
            else if (Keyphrase is not null)
            {
                bool success = await Keyphrase.EnterDownAsync(isAltDown, action, IsPrompted);
                return success;
            }
            return true;
        }

        public override (string, string) AltDown()
        {
            return Keyword?.AltDown()
                ?? ShellItem?.AltDown()
                ?? Representation?.AltDown()
                ?? (Description, Caption);
        }

        public override (string, string) AltUp()
        {
            return Keyword?.AltUp()
                ?? ShellItem?.AltUp()
                ?? Representation?.AltUp()
                ?? (Description, Caption);
        }
    }
}
