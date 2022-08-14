namespace Reginald.Services
{
    /// <summary>
    /// Controls how the window is to be shown.
    /// </summary>
    internal enum ShowWindowCommand : int
    {
        /// <summary>
        /// Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
        /// </summary>
        SW_SHOWNORMAL = 1,
    }
}
