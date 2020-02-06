using System;

namespace WinWeelay.Core
{
    /// <summary>
    /// Generic relay object, base class.
    /// </summary>
    public class WeechatRelayObject
    {
        private char _charValue;
        private int _intValue;
        private long _longValue;
        private string _strValue;
        private byte[] _baValue;
        private WeechatArray _arrayValue;

        /// <summary>
        /// Type of the relay object.
        /// </summary>
        public WeechatType Type { get; set; } = WeechatType.UNKNOWN;

        /// <summary>
        /// Empty constructor for inheritance.
        /// </summary>
        protected WeechatRelayObject() { }

        /// <summary>
        /// Create a new relay object of the character type.
        /// </summary>
        /// <param name="c">Character to wrap in a relay object.</param>
        public WeechatRelayObject(char c)
        {
            _charValue = c;
            Type = WeechatType.CHR;
        }

        /// <summary>
        /// Create a new relay object of the 32-bit integer type.
        /// </summary>
        /// <param name="i">32-bit integer to wrap in a relay object.</param>
        public WeechatRelayObject(int i)
        {
            _intValue = i;
            Type = WeechatType.INT;
        }

        /// <summary>
        /// Create a new relay object of the 64-bit integer type.
        /// </summary>
        /// <param name="l">64-bit integer to wrap in a relay object.</param>
        public WeechatRelayObject(long l)
        {
            _longValue = l;
            Type = WeechatType.LON;
        }

        /// <summary>
        /// Create a new relay object of the string type.
        /// </summary>
        /// <param name="s">String to wrap in a relay object.</param>
        public WeechatRelayObject(string s)
        {
            _strValue = s;
            Type = WeechatType.STR;
        }

        /// <summary>
        /// Create a new relay object of the buffer type.
        /// </summary>
        /// <param name="b">Byte array to wrap in a relay object.</param>
        public WeechatRelayObject(byte[] b)
        {
            _baValue = b;
            Type = WeechatType.BUF;
        }

        /// <summary>
        /// Create a new relay object of the array type.
        /// </summary>
        /// <param name="array">Array to wrap in a relay object.</param>
        public WeechatRelayObject(WeechatArray array)
        {
            _arrayValue = array;
            Type = WeechatType.ARR;
        }

        /// <summary>
        /// Validate that the object is of a given type.
        /// </summary>
        /// <param name="t">A given type.</param>
        private void CheckType(WeechatType t)
        {
            if (Type != t)
                throw new InvalidCastException("Cannot convert from " + Type + " to " + t);
        }

        /// <summary>
        /// Converts the current object to a character.
        /// </summary>
        /// <returns>A character.</returns>
        public char AsChar()
        {
            CheckType(WeechatType.CHR);
            return _charValue;
        }

        /// <summary>
        /// Converts the current object to a 32-bit integer.
        /// </summary>
        /// <returns>A 32-bit integer.</returns>
        public int AsInt()
        {
            CheckType(WeechatType.INT);
            return _intValue;
        }

        /// <summary>
        /// Converts the current object to a 64-bit integer.
        /// </summary>
        /// <returns>A 64-bit integer.</returns>
        public long AsLong()
        {
            CheckType(WeechatType.LON);
            return _longValue;
        }

        /// <summary>
        /// Converts the current object to a string.
        /// </summary>
        /// <returns>A string.</returns>
        public string AsString()
        {
            CheckType(WeechatType.STR);
            return _strValue;
        }

        /// <summary>
        /// Converts the current object to a byte array.
        /// </summary>
        /// <returns>A byte array.</returns>
        public byte[] AsBytes()
        {
            CheckType(WeechatType.BUF);
            return _baValue;
        }

        /// <summary>
        /// Converts the current object to a array.
        /// </summary>
        /// <returns>An array.</returns>
        public WeechatArray AsArray()
        {
            CheckType(WeechatType.ARR);
            return _arrayValue;
        }

        /// <summary>
        /// Converts the current object to a pointer.
        /// </summary>
        /// <returns>A pointer.</returns>
        public string AsPointer()
        {
            CheckType(WeechatType.PTR);
            return _strValue;
        }

        /// <summary>
        /// Converts the current object to a boolean.
        /// </summary>
        /// <returns>A boolean.</returns>
        public bool AsBoolean()
        {
            try
            {
                CheckType(WeechatType.CHR);
                return _charValue == '\u0001';
            }
            catch (InvalidCastException)
            {
                CheckType(WeechatType.INT);
                return _intValue == 1;
            }
        }

        /// <summary>
        /// Converts the current object to a time.
        /// </summary>
        /// <returns>A time.</returns>
        public DateTime AsTime()
        {
            CheckType(WeechatType.TIM);
            DateTime unixDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return unixDate.AddSeconds(_longValue).ToLocalTime();
        }

        /// <summary>
        /// Override for debug purposes.
        /// </summary>
        /// <returns>The string representation of the current value.</returns>
        public override string ToString()
        {
            string value = "Unknown";
            switch (Type)
            {
                case WeechatType.CHR:
                    value = "" + AsChar();
                    break;
                case WeechatType.INT:
                    value = "" + AsInt();
                    break;
                case WeechatType.LON:
                    value = "" + AsLong();
                    break;
                case WeechatType.STR:
                    value = AsString();
                    break;
                case WeechatType.TIM:
                    value = "" + AsTime();
                    break;
                case WeechatType.PTR:
                    value = "" + AsPointer();
                    break;
                case WeechatType.BUF:
                    byte[] bytes = AsBytes();
                    if (bytes == null)
                        value = "";
                    else
                        value = $"[{string.Join(",", AsBytes())}]";
                    break;
                case WeechatType.ARR:
                    value = "" + AsArray();
                    break;
            }

            return $"{value}";
        }
    }
}
