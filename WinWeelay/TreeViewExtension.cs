using System.Windows.Controls;

namespace WinWeelay
{
    /// <summary>
    /// Extension methods for TreeViews.
    /// </summary>
    public static class TreeViewExtension
    {
        /// <summary>
        /// Select a given item in the TreeView.
        /// </summary>
        /// <param name="treeView">A given TreeView.</param>
        /// <param name="item">The item to select.</param>
        public static bool SelectItem(this TreeView treeView, object item)
        {
            return SetSelected(treeView, item);
        }

        private static bool SetSelected(ItemsControl parent, object child)
        {
            if (parent == null || child == null)
                return false;

            TreeViewItem childNode = parent.ItemContainerGenerator.ContainerFromItem(child) as TreeViewItem;

            if (childNode != null)
            {
                childNode.Focus();
                return childNode.IsSelected = true;
            }

            if (parent.Items.Count > 0)
            {
                foreach (object childItem in parent.Items)
                {
                    ItemsControl childControl = (ItemsControl)parent.ItemContainerGenerator.ContainerFromItem(childItem);
                    if (SetSelected(childControl, child))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Clear the selection of a given TreeView.
        /// </summary>
        /// <param name="treeView">A given TreeView.</param>
        public static void ClearSelection(this TreeView treeView)
        {
            if (treeView != null)
                ClearTreeViewItemsControlSelection(treeView.Items, treeView.ItemContainerGenerator);
        }

        private static void ClearTreeViewItemsControlSelection(ItemCollection ic, ItemContainerGenerator icg)
        {
            if (ic == null || icg == null)
                return;

            for (int i = 0; i < ic.Count; i++)
            {
                TreeViewItem tvi = icg.ContainerFromIndex(i) as TreeViewItem;
                if (tvi != null)
                {
                    ClearTreeViewItemsControlSelection(tvi.Items, tvi.ItemContainerGenerator);
                    tvi.IsSelected = false;
                }
            }
        }
    }
}
