using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using HandyControl.Tools.Command;
using ProjectExplorerTree.TreeNodeTypes;

namespace ProjectExplorerTree.Dialog;

public sealed class DialogViewModel : INotifyPropertyChanged, ICloseDialog, IDataErrorInfo
{
    private readonly IEnumerable<TreeNodeBase> _treeItemSource;
    private string _fileName = string.Empty;

    public string FileName
    {
        get => _fileName;
        set
        {
            _fileName = value;
            OnPropertyChanged();
        }
    }

    public ICommand UpdateTextBoxBindingOnEnterCommand { get; }
    public ICommand EscapeOnTextBoxCommand { get; }

    public DialogViewModel(IEnumerable<TreeNodeBase> treeItemSource)
    {
        _treeItemSource = treeItemSource;
        UpdateTextBoxBindingOnEnterCommand = new RelayCommand(ExecuteUpdateTextBoxBindingOnEnterCommand);
        EscapeOnTextBoxCommand = new RelayCommand(EscapeOnTextBox);
    }

    private void EscapeOnTextBox(object obj)
    {
        CloseDialog();
    }

    private void ExecuteUpdateTextBoxBindingOnEnterCommand(object parameter)
    {
        if (parameter is TextBox tBox)
        {
            DependencyProperty prop = TextBox.TextProperty;
            BindingExpression? binding = BindingOperations.GetBindingExpression(tBox, prop);
            if (binding != null) 
                binding.UpdateSource();
        }
    }

    void CloseDialog()
    {
        Close?.Invoke();
    }
    
    private bool SearchDuplicateTreeNodeItem(IEnumerable<TreeNodeBase?> tree, string nameToSearch)
    {
        bool foundDuplicate = false;

        foreach (TreeNodeBase? treeNode in tree)
        {
            foundDuplicate = treeNode != null && treeNode.Children.Any(
                treeNodeBase => treeNodeBase?.Name.Equals(nameToSearch,
                    StringComparison.OrdinalIgnoreCase) == true);

            if (foundDuplicate)
            {
                break;
            }

            if (treeNode?.Children.Any() == true)
            {
                SearchDuplicateTreeNodeItem(treeNode.Children, nameToSearch);
            }
        }

        return foundDuplicate;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public Action? Close { get; set; }
    public string Error => string.Empty;

    public string this[string columnName]
    {
        get
        {
            
            string result = string.Empty;

            if (_fileName == string.Empty)
            {
                result = "";
            } 
            else if (SearchDuplicateTreeNodeItem(_treeItemSource, _fileName))
            {
                result = $"A file name with \"{_fileName}\" already exists";
            }
            else
            {
                CloseDialog();
            }

            return result;
            
        }
    }
}