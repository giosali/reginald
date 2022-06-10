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

                // Ensures the Alt key is always released after pressing the
                // Enter key.
                AltKeyUp();
            }
        }

        public override void AltKeyDown()
        {
            if (CanReceiveKeyboardInput)
            {
                IsAltKeyDown = true;
                _item.AltKeyDown();
                Caption = _item.TempCaption ?? _item.Caption;
                Description = _item.TempDescription ?? _item.Description;
            }
        }

        public override void AltKeyUp()
        {
            if (CanReceiveKeyboardInput)
            {
                IsAltKeyDown = false;
                _item.AltKeyUp();
                Caption = _item.TempCaption ?? _item.Caption;
                Description = _item.TempDescription ?? _item.Description;
            }
        }

        public virtual bool Predicate()
        {
            throw new System.NotImplementedException();
        }
    }
}
