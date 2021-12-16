using System.Runtime.Serialization;

namespace ProjectExplorerTree.TreeNodeTypes;

[DataContract]
public sealed class SimpleFileTreeNode : FileTreeNode
{
    public SimpleFileTreeNode(string name, TreeNodeBase? parent) : base(parent)
    {
        Name = name;
    }
}