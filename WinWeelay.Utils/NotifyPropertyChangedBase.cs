using System.ComponentModel;

namespace WinWeelay.Utils
{
    /// <summary>
    /// Base class for objects used as view models.
    /// </summary>
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Event for reporting property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Emit events for a property change.
        /// </summary>
        /// <param name="propertyName">The property that has changed.</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
