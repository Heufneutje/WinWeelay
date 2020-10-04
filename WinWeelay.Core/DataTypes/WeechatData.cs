using System;
using System.Collections.Generic;
using System.Text;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    /// <summary>
    /// Data structure for a WeeChat reply.
    /// </summary>
    public class WeechatData
    {
        private readonly byte[] _data;
        private int _pointer;

        /// <summary>
        /// Check whether there is any data left to parse.
        /// </summary>
        public bool IsEmpty => _pointer == _data.Length;

        /// <summary>
        /// Initialize a new data structure.
        /// </summary>
        /// <param name="data">De data received on the input on the relay stream.</param>
        public WeechatData(byte[] data)
        {
            _data = data;
            _pointer = 0;
        }

        /// <summary>
        /// Read an unsigned 32-bit integer from the raw data.
        /// </summary>
        /// <returns>An unsigned 32-bit integer</returns>
        public int GetUnsignedInt()
        {
            if (_pointer + 4 > _data.Length)
                throw new IndexOutOfRangeException("Not enough data to compute length");

            int ret = ((_data[_pointer + 0] & 0xFF) << 24) | ((_data[_pointer + 1] & 0xFF) << 16)
                     | ((_data[_pointer + 2] & 0xFF) << 8) | ((_data[_pointer + 3] & 0xFF));

            _pointer += 4;
            return ret;
        }

        /// <summary>
        /// Read a single byte from the raw data.
        /// </summary>
        /// <returns>A byte.</returns>
        public int GetByte()
        {
            int ret = _data[_pointer] & 0xFF;

            _pointer++;
            return ret;
        }

        /// <summary>
        /// Read a character from the raw data.
        /// </summary>
        /// <returns>A character.</returns>
        public char GetChar()
        {
            return (char)GetByte();
        }

        /// <summary>
        /// Read a 64-bit integer from the raw data.
        /// </summary>
        /// <returns>A 64-bit integer.</returns>
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

        /// <summary>
        /// Read a string from the raw data.
        /// </summary>
        /// <returns>A string.</returns>
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

        /// <summary>
        /// Read a byte buffer from the raw data.
        /// </summary>
        /// <returns>A byte buffer.</returns>
        public byte[] GetBuffer()
        {
            int length = GetUnsignedInt();
            if (_pointer + length > _data.Length)
                throw new IndexOutOfRangeException("Not enough data");

            if (length == 0)
                return new byte[0];

            if (length == -1)
                return null;

            byte[] ret = _data.CopyOfRange(_pointer, _pointer + length);

            _pointer += length;
            return ret;
        }

        /// <summary>
        /// Read a pointer from the raw data.
        /// </summary>
        /// <returns>A pointer.</returns>
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

        /// <summary>
        /// Read a timestamp from the raw data.
        /// </summary>
        /// <returns>A timestamp.</returns>
        public long GetTime()
        {
            long time = GetLong();
            return time;
        }

        /// <summary>
        /// Read a hashtable from the raw data.
        /// </summary>
        /// <returns>A hashtable.</returns>
        public WeechatHashtable GetHashtable()
        {
            WeechatType keyType = GetWeechatType();
            WeechatType valueType = GetWeechatType();
            int count = GetUnsignedInt();

            WeechatHashtable hta = new WeechatHashtable();
            for (int i = 0; i < count; i++)
            {
                WeechatRelayObject k = GetObject(keyType);
                WeechatRelayObject v = GetObject(valueType);
                hta.Add(k, v);
            }

            return hta;
        }

        /// <summary>
        /// Read an Hdata from the raw data.
        /// </summary>
        /// <returns>An Hdata.</returns>
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

        /// <summary>
        /// Read an info object from the raw data.
        /// </summary>
        /// <returns>An info object.</returns>
        public WeechatInfo GetInfo()
        {
            string name = GetString();
            string value = GetString();
            return new WeechatInfo(name, value);
        }

        /// <summary>
        /// Read an info list from the raw data.
        /// </summary>
        /// <returns>An info list.</returns>
        public WeechatInfoList GetInfolist()
        {
            string name = GetString();
            int count = GetUnsignedInt();

            WeechatInfoList wil = new WeechatInfoList(name);

            for (int i = 0; i < count; i++)
            {
                int numItems = GetUnsignedInt();
                Dictionary<string, WeechatRelayObject> variables = new Dictionary<string, WeechatRelayObject>();
                for (int j = 0; j < numItems; j++)
                {
                    string itemName = GetString();
                    WeechatType itemType = GetWeechatType();
                    WeechatRelayObject item = GetObject(itemType);
                    variables.Add(itemName, item);
                }
                wil.AddItem(variables);
            }

            return wil;
        }

        /// <summary>
        /// Read an array from the raw data.
        /// </summary>
        /// <returns>An array.</returns>
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

        /// <summary>
        /// Read a data type from the raw data.
        /// </summary>
        /// <returns>A data type.</returns>
        private WeechatType GetWeechatType()
        {
            string typeStr = string.Empty;
            for (int i = 0; i < 3; i++)
                typeStr += GetChar();

            WeechatType type = (WeechatType)Enum.Parse(typeof(WeechatType), typeStr.ToUpper());
            return type;
        }

        /// <summary>
        /// Read a type and matching relay object from the raw data.
        /// </summary>
        /// <returns>A relay object.</returns>
        public WeechatRelayObject GetObject()
        {
            WeechatType type = GetWeechatType();
            return GetObject(type);
        }

        /// <summary>
        /// Read a relay object with a given type from the raw data.
        /// </summary>
        /// <param name="type">A given data type.</param>
        /// <returns>A relay object.</returns>
        private WeechatRelayObject GetObject(WeechatType type)
        {
            WeechatRelayObject ret = null;

            switch (type)
            {
                case WeechatType.CHR:
                    ret = new WeechatRelayObject(GetChar());
                    break;
                case WeechatType.INT:
                    ret = new WeechatRelayObject(GetUnsignedInt());
                    break;
                case WeechatType.LON:
                    ret = new WeechatRelayObject(GetLong());
                    break;
                case WeechatType.STR:
                    ret = new WeechatRelayObject(GetString());
                    break;
                case WeechatType.BUF:
                    ret = new WeechatRelayObject(GetBuffer());
                    break;
                case WeechatType.PTR:
                    ret = new WeechatRelayObject(GetPointer());
                    break;
                case WeechatType.TIM:
                    ret = new WeechatRelayObject(GetTime());
                    break;
                case WeechatType.ARR:
                    ret = new WeechatRelayObject(GetArray());
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

        /// <summary>
        /// Retrieve the unparsed data.
        /// </summary>
        /// <returns>A byte array of unparsed data.</returns>
        public byte[] GetByteArray()
        {
            return _data.CopyOfRange(_pointer, _data.Length);
        }
    }
}
