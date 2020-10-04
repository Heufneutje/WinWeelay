using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace WinWeelay.CustomControls
{
    /// <summary>
    /// Helper to allow for double clicking items in ListViews.
    /// </summary>
    public class ControlItemDoubleClick : DependencyObject
    {
        /// <summary>
        /// Empty constructor for designer.
        /// </summary>
        public ControlItemDoubleClick() { }

        /// <summary>
        /// Property to allow double clicking on items.
        /// </summary>
        public static readonly DependencyProperty ItemsDoubleClickProperty = DependencyProperty.RegisterAttached("ItemsDoubleClick", typeof(bool), typeof(Binding));

        /// <summary>
        /// Set property on a given UI element.
        /// </summary>
        /// <param name="element">The element to apply the property to.</param>
        /// <param name="value">Enable/disable the property.</param>
        public static void SetItemsDoubleClick(ItemsControl element, bool value)
        {
            element.SetValue(ItemsDoubleClickProperty, value);
            if (value)
                element.PreviewMouseDoubleClick += new MouseButtonEventHandler(Element_PreviewMouseDoubleClick);
        }

        private static void Element_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ItemsControl control = sender as ItemsControl;
            foreach (InputBinding b in control.InputBindings)
            {
                if (!(b is MouseBinding))
                    continue;

                if (b.Gesture != null && b.Gesture is MouseGesture && ((MouseGesture)b.Gesture).MouseAction == MouseAction.LeftDoubleClick && b.Command.CanExecute(null))
                {
                    b.Command.Execute(null);
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Get the property value for a given UI element.
        /// </summary>
        /// <param name="element">The element to check the property of.</param>
        /// <returns>The state of the property for the element.</returns>
        public static bool GetItemsDoubleClick(ItemsControl element)
        {
            return (bool)element.GetValue(ItemsDoubleClickProperty);
        }
    }
}
