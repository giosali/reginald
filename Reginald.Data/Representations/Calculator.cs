namespace Reginald.Data.Representations
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using Reginald.Core.Helpers;
    using Reginald.Core.Mathematics;

    public class Calculator : Representation
    {
        public const string Filename = "Calculator.json";

        public Calculator()
        {
        }

        public Calculator(IRepresentationDataModel model)
        {
            Guid = Guid.Parse(model.Guid);
            Name = model.Name;
            Icon = BitmapImageHelper.FromUri(model.Icon);
            Caption = model.Caption;
            AltCaption = model.AltCaption;
            IsEnabled = model.IsEnabled;
        }

        public override void EnterKeyDown()
        {
            Clipboard.SetText(Description);
        }

        public override void AltKeyDown()
        {
            IsAltKeyDown = true;
        }

        public override void AltKeyUp()
        {
            IsAltKeyDown = false;
        }

        public Task<bool> IsExpression(string input)
        {
            bool success;
            if (success = Interpreter.IsMathExpression(input) && !input.StartsWith(' '))
            {
                Description = Core.Mathematics.Calculator.Calculate(input);
            }

            return Task.FromResult(success);
        }
    }
}
