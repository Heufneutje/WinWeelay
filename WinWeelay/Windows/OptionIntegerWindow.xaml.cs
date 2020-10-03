using System.Windows;
using MWindowLib;
using WinWeelay.Core;

namespace WinWeelay
{
    /// <summary>
    /// Interaction logic for OptionIntegerWindow.xaml
    /// </summary>
    public partial class OptionIntegerWindow : MetroWindow, IOptionWindow
    {
        public OptionIntegerWindow(OptionViewModel viewModel)
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
