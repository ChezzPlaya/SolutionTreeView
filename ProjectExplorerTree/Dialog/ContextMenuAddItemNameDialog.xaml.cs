using System.Windows;

namespace ProjectExplorerTree.Dialog;

public partial class ContextMenuAddItemNameDialog
{
    public ContextMenuAddItemNameDialog()
    {
        InitializeComponent();
        Loaded += DialogWindowLoaded;
    }

    private void DialogWindowLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ICloseDialog vm)
        {
            vm.Close += Close;
        }
    }
}