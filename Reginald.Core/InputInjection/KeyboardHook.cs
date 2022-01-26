namespace Reginald.Core.InputInjection
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using Reginald.Core.DataModels;
    using Reginald.Core.Helpers;
    using Reginald.Core.IO;
    using Reginald.Core.NativeMethods;

    /// <summary>
    /// Specifies different usages for keyboard hooks.
    /// </summary>
    public enum Hook
    {
        /// <summary>
        /// The keyboard hook for text expansions.
        /// </summary>
        Expansion,
    }

    public class KeyboardHook
    {
        private static LowLevelKeyboardProc _proc;

        private static IntPtr _hookID = IntPtr.Zero;

        public KeyboardHook(Hook hook)
        {
            switch (hook)
            {
                case Hook.Expansion:
                    OnKeyPress = InterpretKey;

                    Settings = FileOperations.GetSettingsData(ApplicationPaths.SettingsFilename);
                    IEnumerable<ExpansionDataModel> models = FileOperations.GetGenericData<ExpansionDataModel>(ApplicationPaths.ExpansionsJsonFilename, false);
                    if (models is not null)
                    {
                        foreach (ExpansionDataModel model in models)
                        {
                            model.Replacement = model.Replacement.Replace("\\n", "\n");
                        }

                        Expansions.AddRange(models);
                    }

                    string appDataApplicationDirectoryPath = Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName);
                    ExpansionsWatcher = FileSystemWatcherHelper.Initialize(appDataApplicationDirectoryPath, ApplicationPaths.ExpansionsJsonFilename, OnExpansionsChanged);
                    SettingsWatcher = FileSystemWatcherHelper.Initialize(appDataApplicationDirectoryPath, ApplicationPaths.SettingsFilename, OnSettingsChanged);
                    break;
            }

            _proc = HookCallback;
        }

        private delegate ExpansionDataModel KeyPressAction(int vkCode);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Source and for more information, see <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-mapvirtualkeya">Microsoft Documentation on virtual keys</see>.
        /// </summary>
        public enum MapVirtualKeyMapType : uint
        {
            /// <summary>
            /// The uCode parameter is a virtual-key code and is translated into a scan code. If it is a virtual-key code that does not distinguish between left- and right-hand keys, the left-hand scan code is returned. If there is no translation, the function returns 0.
            /// </summary>
            MAPVK_VK_TO_VSC = 0x0,

            /// <summary>
            /// The uCode parameter is a scan code and is translated into a virtual-key code that does not distinguish between left- and right-hand keys. If there is no translation, the function returns 0.
            /// </summary>
            MAPVK_VSC_TO_VK = 0x1,

            /// <summary>
            /// The uCode parameter is a virtual-key code and is translated into an unshifted character value in the low order word of the return value. Dead keys (diacritics) are indicated by setting the top bit of the return value. If there is no translation, the function returns 0.
            /// </summary>
            MAPVK_VK_TO_CHAR = 0x2,

            /// <summary>
            /// The uCode parameter is a scan code and is translated into a virtual-key code that distinguishes between left- and right-hand keys. If there is no translation, the function returns 0.
            /// </summary>
            MAPVK_VSC_TO_VK_EX = 0x3,

            /// <summary>
            /// Windows Vista and later: The uCode parameter is a virtual-key code and is translated into a scan code. If it is a virtual-key code that does not distinguish between left- and right-hand keys, the left-hand scan code is returned. If the scan code is an extended scan code, the high byte of the uCode value can contain either 0xe0 or 0xe1 to specify the extended scan code. If there is no translation, the function returns 0.
            /// </summary>
            MAPVK_VK_TO_VSC_EX = 0x04,
        }

        private SettingsDataModel Settings { get; set; }

        private List<ExpansionDataModel> Expansions { get; set; } = new();

        private StringBuilder Input { get; set; } = new();

        private KeyPressAction OnKeyPress { get; set; }

        private FileSystemWatcher ExpansionsWatcher { get; set; }

        private FileSystemWatcher SettingsWatcher { get; set; }

        public void Add()
        {
            _hookID = SetHook(_proc);
        }

        public void Remove()
        {
            _ = UnhookWindowsHookEx(_hookID);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, MapVirtualKeyMapType uMapType);

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtKey);

        [DllImport("user32.dll")]
        private static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        private static extern int ToUnicode(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)] StringBuilder pwszBuff, int cchBuff, uint wFlags);

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule;
            return SetWindowsHookEx(KeyboardHookNativeMethods.WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        private static char GetCharFromVirtualKeyCode(int vkCode)
        {
            // GetKeyState is necessary for detecting key combinations (Shift + 4)
            // or whether the CAPS LOCK key is toggled
            _ = GetKeyState(vkCode);
            byte[] keyboardState = new byte[256];
            _ = GetKeyboardState(keyboardState);

            uint scanCode = MapVirtualKey((uint)vkCode, MapVirtualKeyMapType.MAPVK_VK_TO_VSC);
            StringBuilder sb = new(2);
            int result = ToUnicode((uint)vkCode, scanCode, keyboardState, sb, sb.Capacity, 0);
            return result == 1 && sb.Length > 0 ? sb[0] : '\0';
        }

        private void OnSettingsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                Settings = FileOperations.GetSettingsData(ApplicationPaths.SettingsFilename);
            }
        }

        private void OnExpansionsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                IEnumerable<ExpansionDataModel> models = FileOperations.GetGenericData<ExpansionDataModel>(ApplicationPaths.ExpansionsJsonFilename, false);
                if (models is not null)
                {
                    foreach (ExpansionDataModel model in models)
                    {
                        model.Replacement = model.Replacement.Replace("\\n", "\n");
                    }

                    Expansions.Clear();
                    Expansions.AddRange(models);
                }
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (Settings.AreExpansionsEnabled && nCode >= 0 && wParam == (IntPtr)KeyboardHookNativeMethods.WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                ExpansionDataModel expansion = InterpretKey(vkCode);
                if (expansion is not null)
                {
                    Keyboard.DelaySendString(expansion);
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private ExpansionDataModel InterpretKey(int vkCode)
        {
            // Handles the Backspace key
            if (vkCode == 8 && Input.Length > 0)
            {
                Input.Length--;
            }
            else
            {
                char c = GetCharFromVirtualKeyCode(vkCode);

                // Ignores '\0'
                if (!char.IsControl(c))
                {
                    _ = Input.Append(c);

                    // This helps keep track of whether the input currently
                    // entered is close to fully matching a trigger
                    bool startsWithInput = false;
                    string input = Input.ToString();

                    // Gives the user some leeway when typing, specifically when
                    // the user mistypes the final character of a trigger
                    string shortenedInput = input.Length > 1 ? input[0..^1] : input;
                    for (int i = 0; i < Expansions.Count; i++)
                    {
                        string trigger = Expansions[i].Trigger;
                        if (trigger == input)
                        {
                            _ = Input.Clear();
                            return Expansions[i];
                        }
                        else if (!startsWithInput)
                        {
                            startsWithInput = trigger.StartsWith(input) || trigger.StartsWith(shortenedInput);
                        }
                    }

                    // Resets input if there's no match
                    if (!startsWithInput)
                    {
                        _ = Input.Clear();
                    }
                }
            }

            return null;
        }
    }
}
