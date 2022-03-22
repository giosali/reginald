namespace Reginald.Data.Representations
{
    public abstract class Representation : Item
    {
        public override bool IsAltKeyDown { get; set; }

        public string AltCaption { get; set; }

        public bool IsEnabled { get; set; }
    }
}
