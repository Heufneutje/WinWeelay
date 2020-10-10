using System.Windows;
using MWindowLib;
using WinWeelay.Core;

namespace WinWeelay
{
    /// <summary>
    /// Dialog to edit passwords.
    /// </summary>
    public partial class OptionPasswordWindow : MetroWindow, IOptionWindow
    {
        /// <summary>
        /// Create a new instance of the window to edit the option.
        /// </summary>
        /// <param name="viewModel">The view model for the logic behind the editor.</param>
        public OptionPasswordWindow(OptionViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _passwordBox.Password = viewModel.EditValue;
        }

        private void _setToNullCheckbox_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            ((OptionViewModel)DataContext).NotifySetToNullChanged();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            OptionViewModel viewModel = ((OptionViewModel)DataContext);
            viewModel.EditValue = _passwordBox.Password;
            viewModel.Commit();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
