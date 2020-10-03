using System.Windows;
using MWindowLib;
using WinWeelay.Core;

namespace WinWeelay
{
    /// <summary>
    /// Interaction logic for OptionStringWindow.xaml
    /// </summary>
    public partial class OptionPasswordWindow : MetroWindow, IOptionWindow
    {
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
