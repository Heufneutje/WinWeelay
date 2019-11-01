using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WinWeelay.Utils
{
    /// <summary>
    /// Extension class for copying an object.
    /// </summary>
    public static class CloneHelper
    {
        /// <summary>
        /// Copy all properties of a given object.
        /// </summary>
        /// <typeparam name="T">The type of the given object.</typeparam>
        /// <param name="source">The object to copy properties from.</param>
        /// <param name="dest">The object to copy properties to.</param>
        public static void CopyPropertiesTo<T>(this T source, T dest)
        {
            List<PropertyInfo> sourceProps = typeof(T).GetProperties().Where(x => x.CanRead).ToList();
            List<PropertyInfo> destProps = typeof(T).GetProperties().Where(x => x.CanWrite).ToList();

            foreach (PropertyInfo sourceProp in sourceProps)
            {
                if (destProps.Any(x => x.Name == sourceProp.Name))
                {
                    PropertyInfo p = destProps.First(x => x.Name == sourceProp.Name);
                    if (p.CanWrite)
                        p.SetValue(dest, sourceProp.GetValue(source, null), null);
                }
            }
        }
    }
}
