using System.Windows;
using MWindowLib;

namespace WinWeelay
{
    /// <summary>
    /// Interaction logic for OptionBooleanWindow.xaml
    /// </summary>
    public partial class OptionBooleanWindow : MetroWindow, IOptionWindow
    {
        public OptionBooleanWindow(OptionViewModel viewModel)
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
