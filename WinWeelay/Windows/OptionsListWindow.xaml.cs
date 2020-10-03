using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

            ScrollViewer scrollViewer = GetScrollViewer(_optionsListView);
            scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;

            ((OptionsListViewModel)DataContext).Search(null);
        }

        private ScrollViewer GetScrollViewer(DependencyObject depObj)
        {
            if (depObj is ScrollViewer) 
                return depObj as ScrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                ScrollViewer result = GetScrollViewer(child);
                if (result != null) 
                    return result;
            }
            return null;
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer view = (ScrollViewer)sender;
            OptionsListViewModel viewModel = (OptionsListViewModel)DataContext;
            double progress = view.VerticalOffset / view.ScrollableHeight;
            if (progress > 0.7 & !viewModel.IsChangingScroll && !viewModel.IsFullyLoaded)
            {
                viewModel.IsChangingScroll = true;
                viewModel.LoadOptions();
            }
        }

        public int GetPossibleNumberOfVisibleItems()
        {
            return (int)_optionsListView.ActualHeight / 20;
        }

        public int GetStepSize()
        {
            return (int)_optionsListView.ActualHeight / 10;
        }

        public void ResetScroll()
        {
            ScrollViewer scrollViewer = GetScrollViewer(_optionsListView);
            scrollViewer.ScrollToTop();
        }
    }
}
