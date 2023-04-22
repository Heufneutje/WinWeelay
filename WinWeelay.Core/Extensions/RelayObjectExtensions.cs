using System;
using WinWeelay.Core.DataTypes;

namespace WinWeelay.Core
{
    /// <summary>
    /// Extensions to easily get values from relay objects.
    /// </summary>
    public static class RelayObjectExtensions
    {
        /// <summary>
        /// Validate that the object is of a given type.
        /// </summary>
        /// <param name="weechatType">A given type.</param>
        private static void CheckType(this WeechatRelayObject obj, WeechatType weechatType)
        {
            if (obj.Type != weechatType)
                throw new InvalidCastException($"Cannot convert from {obj.Type} to {weechatType}");
        }

        /// <summary>
        /// Extract character value.
        /// </summary>  
        /// <param name="obj">The base relay object.</param>
        /// <returns>A character.</returns>
        public static char AsChar(this WeechatRelayObject obj)
        {
            CheckType(obj, WeechatType.Char);
            return (obj as WeechatSimpleValue<char>).Value;
        }

        /// <summary>
        /// Extract 32-bit integer value.
        /// </summary>
        /// <param name="obj">The base relay object.</param>
        /// <returns>A 32-bit integer.</returns>
        public static int AsInt(this WeechatRelayObject obj)
        {
            CheckType(obj, WeechatType.Int32);
            return (obj as WeechatSimpleValue<int>).Value;
        }

        /// <summary>
        /// Extract 64-bit integer value.
        /// </summary>
        /// <param name="obj">The base relay object.</param>
        /// <returns>A 64-bit integer.</returns>
        public static long AsLong(this WeechatRelayObject obj)
        {
            CheckType(obj, WeechatType.Int64);
            return (obj as WeechatSimpleValue<long>).Value;
        }

        /// <summary>
        /// Extract string value.
        /// </summary>
        /// <param name="obj">The base relay object.</param>
        /// <returns>A string.</returns>
        public static string AsString(this WeechatRelayObject obj)
        {
            CheckType(obj, WeechatType.String);
            return (obj as WeechatSimpleValue<string>).Value;
        }

        /// <summary>
        /// Extract byte array value.
        /// </summary>
        /// <param name="obj">The base relay object.</param>
        /// <returns>A byte array.</returns>
        public static byte[] AsBuffer(this WeechatRelayObject obj)
        {
            CheckType(obj, WeechatType.Buffer);
            return (obj as WeechatSimpleValue<byte[]>).Value;
        }

        /// <summary>
        /// Extract array value.
        /// </summary>
        /// <param name="obj">The base relay object.</param>
        /// <returns>An array.</returns>
        public static WeechatArray AsArray(this WeechatRelayObject obj)
        {
            CheckType(obj, WeechatType.Array);
            return obj as WeechatArray;
        }

        /// <summary>
        /// Extract pointer value.
        /// </summary>
        /// <param name="obj">The base relay object.</param>
        /// <returns>A pointer.</returns>
        public static string AsPointer(this WeechatRelayObject obj)
        {
            CheckType(obj, WeechatType.Pointer);
            return (obj as WeechatSimpleValue<string>).Value;
        }

        /// <summary>
        /// Extract boolean value.
        /// </summary>
        /// <param name="obj">The base relay object.</param>
        /// <returns>A boolean.</returns>
        public static bool AsBoolean(this WeechatRelayObject obj)
        {
            if (obj.Type == WeechatType.Char)
                return AsChar(obj) == '\u0001';
            else if (obj.Type == WeechatType.Int32)
                return AsInt(obj) == 1;

            return false;
        }

        /// <summary>
        /// Extract time value.
        /// </summary>
        /// <param name="obj">The base relay object.</param>
        /// <returns>A time value converted to local time.</returns>
        public static DateTime AsTime(this WeechatRelayObject obj)
        {
            CheckType(obj, WeechatType.Time);
            DateTime unixDate = new(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return unixDate.AddSeconds((obj as WeechatSimpleValue<long>).Value).ToLocalTime();
        }
    }
}
