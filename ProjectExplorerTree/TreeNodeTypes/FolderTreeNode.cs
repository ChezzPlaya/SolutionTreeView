using System.Runtime.Serialization;

namespace ProjectExplorerTree.TreeNodeTypes
{
    [DataContract]
    public sealed class FolderTreeNode : TreeNodeBase
    {
        public FolderTreeNode(string name, TreeNodeBase? parent) : base(parent)
        {
            Name = name;
        }
    }
}