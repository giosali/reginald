namespace Reginald.Data.Inputs
{
    using System;

    public class InputProcessingEventArgs : EventArgs
    {
        public string Caption { get; set; }

        public string CompleteInput { get; set; }

        public string Description { get; set; }

        public bool Handled { get; set; }

        public int HashCode { get; set; }

        public string Icon { get; set; }

        public bool IsInputIncomplete { get; set; }

        public bool Remove { get; set; }
    }
}
