namespace ProjectExplorerTree.TreeNodeTypes;

[UsedImplicitly]
public sealed class SimpleFileTreeNode : FileTreeNode
{
    public SimpleFileTreeNode(string name, TreeNodeBase? parent) : base(parent)
    {
        Name = name;
    }
}