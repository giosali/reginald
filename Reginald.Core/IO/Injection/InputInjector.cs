namespace Reginald.Core.IO.Injection
{
    using static Reginald.Core.IO.Injection.NativeMethods;

    public static class InputInjector
    {
        public static void InjectKeyboardInput(InjectedInputKeyboardInfo info)
        {
            _ = SendInput((uint)info.Inputs.Length, info.Inputs, INPUT.Size);
        }
    }
}
