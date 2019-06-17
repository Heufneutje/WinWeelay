using System.Collections.Generic;
using System.Linq;

namespace WinWeelay.Core
{
    public class WeechatArray : WeechatRelayObject
    {
        private readonly List<WeechatRelayObject> _array;
        private readonly WeechatType _arrayType;

        public int Length { get; set; }

        public WeechatArray(WeechatType arrayType, int size)
        {
            _arrayType = arrayType;
            _array = new List<WeechatRelayObject>();

            Length = size;
        }

        public void Add(WeechatRelayObject value)
        {
            _array.Add(value);
        }

        public WeechatRelayObject this[int index]
        {
            get
            {
                return _array[index];
            }
        }

        public string[] ToStringArray()
        {
            if (_arrayType != WeechatType.STR)
                return new string[0];

            string[] ret = new string[Length];
            for (int i = 0; i < Length; i++)
                ret[i] = _array.ElementAt(i).AsString();

            return ret;
        }

        public override string ToString()
        {
            return string.Join(",", _array);
        }
    }
}
