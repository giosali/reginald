using Reginald.Core.AbstractProducts;
using Reginald.Core.DataModels;
using Reginald.Core.Enums;
using Reginald.Core.Helpers;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Reginald.Core.Products
{
    public class CommandKeyword : Keyword
    {
        private Command _command;
        public Command Command
        {
            get => _command;
            set
            {
                _command = value;
                NotifyOfPropertyChange(() => Command);
            }
        }

        public CommandKeyword()
        {

        }

        public CommandKeyword(CommandDataModel model)
        {
            if (Guid.TryParse(model.Guid, out Guid guid))
            {
                Guid = guid;
            }
            Name = model.Name;
            Word = model.Keyword;
            Icon = BitmapImageHelper.FromUri(model.Icon);
            Format = model.Format;
            Placeholder = model.Placeholder;
            Caption = model.Caption;
            IsEnabled = model.IsEnabled;
            if (Enum.TryParse(model.Command, true, out Command command))
            {
                Command = command;
            }
        }

        public override bool Predicate(Regex rx, (string Keyword, string Separator, string Description) input)
        {
            return input.Keyword.Length > 0 && rx.IsMatch(Word);
        }

        public override Task<bool> PredicateAsync(Regex rx, (string Keyword, string Separator, string Description) input, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public override void EnterDown(bool isAltDown, Action action)
        {

        }

        public override Task<bool> EnterDownAsync(bool isAltDown, Action action, object o)
        {
            return Task.FromResult(true);
        }

        public override (string, string) AltDown()
        {
            return (null, null);
        }

        public override (string, string) AltUp()
        {
            return (null, null);
        }
    }
}
