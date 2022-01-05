using Reginald.Core.AbstractProducts;
using Reginald.Core.DataModels;
using Reginald.Core.Helpers;
using Reginald.Core.Mathematics;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

        public override void EnterDown(Representation representation, bool isAltDown, Action action, object sender)
        {
            Calculator calculator = representation as Calculator;
            if (isAltDown)
            {
                TextBox textBox = sender as TextBox;
                textBox.Text = calculator.Description;
                textBox.SelectionStart = calculator.Description.Length;
            }
            else
            {
                action();
                Clipboard.SetText(calculator.Description);
            }
        }

        public override (string Description, string Caption) AltDown(Representation representation)
        {
            Calculator calculator = representation as Calculator;
            string description = null;
            string caption = calculator.AltCaption;
            return (description, caption);
        }

        public override (string Description, string Caption) AltUp(Representation representation)
        {
            Calculator calculator = representation as Calculator;
            string description = null;
            string caption = calculator.Caption;
            return (description, caption);
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
