using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WinWeelay.Utils
{
    /// <summary>
    /// Base class for checking property changes and keeping track of original values.
    /// </summary>
    [Serializable]
    public abstract class BaseChangeTrackable : NotifyPropertyChangedBase
    {
        [NonSerialized]
        private Dictionary<string, object> _originalValues;

        [NonSerialized]
        private Dictionary<string, object> _changedValues;

        [NonSerialized]
        private bool _hasChangesCache;

        /// <summary>
        /// Set the property current values as the orignal values and start keeping track of property changes.
        /// </summary>
        public virtual void StartTrackingChanges()
        {
            _originalValues = new Dictionary<string, object>();

            PropertyInfo[] properties = GetType().GetProperties();
            foreach (PropertyInfo property in properties)
                if (!property.GetCustomAttributes(typeof(ChangeTrackingIgnoreAttribute), true).Any())
                    _originalValues.Add(property.Name, property.GetValue(this));
        }

        /// <summary>
        /// Get all of the changes made to this object since the last StartTrackingChanges() call.
        /// </summary>
        /// <param name="refreshChanges">Whether or not the properties should be rechecked for changes.</param>
        /// <returns>A dictionary which contains the changes. Contains the property name and its current value.</returns>
        public Dictionary<string, object> GetChanges(bool refreshChanges = false)
        {
            if (_originalValues == null)
                _originalValues = new Dictionary<string, object>();

            if (!_hasChangesCache || refreshChanges)
            {
                _changedValues = new Dictionary<string, object>();

                foreach (PropertyInfo property in GetType().GetProperties())
                    if (_originalValues.ContainsKey(property.Name) && !Equals(property.GetValue(this, null), _originalValues[property.Name]))
                        _changedValues.Add(property.Name, property.GetValue(this));

                _hasChangesCache = true;
            }

            return _changedValues;
        }

        /// <summary>
        /// Check whether a given property value has changed since the last StartTrackingChanges() call.
        /// </summary>
        /// <param name="propertyName">The name of the property that is be checked.</param>
        /// <param name="refreshChanges">Whether or not the properties should be rechecked for changes.</param>
        /// <returns>Whether or not the property value has changed.</returns>
        public bool HasPropertyChanged(string propertyName, bool refreshChanges = false)
        {
            return GetChanges(refreshChanges).ContainsKey(propertyName);
        }

        /// <summary>
        /// Check whether this object has any properties that have changed values since the last StartTrackingChanges() call.
        /// </summary>
        /// <param name="refreshChanges">Whether or not the properties should be rechecked for changes.</param>
        /// <returns>Whether or not the property values have changed.</returns>
        public bool HasChanges(bool refreshChanges = false)
        {
            return GetChanges(refreshChanges).Any();
        }

        /// <summary>
        /// Clear the original values and changed values that have been tracked since the last StartTrackingChanges() call.
        /// </summary>
        public void ResetTrackingChanges()
        {
            _originalValues.Clear();
            _changedValues.Clear();
            _hasChangesCache = false;
        }
    }
}
