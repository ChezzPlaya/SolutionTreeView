using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ProjectExplorerTree.Dialog;
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

        private void AddNewItemViaContextMenu<T>(object sender) where T : TreeNodeBase
        {
            string name = RetrieveFileNameFromDialog();
            if (name == string.Empty) return;
            
            var treeNode = (TreeNodeBase)((MenuItem)sender).DataContext;
            var parent = treeNode.GetParent();
            
            // parent is null -> then we've reached the treenode root
            if (parent is null)
            {
                parent = FindRootParent();
                var newItem = CreateTreeNodeInstance<T>(name, parent);
                parent.AddNewItem(newItem);
            }
            else
            {
                // Check for the file tree node type --> Add item to the parent
                if (treeNode.GetType().IsSubclassOf(typeof(FileTreeNode)))
                {
                    var newItem = CreateTreeNodeInstance<T>(name, parent);
                    parent.AddNewItem(newItem);
                }
                else
                {
                    var newItem = CreateTreeNodeInstance<T>(name, treeNode);
                    treeNode.AddNewItem(newItem);
                }
            }
        }

        private string RetrieveFileNameFromDialog()
        {
            ViewModel dialogVm = new ViewModel();
            var dialogContent = new ContextMenuAddItemNameDialog
            {
                DataContext = dialogVm
            };

            dialogContent.ShowDialog();

            return dialogVm.FileName;

        }

        private TreeNodeBase FindRootParent()
        {
            return ((ObservableCollection<TreeNodeBase>)TreeViewMain.ItemsSource).First(x => x.IsSelected);
        }

        private static TreeNodeBase CreateTreeNodeInstance<T>(string name, TreeNodeBase parent) where T : TreeNodeBase
        {
            return (TreeNodeBase)Activator.CreateInstance(typeof(T), name, parent)!;
        }

        private void AddNewFolderViaContextMenu(object sender, RoutedEventArgs e)
        {
            AddNewItemViaContextMenu<FolderTreeNode>(sender);
        }
        
        private void AddNewFileViaContextMenu(object sender, RoutedEventArgs e)
        {
            AddNewItemViaContextMenu<SimpleFileTreeNode>(sender);
        }
        
        private void DeleteCurrentItemViaContextMenu(object sender, RoutedEventArgs e)
        {
            var treeNodeToDelete = (TreeNodeBase)((MenuItem)sender).DataContext;

            var parent = treeNodeToDelete.GetParent();
            parent?.DeleteItem(treeNodeToDelete);
            
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