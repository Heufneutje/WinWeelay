using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using WinWeelay.Core.DataTypes;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    /// <summary>
    /// Data structure for a WeeChat reply.
    /// </summary>
    public class RelayDataParser
    {
        private byte[] _data;
        private int _pointer;

        /// <summary>
        /// Initialize a new data parser.
        /// </summary>
        /// <param name="data">De data received on the input on the relay stream.</param>
        public RelayDataParser(byte[] data)
        {
            _data = data;
        }
        
        /// <summary>
        /// Get ID and length of the message.
        /// </summary>
        /// <returns>Header containing ID and length.</returns>
        public RelayMessageHeader GetHeader()
        {
            Uncompress();
            int length = GetUnsignedInt();
            string id = GetString();
            return new RelayMessageHeader(id, length);
        }

        /// <summary>
        /// Get all relay objects contained in the data structure.
        /// </summary>
        /// <returns>A list of relay objects.</returns>
        public List<WeechatRelayObject> GetObjects()
        {
            List<WeechatRelayObject> objects = new();
            while (_pointer < _data.Length)
                objects.Add(GetObject());

            return objects;
        }

        /// <summary>
        /// Uncompress the data if compression is used.
        /// </summary>
        /// <returns>Whether or not compressed is used.</returns>
        private void Uncompress()
        {
            if (_pointer != 0)
                throw new IndexOutOfRangeException("Message is not commpressed or has already been uncompressed.");

            if (GetByte() == 0x01)
            {
                byte[] zlibData = _data.CopyOfRange(_pointer, _data.Length);                
                using (MemoryStream memoryStream = new(zlibData, 2, zlibData.Length - 6))
                using (DeflateStream deflateStream = new(memoryStream, CompressionMode.Decompress))
                using (MemoryStream resultStream = new())
                {
                    deflateStream.CopyTo(resultStream);
                    _data = resultStream.ToArray();
                }

                _pointer = 0;
            }
        }

        /// <summary>
        /// Read a type and matching relay object from the raw data.
        /// </summary>
        /// <returns>A relay object.</returns>
        private WeechatRelayObject GetObject()
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
            return type switch
            {
                WeechatType.Char => new WeechatSimpleValue<char>(GetChar(), WeechatType.Char),
                WeechatType.Int32 => new WeechatSimpleValue<int>(GetUnsignedInt(), WeechatType.Int32),
                WeechatType.Int64 => new WeechatSimpleValue<long>(GetLong(), WeechatType.Int64),
                WeechatType.String => new WeechatSimpleValue<string>(GetString(), WeechatType.String),
                WeechatType.Buffer => new WeechatSimpleValue<byte[]>(GetBuffer(), WeechatType.Buffer),
                WeechatType.Pointer => new WeechatSimpleValue<string>(GetPointer(), WeechatType.Pointer),
                WeechatType.Time => new WeechatSimpleValue<long>(GetTime(), WeechatType.Time),
                WeechatType.Array => GetArray(),
                WeechatType.Hashtable => GetHashtable(),
                WeechatType.Hdata => GetHdata(),
                WeechatType.Info => GetInfo(),
                WeechatType.Infolist => GetInfolist(),
                _ => null,
            };
        }

        /// <summary>
        /// Read an unsigned 32-bit integer from the raw data.
        /// </summary>
        /// <returns>An unsigned 32-bit integer</returns>
        private int GetUnsignedInt()
        {
            if (_pointer + 4 > _data.Length)
                throw new IndexOutOfRangeException("Not enough data");

            int value = GetValue(0, 24) | GetValue(1, 16) | GetValue(2, 8) | GetValue(3, 0);
            _pointer += 4;
            return value;

            int GetValue(int index, int shift)
            {
                return (_data[_pointer + index] & 0xFF) << shift;
            }
        }

        /// <summary>
        /// Read a single byte from the raw data.
        /// </summary>
        /// <returns>A byte.</returns>
        private byte GetByte()
        {
            byte value = (byte)(_data[_pointer] & 0xFF);
            _pointer++;
            return value;
        }

        /// <summary>
        /// Read multiple bytes from the raw data.
        /// </summary>
        /// <returns>Read bytes.</returns>
        private byte[] GetBytes(int length)
        {
            byte[] values = _data[_pointer..(_pointer + length)];
            _pointer += length;
            return values.Select(x => (byte)(x & 0xFF)).ToArray();
        }

        /// <summary>
        /// Read a character from the raw data.
        /// </summary>
        /// <returns>A character.</returns>
        private char GetChar()
        {
            return (char)GetByte();
        }

        /// <summary>
        /// Read multiple characters from the raw data.
        /// </summary>
        /// <returns>Read characters.</returns>
        private char[] GetCharArray(int length)
        {
            return GetBytes(length).Select(x => (char)x).ToArray();
        }

        /// <summary>
        /// Read a 64-bit integer from the raw data.
        /// </summary>
        /// <returns>A 64-bit integer.</returns>
        private long GetLong()
        {
            int length = GetByte();
            if (_pointer + length > _data.Length)
                throw new IndexOutOfRangeException("Not enough data");

            if (length == 0)
                return 0;

            return Convert.ToInt64(new string(GetCharArray(length)));
        }

        /// <summary>
        /// Read a string from the raw data.
        /// </summary>
        /// <returns>A string.</returns>
        private string GetString()
        {
            int length = GetUnsignedInt();
            if (_pointer + length > _data.Length)
                throw new IndexOutOfRangeException("Not enough data");

            if (length == 0)
                return string.Empty;

            if (length == -1)
                return null;

            byte[] bytes = GetBytes(length);

            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Read a byte buffer from the raw data.
        /// </summary>
        /// <returns>A byte buffer.</returns>
        private byte[] GetBuffer()
        {
            int length = GetUnsignedInt();
            if (_pointer + length > _data.Length)
                throw new IndexOutOfRangeException("Not enough data");

            if (length == 0)
                return Array.Empty<byte>();

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
        private string GetPointer()
        {
            int length = GetByte();
            if (_pointer + length > _data.Length)
                throw new IndexOutOfRangeException("Not enough data");

            string value = new(GetCharArray(length));
            if (length == 1 && Convert.ToInt64(value.ToUpper(), 16) == 0)
                return "0x0";

            return $"0x{value}";
        }

        /// <summary>
        /// Read a timestamp from the raw data.
        /// </summary>
        /// <returns>A timestamp.</returns>
        private long GetTime()
        {
            long time = GetLong();
            return time;
        }

        /// <summary>
        /// Read a hashtable from the raw data.
        /// </summary>
        /// <returns>A hashtable.</returns>
        private WeechatHashtable GetHashtable()
        {
            WeechatType keyType = GetWeechatType();
            WeechatType valueType = GetWeechatType();
            int count = GetUnsignedInt();

            WeechatHashtable hta = new();
            for (int i = 0; i < count; i++)
            {
                string key = GetObject(keyType).AsString();
                WeechatRelayObject value = GetObject(valueType);
                hta.Add(key, value);
            }

            return hta;
        }

        /// <summary>
        /// Read an Hdata from the raw data.
        /// </summary>
        /// <returns>An Hdata.</returns>
        private WeechatHdata GetHdata()
        {
            WeechatHdata whd = new();

            string hpath = GetString();
            string keys = GetString();
            int count = GetUnsignedInt();

            if (count == 0)
                return whd;

            whd.PathList = hpath.Split('/');
            string[] keyList = keys.Split(',');

            for (int i = 0; i < count; i++)
            {
                WeechatHdataEntry hde = new();
                for (int j = 0; j < whd.PathList.Length; j++)
                    hde.AddPointer(GetPointer());

                for (int j = 0; j < keyList.Length; j++)
                {
                    string[] keyParams = keyList[j].Split(':');
                    hde.AddObject(keyParams[0], GetObject(WeechatTypeFactory.GetWeechatType(keyParams[1])));
                }

                whd.AddItem(hde);
            }
            return whd;
        }

        /// <summary>
        /// Read an info object from the raw data.
        /// </summary>
        /// <returns>An info object.</returns>
        private WeechatInfo GetInfo()
        {
            string name = GetString();
            string value = GetString();
            return new WeechatInfo(name, value);
        }

        /// <summary>
        /// Read an info list from the raw data.
        /// </summary>
        /// <returns>An info list.</returns>
        private WeechatInfoList GetInfolist()
        {
            string name = GetString();
            int count = GetUnsignedInt();

            WeechatInfoList wil = new(name);

            for (int i = 0; i < count; i++)
            {
                int numItems = GetUnsignedInt();
                Dictionary<string, WeechatRelayObject> variables = new();
                for (int j = 0; j < numItems; j++)
                {
                    string itemName = GetString();
                    WeechatRelayObject item = GetObject();
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
        private WeechatArray GetArray()
        {
            WeechatType arrayType = GetWeechatType();
            int arraySize = GetUnsignedInt();
            WeechatArray arr = new(arrayType, arraySize);
            for (int i = 0; i < arraySize; i++)
                arr.Add(GetObject(arrayType));

            return arr;
        }

        /// <summary>
        /// Read a data type from the raw data.
        /// </summary>
        /// <returns>A data type.</returns>
        private WeechatType GetWeechatType()
        {
            return WeechatTypeFactory.GetWeechatType(new(GetCharArray(3)));
        }        
    }
}
