namespace Reginald.Data.DisplayItems
{
    public abstract class DisplayItem : Item
    {
        private readonly Item _item;

        public DisplayItem()
        {
        }

        public DisplayItem(Item item)
        {
            _item = item;
            Guid = item.Guid;
            Name = item.Name;
            Icon = item.Icon;
            Caption = item.Caption;
            Description = item.Description;
            Id = item.Id;
            RequiresPrompt = item.RequiresPrompt;
            CanReceiveKeyboardInput = item.CanReceiveKeyboardInput;
            LosesFocus = item.LosesFocus;
        }

        public override bool IsAltKeyDown { get; set; }

        public override void EnterKeyDown()
        {
            if (CanReceiveKeyboardInput)
            {
                _item.EnterKeyDown();
            }
        }

        public override void AltKeyDown()
        {
            if (CanReceiveKeyboardInput)
            {
                IsAltKeyDown = true;
                _item.AltKeyDown();
                Caption = _item.TempCaption;
                Description = _item.TempDescription;
            }
        }

        public override void AltKeyUp()
        {
            if (CanReceiveKeyboardInput)
            {
                IsAltKeyDown = false;
                _item.AltKeyUp();
                Caption = _item.TempCaption;
                Description = _item.TempDescription;
            }
        }

        public virtual bool Predicate()
        {
            throw new System.NotImplementedException();
        }
    }
}
