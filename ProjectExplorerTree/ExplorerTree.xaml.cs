using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HandyControl.Data;
using ProjectExplorerTree.Dialog;
using ProjectExplorerTree.TreeNodeTypes;
using DragDrop = System.Windows.DragDrop;
using MessageBox = HandyControl.Controls.MessageBox;

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

        private TreeNodeBase? _sourceItem;
        private TreeNodeBase? _targetItem;

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
            var treeNode = (TreeNodeBase)((MenuItem)sender).DataContext;
            var parent = treeNode.GetParent();

            // parent is null -> then we've reached the treenode root
            if (parent is null)
            {
                parent = FindRootParent();

                if (RetrieveFileNameFromDialog(parent, out string name)) return;

                var newItem = CreateTreeNodeInstance<T>(name, parent);
                parent.AddNewItem(newItem);
            }
            else
            {
                if (RetrieveFileNameFromDialog(parent, out string name)) return;

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

        private bool RetrieveFileNameFromDialog(TreeNodeBase? parent, out string name)
        {
            name = LaunchDialog(parent);
            if (name == string.Empty) return true;
            return false;
        }

        private string LaunchDialog(TreeNodeBase? treeNodeBase)
        {
            var treeItemSource = treeNodeBase?.Children;

            DialogViewModel dialogVm = new DialogViewModel(treeItemSource!);
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

            string type = treeNodeToDelete is FolderTreeNode ? "folder" : "file";

            MessageBoxResult result = MessageBox.Show(new MessageBoxInfo
            {
                Message = $"Delete '{treeNodeToDelete.Name}' {type}",
                Caption = "Delete",
                Button = MessageBoxButton.OKCancel,
                IconBrushKey = ResourceToken.InfoBrush,
                IconKey = ResourceToken.AskGeometry
            });

            if (result == MessageBoxResult.OK)
            {
                var parent = treeNodeToDelete.GetParent();
                parent?.DeleteItem(treeNodeToDelete);
            }

        }

        private void TreeViewItemPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem? treeViewItem = VisualUpwardSearch((DependencyObject)e.OriginalSource);
            treeViewItem?.Focus();
            e.Handled = true;
        }

        static TreeViewItem? VisualUpwardSearch(DependencyObject? source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return (TreeViewItem?)source;
        }

        private void TreeViewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _sourceItem = (TreeNodeBase)TreeViewMain.SelectedItem;
                DragDrop.DoDragDrop(TreeViewMain, TreeViewMain.SelectedValue, DragDropEffects.Copy);
            }
        }

        private void TreeViewDragOver(object sender, DragEventArgs e)
        {
            TreeViewItem? item = GetNearestContainer(e.OriginalSource as UIElement);

            if (_sourceItem != null && item?.DataContext is TreeNodeBase dropItemTarget)
            {
                e.Effects = CheckDropTarget(_sourceItem, dropItemTarget) ? DragDropEffects.Copy : DragDropEffects.None;
            }

        }

        private void ShowActionCannotCompleteError()
        {
            MessageBox.Show(new MessageBoxInfo
            {
                Message = $"Item {_sourceItem?.Name} already exists at new location",
                Caption = "Cannot complete Action",
                Button = MessageBoxButton.OK,
                IconBrushKey = ResourceToken.AccentBrush,
                IconKey = ResourceToken.ErrorGeometry
            });
        }
        private bool IsDropValid()
        {

            if (_targetItem is FolderTreeNode)
            {
                // Verify in this case the children of the folder due to possible duplicates
                bool anyDuplicates = _targetItem.Children.Any(targetItemChild
                    => targetItemChild?.Name.Equals(_sourceItem?.Name, StringComparison.OrdinalIgnoreCase) == true);

                if (anyDuplicates)
                {
                    ShowActionCannotCompleteError();
                    return false;
                }

            }

            if (_sourceItem != null && _sourceItem.Name.Equals(_targetItem?.Name, StringComparison.OrdinalIgnoreCase))
            {
                ShowActionCannotCompleteError();
                return false;
            }

            return true;
        }

        private void TreeViewDrop(object sender, DragEventArgs e)
        {

            if (IsDropValid())
            {
                // Make a shallow copy of the sourceitem
                var newSourceItem = _sourceItem?.ShallowCopy();
                // Update the parent
                newSourceItem?.SetParent(_targetItem);
                _targetItem?.AddNewItem(newSourceItem);

                // Remove the dragged source from the treeview
                var sourceParent = _sourceItem?.GetParent();
                sourceParent?.DeleteItem(_sourceItem);
            }

            e.Handled = true;

        }

        private bool CheckDropTarget(TreeNodeBase source, TreeNodeBase target)
        {
            _sourceItem = source;
            _targetItem = target;

            if (source.GetType().IsSubclassOf(typeof(FileTreeNode)))
            {
                return (!target.GetType().IsSubclassOf(typeof(FileTreeNode)) || !source.GetType().IsSubclassOf(typeof(FileTreeNode)));
            }
            else if (source is FolderTreeNode)
            {
                return (!target.GetType().IsSubclassOf(typeof(FileTreeNode)) || source is not FolderTreeNode);
            }
            else
            {
                return false;
            }

        }

        private TreeViewItem? GetNearestContainer(UIElement? element)
        {
            // Walk up the element tree to the nearest tree view item.
            TreeViewItem? container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }

            return container;
        }

    }
}