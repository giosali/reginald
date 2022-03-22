namespace Reginald.Data
{
    public interface IKeyboardInputProperty
    {
        bool CanReceiveKeyboardInput { get; set; }

        bool IsAltKeyDown { get; set; }

        void EnterKeyDown();

        void AltKeyDown();

        void AltKeyUp();
    }
}
