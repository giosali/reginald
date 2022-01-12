using Caliburn.Micro;
using System;
using System.Threading.Tasks;

namespace Reginald.Core.AbstractProducts
{
    public abstract class InteractiveAbstractProductBase : PropertyChangedBase
    {
        public abstract void EnterDown(bool isAltDown, Action action);

        public abstract Task<bool> EnterDownAsync(bool isAltDown, Action action, object o);

        /// <summary>
        /// Invoked when the Alt key is pressed.
        /// </summary>
        /// <returns>Returns a 2-tuple containing a description and a caption, respectively.</returns>
        public abstract (string, string) AltDown();

        /// <summary>
        /// Invoked when the Alt key is released.
        /// </summary>
        /// <returns>Returns a 2-tuple containing a description and a caption, respectively.</returns>
        public abstract (string, string) AltUp();
    }
}
