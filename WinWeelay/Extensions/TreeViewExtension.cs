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
            object selected = treeView.SelectedItem;
            if (selected == null)
                return;

            treeView.Focus();
            TreeViewItem tvi = ContainerFromItem(treeView, selected) as TreeViewItem;
            if (tvi != null) 
                tvi.IsSelected = false;
        }

        private static TreeViewItem ContainerFromItem(TreeView treeView, object item)
        {
            TreeViewItem containerThatMightContainItem = (TreeViewItem)treeView.ItemContainerGenerator.ContainerFromItem(item);
            if (containerThatMightContainItem != null)
                return containerThatMightContainItem;
            else
                return ContainerFromItem(treeView.ItemContainerGenerator, treeView.Items, item);
        }

        private static TreeViewItem ContainerFromItem(ItemContainerGenerator parentItemContainerGenerator, ItemCollection itemCollection, object item)
        {
            foreach (object curChildItem in itemCollection)
            {
                TreeViewItem parentContainer = (TreeViewItem)parentItemContainerGenerator.ContainerFromItem(curChildItem);
                if (parentContainer == null)
                    return null;

                TreeViewItem containerThatMightContainItem = (TreeViewItem)parentContainer.ItemContainerGenerator.ContainerFromItem(item);
                if (containerThatMightContainItem != null)
                    return containerThatMightContainItem;

                TreeViewItem recursionResult = ContainerFromItem(parentContainer.ItemContainerGenerator, parentContainer.Items, item);
                if (recursionResult != null)
                    return recursionResult;
            }
            return null;
        }
    }
}
