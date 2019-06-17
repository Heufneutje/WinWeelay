using System.Collections.Generic;
using System.Linq;

namespace WinWeeRelay.Core
{
    public class WeechatArray : WinWeeRelayObject
    {
        private readonly List<WinWeeRelayObject> _array;
        private readonly WeechatType _arrayType;

        public int Length { get; set; }

        public WeechatArray(WeechatType arrayType, int size)
        {
            _arrayType = arrayType;
            _array = new List<WinWeeRelayObject>();

            Length = size;
        }

        public void Add(WinWeeRelayObject value)
        {
            _array.Add(value);
        }

        public WinWeeRelayObject this[int index]
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
