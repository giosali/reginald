namespace Reginald.Services.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using Reginald.Core.Extensions;
    using static Reginald.Services.Utilities.NativeMethods;

    public static class ProcessUtility
    {
        public static IEnumerable<Process> GetTopLevelProcesses(string input = null)
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
                                    string pattern = $@"^{input.RegexClean()}";
                                    return new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(process.MainModule.FileVersionInfo.FileDescription);
                                }

                                return true;
                            });
        }

        public static void QuitProcessById(int processId)
        {
            try
            {
                Process process = Process.GetProcessById(processId);
                process.CloseMainWindow();
                process.Close();
            }
            catch (ArgumentException)
            {
            }
        }

        public static void RestartApplication()
        {
            string filename = Environment.ProcessPath;
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
    }
}
