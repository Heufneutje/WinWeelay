using System;

namespace WinWeelay.Core
{
    public class WinWeelayObject
    {
        private char _charValue;
        private int _intValue;
        private long _longValue;
        private string _strValue;
        private byte[] _baValue;
        private WeechatArray _arrayValue;

        public WeechatType Type { get; set; } = WeechatType.UNKNOWN;

        public WinWeelayObject()
        {
            // Does nothing
        }

        public WinWeelayObject(char c)
        {
            _charValue = c;
            Type = WeechatType.CHR;
        }

        public WinWeelayObject(int i)
        {
            _intValue = i;
            Type = WeechatType.INT;
        }

        public WinWeelayObject(long l)
        {
            _longValue = l;
            Type = WeechatType.LON;
        }

        public WinWeelayObject(string s)
        {
            _strValue = s;
            Type = WeechatType.STR;
        }

        public WinWeelayObject(byte[] b)
        {
            _baValue = b;
            Type = WeechatType.BUF;
        }

        public WinWeelayObject(WeechatArray array)
        {
            _arrayValue = array;
            Type = WeechatType.ARR;
        }

        private void CheckType(WeechatType t)
        {
            if (Type != t)
                throw new InvalidCastException("Cannot convert from " + Type + " to " + t);
        }

        public char AsChar()
        {
            CheckType(WeechatType.CHR);
            return _charValue;
        }

        public int AsInt()
        {
            CheckType(WeechatType.INT);
            return _intValue;
        }

        public long AsLong()
        {
            CheckType(WeechatType.LON);
            return _longValue;
        }

        public string AsString()
        {
            CheckType(WeechatType.STR);
            return _strValue;
        }

        public byte[] AsBytes()
        {
            CheckType(WeechatType.BUF);
            return _baValue;
        }

        public WeechatArray AsArray()
        {
            CheckType(WeechatType.ARR);
            return _arrayValue;
        }

        public string AsPointer()
        {
            CheckType(WeechatType.PTR);
            return _strValue;
        }

        public bool AsBoolean()
        {
            CheckType(WeechatType.CHR);
            return _charValue == '\u0001';
        }

        public long AsPointerLong()
        {
            try
            {
                return Convert.ToInt64((AsPointer().Substring(2), 16));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public DateTime AsTime()
        {
            CheckType(WeechatType.TIM);
            DateTime unixDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return unixDate.AddSeconds(_longValue).ToLocalTime();
        }

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
