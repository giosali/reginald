namespace Reginald.Data.Keywords
{
    using System;
    using Reginald.Core.Helpers;

    /// <summary>
    /// Specifies the type of command.
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// The command for timers.
        /// </summary>
        Timer,

        /// <summary>
        /// The command for quitting processes.
        /// </summary>
        Quit,
    }

    public abstract partial class CommandKeyword : Keyword
    {
        public const string Filename = "Commands.json";

        public CommandKeyword()
        {
        }

        public CommandKeyword(CommandKeywordDataModel model)
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
            if (Enum.TryParse(model.CommandType, true, out CommandType command))
            {
                CommandType = command;
            }
        }

        public CommandType CommandType { get; set; }

        public override void EnterKeyDown()
        {
        }

        public override void AltKeyDown()
        {
            IsAltKeyDown = true;
            TempCaption = Caption;
            TempDescription = Description;
        }

        public override void AltKeyUp()
        {
            IsAltKeyDown = false;
            TempCaption = Caption;
            TempDescription = Description;
        }
    }
}
