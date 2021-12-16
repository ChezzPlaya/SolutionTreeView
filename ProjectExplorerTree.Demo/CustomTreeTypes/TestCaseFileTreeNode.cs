using System.Runtime.Serialization;
using ProjectExplorerTree.TreeNodeTypes;

namespace ProjectExplorerTree.Demo.CustomTreeTypes
{
    [DataContract]
    public sealed class TestCaseFileTreeNode : FileTreeNode
    {
        public TestCaseFileTreeNode(string name, TreeNodeBase? parent) : base(parent)
        {
            Name = name;
        }
    }
}