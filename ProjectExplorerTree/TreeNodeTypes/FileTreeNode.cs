using System.Runtime.Serialization;

namespace ProjectExplorerTree.TreeNodeTypes;

[DataContract]
public abstract class FileTreeNode : TreeNodeBase
{
    protected FileTreeNode(TreeNodeBase? parent) : base(parent)
    {
    }
}