using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MWindowLib;

namespace WinWeelay.Windows
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
