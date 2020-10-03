using System.Windows;
using MWindowLib;

namespace WinWeelay
{
    /// <summary>
    /// Interaction logic for OptionComboBoxWindow.xaml
    /// </summary>
    public partial class OptionComboBoxWindow : MetroWindow, IOptionWindow
    {
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
