namespace Reginald.Data.Inputs
{
    using System;

    public abstract class KeyboardInput
    {
        public event EventHandler<InputProcessingEventArgs> EnterKeyPressed;

        public abstract void PressEnter(InputProcessingEventArgs e);

        protected virtual void OnEnterKeyPressed(InputProcessingEventArgs e)
        {
            EnterKeyPressed?.Invoke(this, e);
        }
    }
}