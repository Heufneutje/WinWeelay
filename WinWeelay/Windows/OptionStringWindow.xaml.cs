using System.Windows;
using MWindowLib;

namespace WinWeelay
{
    /// <summary>
    /// Interaction logic for OptionStringWindow.xaml
    /// </summary>
    public partial class OptionStringWindow : MetroWindow, IOptionWindow
    {
        public OptionStringWindow(OptionViewModel viewModel)
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
