using ProjectExplorerTree.TreeNodeTypes;

namespace ProjectExplorerTree.Demo.CustomTreeTypes
{
    public sealed class TestProjectTreeNode : TreeNodeBase
    {
        public TestProjectTreeNode(string name, TreeNodeBase? parent) : base(parent)
        {
            Name = name;
        }
    }
}