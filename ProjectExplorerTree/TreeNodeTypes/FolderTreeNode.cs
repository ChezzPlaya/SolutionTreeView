namespace ProjectExplorerTree.TreeNodeTypes
{
    public sealed class FolderTreeNode : TreeNodeBase
    {
        public FolderTreeNode(string name, TreeNodeBase? parent) : base(parent)
        {
            Name = name;
        }
    }
}