using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace WinWeelay.CustomControls
{
    /// <summary>
    /// Interaction logic for IrcColorPicker.xaml
    /// </summary>
    public partial class IrcColorPicker : UserControl
    {
        public IrcColor SelectedColor => (IrcColor)colorComboBox.SelectedItem;

        public event EventHandler SelectedColorChanged;

        public IrcColorPicker()
        {
            InitializeComponent();
            colorComboBox.SelectedIndex = 0;
        }

        private void colorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedColorChanged?.Invoke(this, e);
        }

        public void SetSelectedIndex(int index)
        {
            colorComboBox.SelectedIndex = index;
        }

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
