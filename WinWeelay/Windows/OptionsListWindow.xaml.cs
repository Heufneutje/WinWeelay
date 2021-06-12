using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MWindowLib;

namespace WinWeelay
{
    /// <summary>
    /// Window to edit WeeChat options.
    /// </summary>
    public partial class OptionsListWindow : MetroWindow
    {
        /// <summary>
        /// Create a new instance of the window.
        /// </summary>
        public OptionsListWindow()
        {
            InitializeComponent();
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
            if (progress > 0.95 & !viewModel.IsChangingScroll && !viewModel.IsFullyLoaded)
            {
                viewModel.IsChangingScroll = true;
                viewModel.LoadOptions();
            }
        }

        private void ViewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ((OptionsListViewModel)DataContext).UpdateViewSettings();
        }

        /// <summary>
        /// Calculate the number of options that fit on screen to determinte how many to load at a time.
        /// </summary>
        /// <returns>The estimated number of visible items.</returns>
        public int GetPossibleNumberOfVisibleItems()
        {
            return (int)_optionsListView.ActualHeight / 20;
        }

        /// <summary>
        /// Calculate the number of items to load when scrolling.
        /// </summary>
        /// <returns>The number of items to load when scrolling.</returns>
        public int GetStepSize()
        {
            return (int)_optionsListView.ActualHeight / 10;
        }

        /// <summary>
        /// Scroll to the top of the options list.
        /// </summary>
        public void ResetScroll()
        {
            ScrollViewer scrollViewer = GetScrollViewer(_optionsListView);
            scrollViewer.ScrollToTop();
        }
    }
}
