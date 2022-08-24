namespace Reginald.Data.Drawing
{
    using System.Windows.Media.Imaging;

    public class Icon
    {
        public Icon(BitmapSource source)
        {
            Source = source;
        }

        public Icon(string path)
        {
            Path = path;
        }

        public string Path { get; set; }

        public BitmapSource Source { get; set; }
    }
}
