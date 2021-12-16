using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ProjectExplorerTree.TreeNodeTypes
{
    [DataContract(IsReference = true)]
    public abstract class TreeNodeBase : ObservableObject
    {
        private ObservableCollection<TreeNodeBase?> mChildren;
        
        [DataMember]
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
        
        [DataMember]
        // Children are required to use this in a TreeView
        public ObservableCollection<TreeNodeBase?> Children
        {
            get { return mChildren; }
            private set
            {
                mChildren = value;
            }
        }

        public TreeNodeBase? GetParent() => Parent;
        
        [DataMember]
        protected TreeNodeBase? Parent { get; private set; }

        protected TreeNodeBase(TreeNodeBase? parent)
        {
            mChildren = new ObservableCollection<TreeNodeBase?>();
            IsExpanded = false;
            Parent = parent;
        }

        public void SetParent(TreeNodeBase? newParent)
        {
            Parent = newParent;
        }

        public TreeNodeBase ShallowCopy()
        {
            return (TreeNodeBase)this.MemberwiseClone();
        }
    }
}