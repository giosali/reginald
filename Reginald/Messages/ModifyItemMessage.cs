namespace Reginald.Messages
{
    using System;

    /// <summary>
    /// The modification to perform on the item.
    /// </summary>
    public enum ModificationType
    {
        /// <summary>
        /// Don't modify.
        /// </summary>
        None,

        /// <summary>
        /// Modify the item by toggling its IsEnabled property.
        /// </summary>
        ToggleIsEnabled,

        /// <summary>
        /// Modify the item by editing it.
        /// </summary>
        Edit,

        /// <summary>
        /// Modify the item by deleting it.
        /// </summary>
        Delete,
    }

    public sealed class ModifyItemMessage
    {
        public ModifyItemMessage(string guid, ModificationType modificationType)
        {
            Guid = Guid.Parse(guid);
            ModificationType = modificationType;
        }

        public Guid Guid { get; set; }

        public ModificationType ModificationType { get; set; }
    }
}
