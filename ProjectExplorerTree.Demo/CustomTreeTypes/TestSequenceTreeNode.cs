using ProjectExplorerTree.TreeNodeTypes;

namespace ProjectExplorerTree.Demo.CustomTreeTypes
{
    public sealed class TestSequenceTreeNode : FileTreeNode
    {
        public TestSequenceTreeNode(string name, TreeNodeBase? parent) : base(parent)
        {
            Name = name;
        }

    }
}