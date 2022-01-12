using Reginald.Core.AbstractProducts;
using Reginald.Core.DataModels;
using Reginald.Core.Helpers;
using Reginald.Core.Mathematics;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Reginald.Core.Products
{
    public class Calculator : Representation
    {
        public Calculator(InputDataModelBase model)
        {
            if (Guid.TryParse(model.Guid, out Guid guid))
            {
                Guid = guid;
            }
            Name = model.Name;
            Icon = BitmapImageHelper.FromUri(model.Icon);
            Caption = model.Caption;
            AltCaption = model.AltCaption;
            IsEnabled = model.IsEnabled;
        }

        public override void EnterDown(bool isAltDown, Action action)
        {
            action();
            Clipboard.SetText(Description);
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

        public Task<bool> IsExpression(string input)
        {
            bool success;
            if (success = Interpreter.IsMathExpression(input) && !input.StartsWith(' '))
            {
                Description = Mathematics.Calculator.Calculate(input);
            }
            return Task.FromResult(success);
        }
    }
}
