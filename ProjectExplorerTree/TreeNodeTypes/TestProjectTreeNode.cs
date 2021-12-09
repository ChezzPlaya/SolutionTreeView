namespace ProjectExplorerTree.TreeNodeTypes
{
    public sealed class TestProjectTreeNode : TreeNodeBase
    {
        public TestProjectTreeNode(string name, TreeNodeBase? parent) : base(parent)
        {
            Name = name;
        }
    }
}