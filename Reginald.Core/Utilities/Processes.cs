using Reginald.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Reginald.Core.Utilities
{
    public static class Processes
    {
        private delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumDelegate lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern UInt32 GetWindowThreadProcessId(IntPtr hWnd, out Int32 lpdwProcessId);

        public static string[] GetTopLevelProcessNames(string input = null)
        {
            List<string> names = new();
            bool filter(IntPtr hWnd, int lParam)
            {
                StringBuilder sb = new(255);
                _ = GetWindowText(hWnd, sb, sb.Capacity);

                if (IsWindowVisible(hWnd) && !string.IsNullOrEmpty(sb.ToString()))
                {
                    _ = GetWindowThreadProcessId(hWnd, out int pid);
                    Process process = Process.GetProcessById(pid);
                    string name = process.MainModule.FileVersionInfo.FileDescription;
                    if (!string.IsNullOrEmpty(name))
                    {
                        names.Add(name);
                    }
                }
                return true;
            }

            _ = EnumWindows(filter, IntPtr.Zero);
            return names.Distinct()
                        .Where(name =>
                        {
                            if (!string.IsNullOrEmpty(input))
                            {
                                string pattern = $@"(?<!\w+){StringHelper.RegexClean(input)}";
                                return new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(name);
                            }
                            return true;
                        })
                        .ToArray();
        }

        public static void QuitProcess(string name)
        {
            Process[] processes = Process.GetProcesses();
            for (int i = 0; i < processes.Length; i++)
            {
                Process process = processes[i];
                if (!string.IsNullOrEmpty(process.MainWindowTitle))
                {
                    string fileDescription = process.MainModule.FileVersionInfo.FileDescription;
                    if (fileDescription == name)
                    {
                        _ = process.CloseMainWindow();
                        process.Close();
                    }
                }
            }
        }
    }
}
