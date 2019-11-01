using System;

namespace WinWeelay.Utils
{
    /// <summary>
    /// Attribute to exclude a property from change tracking.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ChangeTrackingIgnoreAttribute : Attribute
    {
    }
}
