namespace ProjectExplorerTree.TreeNodeTypes
{
    public sealed class TestCaseFileTreeNode : FileTreeNode
    {
        public TestCaseFileTreeNode(string name, TreeNodeBase? parent) : base(parent)
        {
            Name = name;
        }
    }
}