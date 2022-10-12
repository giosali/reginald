namespace Reginald.Models.Drawing
{
    using System.Windows.Media.Imaging;

    internal sealed class Icon
    {
        public Icon(BitmapSource source)
        {
            Source = source;
        }

        public Icon(string path)
        {
            Path = path;
        }

        public string Path { get; private set; }

        public BitmapSource Source { get; private set; }
    }
}
