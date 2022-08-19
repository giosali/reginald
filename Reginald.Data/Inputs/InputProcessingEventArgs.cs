namespace Reginald.Data.Inputs
{
    using System;

    public class InputProcessingEventArgs : EventArgs
    {
        public string CompleteInput { get; set; }

        public string Description { get; set; }

        public bool Handled { get; set; }

        public bool IsAltKeyDown { get; set; }

        public bool IsInputIncomplete { get; set; }
    }
}
