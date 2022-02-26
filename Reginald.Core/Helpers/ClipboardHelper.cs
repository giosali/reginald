namespace Reginald.Core.Helpers
{
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;

    public static class ClipboardHelper
    {
        private const uint ClipboardExceptionCantOpen = 0x800401D0;

        public static bool TryGetText(out string text)
        {
            text = null;
            if (Clipboard.ContainsText())
            {
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        text = Clipboard.GetText();
                        return true;
                    }
                    catch (COMException ex)
                    {
                        if ((uint)ex.ErrorCode != ClipboardExceptionCantOpen)
                        {
                            throw;
                        }
                    }

                    Thread.Sleep(10);
                }
            }

            return false;
        }

        public static bool TrySetText(string text)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    Clipboard.SetText(text);
                    return true;
                }
                catch (COMException ex)
                {
                    if ((uint)ex.ErrorCode != ClipboardExceptionCantOpen)
                    {
                        throw;
                    }
                }

                Thread.Sleep(10);
            }

            return false;
        }
    }
}
