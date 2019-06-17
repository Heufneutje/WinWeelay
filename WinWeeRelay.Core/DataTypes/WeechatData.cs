using System;
using System.Collections.Generic;
using System.Text;
using WinWeeRelay.Utils;

namespace WinWeeRelay.Core
{
    public class WeechatData
    {
        private readonly byte[] _data;
        private int _pointer;

        public bool IsEmpty
        {
            get
            {
                return _pointer == _data.Length;
            }
        }

        public WeechatData(byte[] data)
        {
            _data = data;
            _pointer = 0;
        }

        public int GetUnsignedInt()
        {
            if (_pointer + 4 > _data.Length)
                throw new IndexOutOfRangeException("Not enough data to compute length");

            int ret = ((_data[_pointer + 0] & 0xFF) << 24) | ((_data[_pointer + 1] & 0xFF) << 16)
                     | ((_data[_pointer + 2] & 0xFF) << 8) | ((_data[_pointer + 3] & 0xFF));

            _pointer += 4;
            return ret;
        }

        public int GetByte()
        {
            int ret = _data[_pointer] & 0xFF;

            _pointer++;
            return ret;
        }

        public char GetChar()
        {
            return (char)GetByte();
        }

        public long GetLong()
        {
            int length = GetByte();
            if (_pointer + length > _data.Length)
                throw new IndexOutOfRangeException("Not enough data");

            if (length == 0)
                //throw new InvalidOperationException("Length must not be zero");
                return 0;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
                sb.Append(GetChar());

            return Convert.ToInt64(sb.ToString());
        }

        public string GetString()
        {
            int length = GetUnsignedInt();
            if (_pointer + length > _data.Length)
                throw new IndexOutOfRangeException("Not enough data");

            if (length == 0)
                return "";

            if (length == -1)
                return null;

            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
                bytes[i] = (byte)GetByte();

            return Encoding.UTF8.GetString(bytes);
        }

        public byte[] GetBuffer()
        {
            int length = GetUnsignedInt();
            if (_pointer + length > _data.Length)
                throw new IndexOutOfRangeException("Not enough data");

            if (length == 0)
                return new byte[0];

            if (length == -1)
                return null;

            byte[] ret = ArrayHelper.CopyOfRange(_data, _pointer, _pointer + length);

            _pointer += length;
            return ret;
        }

        public string GetPointer()
        {
            int length = GetByte();
            if (_pointer + length > _data.Length)
                throw new IndexOutOfRangeException("Not enough data");

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
                sb.Append(GetChar());

            if (length == 1 && Convert.ToInt64(sb.ToString().ToUpper(), 16) == 0)
            {
                // Null Pointer
                return "0x0";
            }

            return "0x" + sb.ToString();
        }

        public long GetTime()
        {
            long time = GetLong();
            return time;
        }

        public WeechatHashtable GetHashtable()
        {
            WeechatType keyType = GetWeechatType();
            WeechatType valueType = GetWeechatType();
            int count = GetUnsignedInt();

            WeechatHashtable hta = new WeechatHashtable();
            for (int i = 0; i < count; i++)
            {
                WinWeeRelayObject k = GetObject(keyType);
                WinWeeRelayObject v = GetObject(valueType);
                hta.Add(k, v);
            }

            return hta;
        }

        public WeechatHdata GetHdata()
        {
            WeechatHdata whd = new WeechatHdata();

            string hpath = GetString();
            string keys = GetString();
            int count = GetUnsignedInt();

            if (count == 0)
                return whd;

            whd.PathList = hpath.Split(new char[] { '/' });
            whd.SetKeys(keys.Split(new char[] { ',' }));

            for (int i = 0; i < count; i++)
            {
                WeechatHdataEntry hde = new WeechatHdataEntry();
                for (int j = 0; j < whd.PathList.Length; j++)
                {
                    string pointer = GetPointer();
                    hde.AddPointer(pointer);
                }

                for (int j = 0; j < whd.KeyList.Length; j++)
                {
                    hde.AddObject(whd.KeyList[j], GetObject(whd.TypeList[j]));
                }
                whd.AddItem(hde);
            }
            return whd;
        }

        public WeechatInfo GetInfo()
        {
            string name = GetString();
            string value = GetString();
            return new WeechatInfo(name, value);
        }

        public WeechatInfoList GetInfolist()
        {
            string name = GetString();
            int count = GetUnsignedInt();

            WeechatInfoList wil = new WeechatInfoList(name);

            for (int i = 0; i < count; i++)
            {
                int numItems = GetUnsignedInt();
                Dictionary<string, WinWeeRelayObject> variables = new Dictionary<string, WinWeeRelayObject>();
                for (int j = 0; j < numItems; j++)
                {
                    string itemName = GetString();
                    WeechatType itemType = GetWeechatType();
                    WinWeeRelayObject item = GetObject(itemType);
                    variables.Add(itemName, item);
                }
                wil.AddItem(variables);
            }

            return wil;
        }

        public WeechatArray GetArray()
        {
            WeechatType arrayType = GetWeechatType();
            int arraySize = GetUnsignedInt();
            WeechatArray arr = new WeechatArray(arrayType, arraySize);
            for (int i = 0; i < arraySize; i++)
            {
                arr.Add(GetObject(arrayType));
            }
            return arr;
        }

        private WeechatType GetWeechatType()
        {
            char a = GetChar();
            char b = GetChar();
            char c = GetChar();

            WeechatType type = (WeechatType)Enum.Parse(typeof(WeechatType), new string(new char[] { a, b, c }).ToUpper());
            return type;
        }

        public WinWeeRelayObject GetObject()
        {
            WeechatType type = GetWeechatType();
            return GetObject(type);
        }

        private WinWeeRelayObject GetObject(WeechatType type)
        {
            WinWeeRelayObject ret = null;

            switch (type)
            {
                case WeechatType.CHR:
                    ret = new WinWeeRelayObject(GetChar());
                    break;
                case WeechatType.INT:
                    ret = new WinWeeRelayObject(GetUnsignedInt());
                    break;
                case WeechatType.LON:
                    ret = new WinWeeRelayObject(GetLong());
                    break;
                case WeechatType.STR:
                    ret = new WinWeeRelayObject(GetString());
                    break;
                case WeechatType.BUF:
                    ret = new WinWeeRelayObject(GetBuffer());
                    break;
                case WeechatType.PTR:
                    ret = new WinWeeRelayObject(GetPointer());
                    break;
                case WeechatType.TIM:
                    ret = new WinWeeRelayObject(GetTime());
                    break;
                case WeechatType.ARR:
                    ret = new WinWeeRelayObject(GetArray());
                    break;
                case WeechatType.HTB:
                    ret = GetHashtable();
                    break;
                case WeechatType.HDA:
                    ret = GetHdata();
                    break;
                case WeechatType.INF:
                    ret = GetInfo();
                    break;
                case WeechatType.INL:
                    ret = GetInfolist();
                    break;
            }

            if (ret != null)
                ret.Type = type;

            return ret;
        }

        public byte[] GetByteArray()
        {
            return ArrayHelper.CopyOfRange(_data, _pointer, _data.Length);
        }
    }
}
