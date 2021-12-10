using System.Collections.ObjectModel;
using ProjectExplorerTree.Demo.CustomTreeTypes;
using ProjectExplorerTree.TreeNodeTypes;

namespace ProjectExplorerTree.Demo;

public class ViewModel
{
    public ObservableCollection<TreeNodeBase?> Tree { get; } = new();
    public ViewModel()
    {
        for (int k = 0; k < 2; k++)
        {
            var testProject = new TestProjectTreeNode($"My Test Project {k}", null) { IsExpanded = true };

            for (int j = 0; j < 2; j++)
            {
                var myFolder = new FolderTreeNode($"My Folder Nr. {j}", testProject) { IsExpanded = true };

                for (int i = 0; i < 2; i++)
                {
                    myFolder.AddNewItem(new TestCaseFileTreeNode($"MyTestCase Nr. {i} of MyRootFolder Nr. {j} ", myFolder));
                }

                var testSequencesNode = new FolderTreeNode( $"MyTestSequences in Nr. {j}", myFolder);
            
                for (int i = 0; i < 2; i++)
                {
                    testSequencesNode.AddNewItem(new TestSequenceTreeNode($"Test sequence Nr.{i}", testSequencesNode));
                }
            
                myFolder.AddNewItem(testSequencesNode);
            
                testProject.AddNewItem(myFolder);
            }
            
            Tree.Add(testProject);
        }
    }
}