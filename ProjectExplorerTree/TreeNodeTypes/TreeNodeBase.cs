using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ProjectExplorerTree.TreeNodeTypes
{
    public abstract class TreeNodeBase : ObservableObject
    {
        private readonly ObservableCollection<TreeNodeBase?> mChildren;
        
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _name = string.Empty;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isExpanded;
        
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private bool _isSelected;
        
        public void AddNewItem(TreeNodeBase? treeNode)
        {
            mChildren.Add(treeNode);
        }

        public void DeleteItem(TreeNodeBase? treeNode)
        {
            mChildren.Remove(treeNode);
        }
        
        // Children are required to use this in a TreeView
        public IEnumerable<TreeNodeBase?> Children
        {
            get { return mChildren; }
        }

        public TreeNodeBase? GetParent() => Parent;
        
        protected TreeNodeBase? Parent { get; }
        
        protected TreeNodeBase(TreeNodeBase? parent)
        {
            mChildren = new ObservableCollection<TreeNodeBase?>();
            IsExpanded = false;
            Parent = parent;
        }
    }
}