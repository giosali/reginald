namespace Reginald.Data.Inputs
{
    using System;
    using Caliburn.Micro;

    public abstract class KeyboardInput : PropertyChangedBase
    {
        public event EventHandler<InputProcessingEventArgs> AltKeyPressed;

        public event EventHandler<InputProcessingEventArgs> EnterKeyPressed;

        public abstract void PressAlt(InputProcessingEventArgs e);

        public abstract void PressEnter(InputProcessingEventArgs e);

        protected virtual void OnAltKeyPressed(InputProcessingEventArgs e)
        {
            AltKeyPressed?.Invoke(this, e);
        }

        protected virtual void OnEnterKeyPressed(InputProcessingEventArgs e)
        {
            EnterKeyPressed?.Invoke(this, e);
        }
    }
}
