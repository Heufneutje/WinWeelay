using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace WinWeelay.CustomControls
{
    /// <summary>
    /// Custom control to select an IRC color.
    /// </summary>
    public partial class IrcColorPicker : UserControl
    {
        /// <summary>
        /// Selected color in the dropdown.
        /// </summary>
        public IrcColor SelectedColor => (IrcColor)colorComboBox.SelectedItem;

        /// <summary>
        /// Evemt which is fired when the selected color changes.
        /// </summary>
        public event EventHandler SelectedColorChanged;

        /// <summary>
        /// Create instance of the color picker.
        /// </summary>
        public IrcColorPicker()
        {
            InitializeComponent();
            colorComboBox.SelectedIndex = 0;
        }

        private void colorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedColorChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Set the selected color based on a given index.
        /// </summary>
        /// <param name="index">Index.</param>
        public void SetSelectedIndex(int index)
        {
            colorComboBox.SelectedIndex = index;
        }

        /// <summary>
        /// Set the selected color based on a WPF color value.
        /// </summary>
        /// <param name="color">A given color.</param>
        public void SetSelectedColor(Color color)
        {
            int? selectedColor = (IrcColor.GetColors().FirstOrDefault(x => x.Color == color)?.ColorIndex);
            if (selectedColor == null || selectedColor == 99)
                colorComboBox.SelectedIndex = 0;
            else
                colorComboBox.SelectedIndex = selectedColor.Value + 1;
        }
    }
}
