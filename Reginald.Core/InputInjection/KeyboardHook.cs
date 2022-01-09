using Reginald.Core.DataModels;
using Reginald.Core.Enums;
using Reginald.Core.Helpers;
using Reginald.Core.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Reginald.Core.InputInjection
{
    public class KeyboardHook
    {
        private const int WH_KEYBOARD_LL = 13;

        private const int WM_KEYDOWN = 0x0100;

        private static LowLevelKeyboardProc _proc;

        private static IntPtr _hookID = IntPtr.Zero;

        private SettingsDataModel Settings { get; set; }

        private List<ExpansionDataModel> Expansions { get; set; } = new();

        private StringBuilder Input { get; set; } = new();

        private KeyPressAction OnKeyPress { get; set; }

        private FileSystemWatcher ExpansionsWatcher { get; set; }

        private FileSystemWatcher SettingsWatcher { get; set; }

        private delegate ExpansionDataModel KeyPressAction(int vkCode);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

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
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

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
                        Expansions.AddRange(models);
                    }

                    string appDataApplicationDirectoryPath = Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName);
                    ExpansionsWatcher = FileSystemWatcherHelper.Initialize(appDataApplicationDirectoryPath, ApplicationPaths.ExpansionsJsonFilename, OnExpansionsChanged);
                    SettingsWatcher = FileSystemWatcherHelper.Initialize(appDataApplicationDirectoryPath, ApplicationPaths.SettingsFilename, OnSettingsChanged);
                    break;
            }
            _proc = HookCallback;
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
                    Expansions.AddRange(models);
                }
            }
        }

        public void Add()
        {
            _hookID = SetHook(_proc);
        }

        public void Remove()
        {
            _ = UnhookWindowsHookEx(_hookID);
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

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (Settings.AreExpansionsEnabled && nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
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
            if (vkCode == 8 && Input.Length > 0) // Handles the Backspace key
            {
                Input.Length--;
            }
            else
            {
                char c = GetCharFromVirtualKeyCode(vkCode);
                if (!char.IsControl(c)) // Ignores '\0'
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

                    if (!startsWithInput) // Resets input if there's no match
                    {
                        _ = Input.Clear();
                    }
                }
            }
            return null;
        }
    }
}
