namespace Reginald.Core.Extensions
{
    using System.Windows.Controls;

    public static class TextBoxExtensions
    {
        public static void SetText(this TextBox textBox, string textData)
        {
            textBox.Text = textData;
            textBox.CaretIndex = textData.Length;
        }
    }
}
