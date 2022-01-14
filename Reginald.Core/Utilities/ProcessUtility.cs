namespace Reginald.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using Reginald.Extensions;

    public static class ProcessUtility
    {
        private delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        public static string[] GetTopLevelProcessNames(string input = null)
        {
            List<string> names = new();
            bool Filter(IntPtr hWnd, int lParam)
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

            _ = EnumWindows(Filter, IntPtr.Zero);
            return names.Distinct()
                        .Where(name =>
                        {
                            if (!string.IsNullOrEmpty(input))
                            {
                                string pattern = $@"(?<!\w+){input.RegexClean()}";
                                return new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(name);
                            }

                            return true;
                        })
                        .ToArray();
        }

        public static Process[] GetTopLevelProcesses(string input = null)
        {
            List<Process> processes = new();
            bool Filter(IntPtr hWnd, int lParam)
            {
                StringBuilder sb = new(255);
                _ = GetWindowText(hWnd, sb, sb.Capacity);

                if (IsWindowVisible(hWnd) && !string.IsNullOrEmpty(sb.ToString()))
                {
                    _ = GetWindowThreadProcessId(hWnd, out int pid);
                    Process process = Process.GetProcessById(pid);
                    if (!string.IsNullOrEmpty(process.MainModule.FileVersionInfo.FileDescription))
                    {
                        processes.Add(process);
                    }
                }

                return true;
            }

            _ = EnumWindows(Filter, IntPtr.Zero);
            return processes.Distinct()
                            .Where(process =>
                            {
                                if (!string.IsNullOrEmpty(input))
                                {
                                    string pattern = $@"(?<!\w+){input.RegexClean()}";
                                    return new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(process.MainModule.FileVersionInfo.FileDescription);
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

        public static void RestartApplication()
        {
            string filename = Process.GetCurrentProcess().MainModule.FileName;
            ProcessStartInfo startInfo = new()
            {
                Arguments = $"/C ping 127.0.0.1 -n 2 && \"{filename}\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe",
            };

            _ = Process.Start(startInfo);
            Application.Current.Shutdown();
        }

        public static void GoTo(string uri)
        {
            try
            {
                ProcessStartInfo startInfo = new()
                {
                    FileName = uri,
                    UseShellExecute = true,
                };
                _ = Process.Start(startInfo);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                if (ex.ErrorCode == -2147467259)
                {
                    _ = MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message);
            }
        }

        public static void OpenFromPath(string path)
        {
            _ = Process.Start("explorer.exe", path);
        }

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumDelegate lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
    }
}
