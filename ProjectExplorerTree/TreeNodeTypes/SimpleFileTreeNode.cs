namespace ProjectExplorerTree.TreeNodeTypes;

public sealed class SimpleFileTreeNode : FileTreeNode
{
    public SimpleFileTreeNode(string name, TreeNodeBase? parent) : base(parent)
    {
        Name = name;
    }
}