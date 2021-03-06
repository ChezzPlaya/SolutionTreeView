# SolutionTreeView

This implementation contains a WPF TreeView with add and delete item support. The item can be either of type "file" or of type "directory".

The general idea of this project is to provide a starting point for everyone who wants to create a kind of **Solution Explorer Tree** (thus the project name), like you find one in e.g. Visual Studio.

## One possible output might look like this:

<img src="https://user-images.githubusercontent.com/76159073/145564487-f818a536-7473-4c12-b19f-7037a1788c1b.png" width="90%"></img> 

#### With default context menu:
<img src="https://user-images.githubusercontent.com/76159073/145564887-7d6c7fdc-ecfb-4ea6-b029-0f50bf14a98c.png" width="90%"></img> 

## See it in action:

### Add file or folder
<img src="https://user-images.githubusercontent.com/76159073/146186137-80d8f99b-3e32-4033-b07f-ae4aa6240500.gif" width="90%">
</img> 

### Reorder file or folder
<img src="https://user-images.githubusercontent.com/76159073/146186253-43bebb66-e423-4804-a7c0-7dd957539cb5.gif" width="90%">
</img> 

### Delete file or folder
<img src="https://user-images.githubusercontent.com/76159073/146186258-a3e5ee1b-18bb-43e2-a85e-1a920e462157.gif" width="90%">
</img> 

### Error handling on drag and drop
<img src="https://user-images.githubusercontent.com/76159073/146186259-a153f0a3-e9f9-4f68-9a2a-fb0504e06e71.gif" width="90%">
</img> 

### Error handling on item creation
<img src="https://user-images.githubusercontent.com/76159073/146186260-432d9264-647a-4296-a79d-223d05f2dad0.gif" width="90%">
</img> 

## Usage:
### Requirements:
- NET 6.0
- HandyControls lib (https://hosseini.ninja/handycontrol/quick_start/) `dotnet add package HandyControls --version 3.3.9`
- MahApps Material Icons `dotnet add package MahApps.Metro.IconPacks.Material --version 4.11.0`

### XAML:

**Provide the namespace**:
`xmlns:projectExplorerTree="clr-namespace:ProjectExplorerTree;assembly=ProjectExplorerTree"`

**Add the control to your xaml**:
`<projectExplorerTree:ExplorerTree ItemsSource="{Binding Tree}"/>`

**To build the tree in ViewModel** (_**you'll need at least one root tree node**_):

``` CSharp
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
```

If you'd like to create you own treeviewitem file style then simply inherit from the **FileTreeNode** like so:

```CSharp
public sealed class TestCaseFileTreeNode : FileTreeNode
{
    public TestCaseFileTreeNode(string name, TreeNodeBase? parent) : base(parent)
    {
        Name = name;
    }
  }
```

and provide the DataTemplate in your Resources:

```xaml
<HierarchicalDataTemplate DataType="{x:Type customTreeTypes:TestCaseFileTreeNode}"
                          ItemsSource="{Binding Children}">
    <StackPanel Orientation="Horizontal">
        <iconPacks:PackIconMaterial
            Foreground="Green"
            Kind="File"
            Margin="3,0,5,0" />
        <TextBlock Text="{Binding Name}" />
    </StackPanel>
</HierarchicalDataTemplate>
<HierarchicalDataTemplate DataType="{x:Type customTreeTypes:TestSequenceTreeNode}"
                          ItemsSource="{Binding Children}">
    <StackPanel Orientation="Horizontal">
        <iconPacks:PackIconMaterial
            Foreground="Orange"
            Kind="File"
            Margin="3,0,5,0" />
        <TextBlock Text="{Binding Name}" />
    </StackPanel>
</HierarchicalDataTemplate>
```

The control then knows how to handle this because of the data type. 

Please take a look at the provided Example Project: **ProjectExplorerTree.Demo**
