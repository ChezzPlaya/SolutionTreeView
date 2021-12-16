using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Input;
using System.Xml;
using HandyControl.Tools.Command;
using ProjectExplorerTree.Demo.CustomTreeTypes;
using ProjectExplorerTree.TreeNodeTypes;

namespace ProjectExplorerTree.Demo;

public class ViewModel
{
    public ICommand SerializeCmd { get; }
    public ObservableCollection<TreeNodeBase?> Tree { get; } = new();
    public ViewModel()
    {
        SerializeCmd = new RelayCommand(SerializeTree);

        for (int k = 0; k < 1; k++)
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
    private void SerializeTree(object obj)
    {
        IEnumerable<Type> knownTypes = new[]
        {
            typeof(FolderTreeNode), typeof(SimpleFileTreeNode), typeof(FileTreeNode), typeof(TestCaseFileTreeNode), typeof(TestSequenceTreeNode),
            typeof(ObservableCollection<TreeNodeBase>)
        };

        string fileName = "treeNodeSerialized.xml";
        FileStream fs = new(fileName, FileMode.Create);
        XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(fs);
        DataContractSerializer ser = new(typeof(ObservableCollection<TestProjectTreeNode>), knownTypes);
        ser.WriteObject(writer, Tree);
        writer.Close();
    }

    private ObservableCollection<TreeNodeBase>? DeserializeTree()
    {
        IEnumerable<Type> knownTypes = new[]
        {
            typeof(FolderTreeNode), typeof(SimpleFileTreeNode), typeof(FileTreeNode), typeof(TestCaseFileTreeNode), typeof(TestSequenceTreeNode),
            typeof(ObservableCollection<TreeNodeBase>)
        };
        
        string fileName = "treeNodeSerialized.xml";
        FileStream fs = new(fileName, FileMode.Open);
        XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
        DataContractSerializer ser = new DataContractSerializer(typeof(ObservableCollection<TestProjectTreeNode>), knownTypes);

        // Deserialize the data and read it from the instance.
        var dTestProjectTreeNodes = (ObservableCollection<TreeNodeBase>?)ser.ReadObject(reader, true);
        reader.Close();
        fs.Close();

        return dTestProjectTreeNodes;
    }
}