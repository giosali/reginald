namespace Reginald.Core.IO.Hooks
{
    using System;
    using System.Runtime.InteropServices;

    public abstract class Hook
    {
        protected IntPtr HookId { get; set; }

        /// <summary>
        /// Gets or sets a pinned object that prevents the LowLevelKeyboardProc from being collected to prevent ExecutionEngineException from being thrown.
        /// </summary>
        protected GCHandle ProcHandle { get; set; }

        public abstract void Add();

        public abstract void Remove();

        protected abstract IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam);
    }
}
