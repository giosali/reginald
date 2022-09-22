namespace Reginald.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Windows;
    using static Reginald.Core.Services.NativeMethods;

    public static class ProcessService
    {
        private static readonly HashSet<string> _systemWindows = new() { "Progman", "Shell_TrayWnd", "CabinetWClass", "ApplicationFrameWindow" };

        public static List<Process> GetTopLevelProcesses()
        {
            List<Process> processes = new();
            _ = EnumWindows(
                (hWnd, lParam) =>
                {
                    if (!IsWindowVisible(hWnd))
                    {
                        return true;
                    }

                    // Eliminates cloaked UWP applications.
                    if (DwmGetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.DWMWA_CLOAKED, out bool isCloaked, sizeof(int)) == 0 && isCloaked)
                    {
                        return true;
                    }

                    if (GetWindowTextLength(hWnd) == 0)
                    {
                        return true;
                    }

                    StringBuilder className = new(256);
                    if (GetClassName(hWnd, className, className.Capacity) == 0 || _systemWindows.Contains(className.ToString()))
                    {
                        return true;
                    }

                    _ = GetWindowThreadProcessId(hWnd, out int pid);
                    processes.Add(Process.GetProcessById(pid));
                    return true;
                },
                IntPtr.Zero);
            return processes;
        }

        public static void GoTo(string uri)
        {
            // 1 = SW_SHOWNORMAL
            _ = ShellExecute(IntPtr.Zero, null, uri, null, null, 1);
        }

        public static void OpenFromPath(string path)
        {
            _ = Process.Start("explorer.exe", path);
        }

        public static void QuitProcessById(int processId)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(Process.GetProcessById(processId).ProcessName);
                for (int i = 0; i < processes.Length; i++)
                {
                    Process process = processes[i];
                    if (process.CloseMainWindow())
                    {
                        process.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is not AggregateException && ex is not SystemException)
                {
                    throw;
                }
            }
        }

        public static void ForceQuitProcessById(int processId)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(Process.GetProcessById(processId).ProcessName);
                for (int i = 0; i < processes.Length; i++)
                {
                    Process process = processes[i];
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                if (ex is not AggregateException && ex is not SystemException)
                {
                    throw;
                }
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
    }
}
