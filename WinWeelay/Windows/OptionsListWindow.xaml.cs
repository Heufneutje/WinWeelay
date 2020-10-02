using System.Windows;
using System.Windows.Controls;
using MWindowLib;

namespace WinWeelay
{
    /// <summary>
    /// Interaction logic for OptionsListWindow.xaml
    /// </summary>
    public partial class OptionsListWindow : MetroWindow
    {
        public OptionsListWindow(OptionsListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((OptionsListViewModel)DataContext).OnSelectedOptionChanged();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            searchTextBox.Focus();
        }
    }
}
