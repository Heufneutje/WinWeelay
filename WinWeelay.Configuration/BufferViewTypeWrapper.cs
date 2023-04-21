using System.Collections.Generic;

namespace WinWeelay.Configuration
{
    /// <summary>
    /// Wrapper to display buffer view types in a combo box.
    /// </summary>
    public class BufferViewTypeWrapper
    {
        /// <summary>
        /// Type of buffer selection in the UI.
        /// </summary>
        public BufferViewType BufferViewType { get; private set; }

        /// <summary>
        /// Display text for the combo box.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Create a new wrapper for a buffer type.
        /// </summary>
        /// <param name="bufferViewType">The buffer type.</param>
        /// <param name="description">Display text for the combo box.</param>
        public BufferViewTypeWrapper(BufferViewType bufferViewType, string description)
        {
            BufferViewType = bufferViewType;
            Description = description;
        }

        /// <summary>
        /// Retrieve a user-friendly list of all buffer types.
        /// </summary>
        /// <returns>A list of all buffer types.</returns>
        public static IEnumerable<BufferViewTypeWrapper> GetTypes()
        {
            List<BufferViewTypeWrapper> types = new();
            types.Add(new BufferViewTypeWrapper(BufferViewType.List, "List view"));
            types.Add(new BufferViewTypeWrapper(BufferViewType.Tree, "Tree view"));
            return types;
        }
    }
}
