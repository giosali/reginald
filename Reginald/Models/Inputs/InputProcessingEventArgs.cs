namespace Reginald.Models.Inputs
{
    using System;

    internal class InputProcessingEventArgs : EventArgs
    {
        public string CompleteInput { get; set; }

        public bool Handled { get; set; }

        public bool IsInputIncomplete { get; set; }

        public bool Remove { get; set; }
    }
}
