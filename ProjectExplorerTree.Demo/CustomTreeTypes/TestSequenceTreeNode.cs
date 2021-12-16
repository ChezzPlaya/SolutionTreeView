using System.Runtime.Serialization;
using ProjectExplorerTree.TreeNodeTypes;

namespace ProjectExplorerTree.Demo.CustomTreeTypes
{
    [DataContract]
    public sealed class TestSequenceTreeNode : FileTreeNode
    {
        public TestSequenceTreeNode(string name, TreeNodeBase? parent) : base(parent)
        {
            Name = name;
        }

    }
}