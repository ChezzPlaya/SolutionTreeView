using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ProjectExplorerTree.TreeNodeTypes;

namespace ProjectExplorerTree
{
    /// <summary>
    /// Interaction logic for ExplorerTree.xaml
    /// </summary>
    public partial class ExplorerTree
    {
        public ExplorerTree()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<TreeNodeBase>),
                typeof(ExplorerTree), new PropertyMetadata(null));

        public IEnumerable<TreeNodeBase> ItemsSource
        {
            get => (IEnumerable<TreeNodeBase>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private void AddNewItemViaContextMenu<T>(object sender, string name) where T : TreeNodeBase
        {
            var treeNode = (TreeNodeBase)((MenuItem)sender).DataContext;
            var parent = treeNode.GetParent();
            
            // parent is null -> then we've reached the treenode root
            if (parent is null)
            {
                // Todo: Fix this for more then one root node
                parent = ((ObservableCollection<TreeNodeBase>)TreeViewMain.ItemsSource).First();
                var newItem = (TreeNodeBase)Activator.CreateInstance(typeof(T), name, parent)!;
                parent.AddNewItem(newItem);
            }
            else
            {
                // Check for the file tree node type --> Add item to the parent
                if (treeNode.GetType().IsSubclassOf(typeof(FileTreeNode)))
                {
                    var newItem = (TreeNodeBase)Activator.CreateInstance(typeof(T), name, parent)!;
                    parent.AddNewItem(newItem);
                }
                else
                {
                    var newItem = (TreeNodeBase)Activator.CreateInstance(typeof(T), name, treeNode)!;
                    treeNode.AddNewItem(newItem);
                }
            }
        }

        private void AddNewFolderViaContextMenu(object sender, RoutedEventArgs e)
        {
            AddNewItemViaContextMenu<FolderTreeNode>(sender, "New Folder");
        }
        
        private void AddNewFileViaContextMenu(object sender, RoutedEventArgs e)
        {
            AddNewItemViaContextMenu<SimpleFileTreeNode>(sender, "New File");
        }
        
        // Not used anymore. Recursive version of item deletion
        // private void DeleteItem(IEnumerable<TreeNodeBase?> tree, TreeNodeBase itemToDelete)
        // {
        //     foreach (var treeNode in tree)
        //     {
        //         bool isAnyNodeSelected = treeNode.Children.Any(treeNodeBase => treeNodeBase.IsSelected);
        //
        //         if (isAnyNodeSelected)
        //         {
        //             treeNode.DeleteItem(itemToDelete);
        //             break;
        //         }
        //
        //         if (treeNode.Children.Any())
        //         {
        //             DeleteItem(treeNode.Children, itemToDelete);
        //         }
        //     }
        // }

        private void DeleteCurrentItemViaContextMenu(object sender, RoutedEventArgs e)
        {
            var treeNodeToDelete = (TreeNodeBase)((MenuItem)sender).DataContext;

            var parent = treeNodeToDelete.GetParent();
            parent?.DeleteItem(treeNodeToDelete);

            // unused
            // var treeItemSource = (ObservableCollection<TreeNodeBase>)TreeViewMain.ItemsSource;
            // DeleteItem(treeItemSource, treeNodeToDelete);
        }

        private void TreeViewItemPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch((DependencyObject)e.OriginalSource);
            treeViewItem.Focus();
            e.Handled = true;
        }
        
        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return (TreeViewItem)source;
        }


    }
}