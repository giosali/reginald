namespace Reginald.Data.Inputs
{
    using System;

    public abstract class KeyboardInput
    {
        public event EventHandler<EventArgs> EnterKeyPressed;

        public abstract void PressEnter();

        protected virtual void OnEnterKeyPressed(EventArgs e)
        {
            EnterKeyPressed?.Invoke(this, e);
        }
    }
}