using System.Collections.Generic;
using System.Linq;

namespace WinWeelay.Core
{
    /// <summary>
    /// Array representation of a relay object.
    /// </summary>
    public class WeechatArray : WeechatRelayObject
    {
        private readonly List<WeechatRelayObject> _array;
        private readonly WeechatType _arrayType;

        /// <summary>
        /// Number of items in the array.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Create a new array.
        /// </summary>
        /// <param name="arrayType">Data type of the relay object.</param>
        /// <param name="size">Number of items in the array.</param>
        public WeechatArray(WeechatType arrayType, int size)
        {
            _arrayType = arrayType;
            _array = new List<WeechatRelayObject>();
            Type = WeechatType.Array;
            Length = size;
        }

        /// <summary>
        /// Add a new relay object to the array.
        /// </summary>
        /// <param name="value">The item to add.</param>
        public void Add(WeechatRelayObject value)
        {
            _array.Add(value);
        }

        /// <summary>
        /// Retrieve an object from the array at the given index.
        /// </summary>
        /// <param name="index">The index at which to find the object.</param>
        /// <returns>The retrieved relay object.</returns>
        public WeechatRelayObject this[int index] => _array[index];

        /// <summary>
        /// Convert all strings the array as a new array.
        /// </summary>
        /// <returns>A string representation of the array values.</returns>
        public string[] ToStringArray()
        {
            if (_arrayType != WeechatType.String)
                return new string[0];

            return _array.Select(x => x.ToString()).ToArray();
        }
    }
}
