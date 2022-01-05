using Reginald.Core.DataModels;
using Reginald.Core.Enums;
using Reginald.Core.Extensions;
using Reginald.Core.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Reginald.Core.InputInjection
{
    public class KeyboardHook
    {
        private bool _shiftLastUsed;
        private bool _dollarSignUsed;
        private string _input = string.Empty;
        private int _maxTriggerLength = 0;
        private delegate ExpansionDataModel KeyPressAction(Key key);
        private readonly FileSystemWatcher expansionsWatcher;

        private KeyPressAction OnKeyPress { get; set; }

        private List<ExpansionDataModel> Expansions { get; set; } = new();

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc;
        private static IntPtr _hookID = IntPtr.Zero;

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

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule;
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Key key = KeyInterop.KeyFromVirtualKey(vkCode);
                ExpansionDataModel expansion = InterpretKey(key);
                if (expansion is not null)
                {
                    Keyboard.DelaySendString(expansion);
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private ExpansionDataModel InterpretKey(Key key)
        {
            if (key == Key.Back)
            {
                if (_input != string.Empty)
                {
                    _input = _input.Remove(_input.Length - 1);
                }
                else if (_dollarSignUsed)
                {
                    _dollarSignUsed = false;
                }
                else if (_shiftLastUsed)
                {
                    _shiftLastUsed = false;
                }
            }

            // If expansion was initiated...
            if (_dollarSignUsed)
            {
                string keyString = key.ToString().ToLower();
                if (!(keyString.Length > 2))
                {
                    // ...keep track of characters typed
                    _input += keyString.StartsWith("d") ? keyString.Substring(1) : keyString;
                    if (_input.Length > _maxTriggerLength)
                    {
                        _input = string.Empty;
                        _dollarSignUsed = _shiftLastUsed = false;
                    }
                    else
                    {
                        for (int i = 0; i < Expansions.Count; i++)
                        {
                            ExpansionDataModel expansion = Expansions[i];
                            if (expansion.Trigger.ToLower() == _input)
                            {
                                _input = string.Empty;
                                _dollarSignUsed = _shiftLastUsed = false;
                                return expansion;
                            }
                        }
                    }
                }
                else
                {
                    _input = string.Empty;
                    _dollarSignUsed = _shiftLastUsed = false;
                }
            }
            else
            {
                // Reset characters being typed
                _input = string.Empty;

                // If the shift key was last pressed...
                if (_shiftLastUsed)
                {
                    // Check if an expansion was initiated
                    _dollarSignUsed = key is Key.D4;
                }

                // Indicate whether or not the shift key was pressed to initiate the expansion key
                _shiftLastUsed = key is Key.LeftShift or Key.RightShift;
            }
            return null;
        }

        private void OnExpansionsChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                IEnumerable<ExpansionDataModel> models = FileOperations.GetGenericData<ExpansionDataModel>(ApplicationPaths.ExpansionsJsonFilename, false);
                if (models is not null)
                {
                    Expansions.AddRange(models);
                    _maxTriggerLength = Expansions.LongestTriggerLength();
                }
            }
        }

        public KeyboardHook(Hook hook)
        {
            switch (hook)
            {
                case Hook.Expansion:
                    OnKeyPress = InterpretKey;
                    IEnumerable<ExpansionDataModel> models = FileOperations.GetGenericData<ExpansionDataModel>(ApplicationPaths.ExpansionsJsonFilename, false);
                    if (models is not null)
                    {
                        Expansions.AddRange(models);
                        _maxTriggerLength = Expansions.LongestTriggerLength();
                    }

                    string path = Path.Combine(ApplicationPaths.AppDataDirectoryPath, ApplicationPaths.ApplicationName);
                    string filter = ApplicationPaths.ExpansionsJsonFilename;
                    expansionsWatcher = new(path, filter);
                    expansionsWatcher.NotifyFilter = NotifyFilters.Attributes
                                                    | NotifyFilters.CreationTime
                                                    | NotifyFilters.LastAccess
                                                    | NotifyFilters.LastWrite
                                                    | NotifyFilters.Size;
                    expansionsWatcher.Changed += OnExpansionsChanged;
                    expansionsWatcher.EnableRaisingEvents = true;
                    break;

                default:
                    break;
            }
            _proc = HookCallback;
        }

        public void Add()
        {
            _hookID = SetHook(_proc);
        }

        public void Remove()
        {
            _ = UnhookWindowsHookEx(_hookID);
        }
    }
}
