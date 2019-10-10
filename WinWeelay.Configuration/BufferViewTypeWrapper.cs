using System.Collections.Generic;

namespace WinWeelay.Configuration
{
    public class BufferViewTypeWrapper
    {
        public BufferViewType BufferViewType { get; private set; }
        public string Description { get; private set; }

        public BufferViewTypeWrapper(BufferViewType bufferViewType, string description)
        {
            BufferViewType = bufferViewType;
            Description = description;
        }

        public static IEnumerable<BufferViewTypeWrapper> GetTypes()
        {
            List<BufferViewTypeWrapper> types = new List<BufferViewTypeWrapper>();
            types.Add(new BufferViewTypeWrapper(BufferViewType.List, "List view"));
            types.Add(new BufferViewTypeWrapper(BufferViewType.Tree, "Tree view"));
            return types;
        }
    }
}
