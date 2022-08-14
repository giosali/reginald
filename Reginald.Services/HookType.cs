namespace Reginald.Services
{
    /// <summary>
    /// Enumerates the valid hook types passed as the idHook parameter into a call to SetWindowsHookEx.
    /// </summary>
    internal enum HookType : int
    {
        /// <summary>
        /// Installs a hook procedure that monitors low-level keyboard input events. For more information, see the <see href="https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644985(v=vs.85)">LowLevelKeyboardProc hook procedure</see>.
        /// </summary>
        WH_KEYBOARD_LL = 13,

        /// <summary>
        /// Installs a hook procedure that monitors low-level mouse input events. For more information, see the <see href="https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644985(v=vs.85)">LowLevelKeyboardProc hook procedure</see>.
        /// </summary>
        WH_MOUSE_LL = 14
    }
}
