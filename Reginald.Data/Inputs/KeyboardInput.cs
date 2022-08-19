namespace Reginald.Data.Inputs
{
    using System;
    using Caliburn.Micro;

    public abstract class KeyboardInput : PropertyChangedBase
    {
        public event EventHandler<InputProcessingEventArgs> AltAndEnterKeysPressed;

        public event EventHandler<InputProcessingEventArgs> AltKeyPressed;

        public event EventHandler<InputProcessingEventArgs> AltKeyReleased;

        public event EventHandler<InputProcessingEventArgs> EnterKeyPressed;

        public event EventHandler<InputProcessingEventArgs> TabKeyPressed;

        public abstract void PressAlt(InputProcessingEventArgs e);

        public abstract void PressAltAndEnter(InputProcessingEventArgs e);

        public abstract void PressEnter(InputProcessingEventArgs e);

        public abstract void PressTab(InputProcessingEventArgs e);

        public abstract void ReleaseAlt(InputProcessingEventArgs e);

        protected virtual void OnAltAndEnterKeysPressed(InputProcessingEventArgs e)
        {
            AltAndEnterKeysPressed?.Invoke(this, e);
        }

        protected virtual void OnAltKeyPressed(InputProcessingEventArgs e)
        {
            AltKeyPressed?.Invoke(this, e);
        }

        protected virtual void OnAltKeyReleased(InputProcessingEventArgs e)
        {
            AltKeyReleased?.Invoke(this, e);
        }

        protected virtual void OnEnterKeyPressed(InputProcessingEventArgs e)
        {
            EnterKeyPressed?.Invoke(this, e);
        }

        protected virtual void OnTabKeyPressed(InputProcessingEventArgs e)
        {
            TabKeyPressed?.Invoke(this, e);
        }
    }
}
