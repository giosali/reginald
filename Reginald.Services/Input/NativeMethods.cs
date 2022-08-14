namespace Reginald.Services.Input
{
    using System;
    using System.Runtime.InteropServices;

    public static class NativeMethods
    {
        /// <summary>
        /// Specifies various aspects of mouse motion and button clicking.
        /// </summary>
        [Flags]
        internal enum MOUSEEVENTF : uint
        {
            /// <summary>
            /// Movement occurred.
            /// </summary>
            MOVE = 0x0001,

            /// <summary>
            /// The left button is down.
            /// </summary>
            LEFTDOWN = 0x0002,

            /// <summary>
            /// The left button is up.
            /// </summary>
            LEFTUP = 0x0004,

            /// <summary>
            /// The right button is down.
            /// </summary>
            RIGHTDOWN = 0x0008,

            /// <summary>
            /// The right button is up.
            /// </summary>
            RIGHTUP = 0x0010,

            /// <summary>
            /// The middle button is down.
            /// </summary>
            MIDDLEDOWN = 0x0020,

            /// <summary>
            /// The middle button is up.
            /// </summary>
            MIDDLEUP = 0x0040,

            /// <summary>
            /// An X button was pressed.
            /// </summary>
            XDOWN = 0x0080,

            /// <summary>
            /// An X button was released.
            /// </summary>
            XUP = 0x0100,

            /// <summary>
            /// The wheel button is rotated.
            /// </summary>
            WHEEL = 0x0800,

            /// <summary>
            /// The wheel button is tilted.
            /// </summary>
            HWHEEL = 0x01000,

            /// <summary>
            /// Move (do not coalesce move messages). The application processes all mouse events since the previously processed mouse event.
            /// </summary>
            MOVE_NOCOALESCE = 0x2000,

            /// <summary>
            /// Map coordinates to the entire virtual desktop.
            /// </summary>
            VIRTUALDESK = 0x4000,

            /// <summary>
            /// The dx and dy parameters contain normalized absolute coordinates. If not set, those parameters contain relative data: the change in position since the last reported position. This flag can be set, or not set, regardless of what kind of mouse or mouse-like device, if any, is connected to the system. For further information about relative mouse motion, see the following Remarks section.
            /// </summary>
            ABSOLUTE = 0x8000,
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "PostMessageA")]
        internal static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, nuint wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            internal uint type;
            internal InputUnion U;

            internal static int Size => Marshal.SizeOf(typeof(INPUT));
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT
        {
            internal int uMsg;
            internal short wParamL;
            internal short wParamH;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBDINPUT
        {
            internal VirtualKeyShort wVk;
            internal short wScan;
            internal KEYEVENTF dwFlags;
            internal int time;
            internal UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            internal int dx;
            internal int dy;
            internal int mouseData;
            internal MOUSEEVENTF dwFlags;
            internal uint time;
            internal UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct InputUnion
        {
            [FieldOffset(0)]
            internal MOUSEINPUT mi;
            [FieldOffset(0)]
            internal KEYBDINPUT ki;
            [FieldOffset(0)]
            internal HARDWAREINPUT hi;
        }
    }
}
