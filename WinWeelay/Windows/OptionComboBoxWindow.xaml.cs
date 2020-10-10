using System.Windows;
using MWindowLib;
using WinWeelay.Core;

namespace WinWeelay
{
    /// <summary>
    /// Dialog to edit options with a choice.
    /// </summary>
    public partial class OptionComboBoxWindow : MetroWindow, IOptionWindow
    {
        /// <summary>
        /// Create a new instance of the window to edit the option.
        /// </summary>
        /// <param name="viewModel">The view model for the logic behind the editor.</param>
        public OptionComboBoxWindow(OptionViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void _setToNullCheckbox_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            ((OptionViewModel)DataContext).NotifySetToNullChanged();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ((OptionViewModel)DataContext).Commit();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
