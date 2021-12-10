using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using HandyControl.Tools.Command;
using ProjectExplorerTree.TreeNodeTypes;

namespace ProjectExplorerTree.Dialog;

public sealed class DialogViewModel : ObservableObject, ICloseDialog
{
    private readonly IEnumerable<TreeNodeBase>? _treeItemSource;
    private string _fileName = string.Empty;
    private bool _isFileNameInvalid;
    private string _errorString = string.Empty;

    public string ErrorString
    {
        get => _errorString;
        set
        {
            _errorString = value;
            OnPropertyChanged();
        }
    }

    public bool IsFileNameInvalid
    {
        get => _isFileNameInvalid;
        set
        {
            _isFileNameInvalid = value;
            OnPropertyChanged();
        }
    }

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

    public DialogViewModel(IEnumerable<TreeNodeBase>? treeItemSource)
    {
        _treeItemSource = treeItemSource;
        UpdateTextBoxBindingOnEnterCommand = new RelayCommand(ExecuteUpdateTextBoxBindingOnEnterCommand);
        EscapeOnTextBoxCommand = new RelayCommand(EscapeOnTextBox);
    }

    private void EscapeOnTextBox(object obj)
    {
        if (_isFileNameInvalid) _fileName = string.Empty;
        CloseDialog();
    }

    private void ExecuteUpdateTextBoxBindingOnEnterCommand(object parameter)
    {
        if (parameter is TextBox tBox)
        {
            DependencyProperty prop = TextBox.TextProperty;
            BindingExpression? binding = BindingOperations.GetBindingExpression(tBox, prop);
            if (binding != null)
            {
                binding.UpdateSource();
                
                if (_fileName == string.Empty || string.IsNullOrWhiteSpace(_fileName))
                {
                    ErrorString = "File name can not be empty";
                    IsFileNameInvalid = true;
                } 
                else if (_treeItemSource != null && SearchDuplicateTreeNodeItem(_treeItemSource, _fileName))
                {
                    ErrorString = $"A file name with \"{_fileName}\" already exists";
                    IsFileNameInvalid = true;
                }
                else
                {
                    IsFileNameInvalid = false;
                    ErrorString = string.Empty;
                    CloseDialog();
                }
            }
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
            // Check the treenode for duplicate
            if (CheckForName(nameToSearch, treeNode) == true)
            {
                foundDuplicate = true;
                break;
            }
            
            // Verify treenode children for duplicate
            foundDuplicate = treeNode != null && treeNode.Children.Any(
                treeNodeBase => CheckForName(nameToSearch, treeNodeBase) == true);

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

    private static bool? CheckForName(string nameToSearch, TreeNodeBase? treeNodeBase)
    {
        return treeNodeBase?.Name.Equals(nameToSearch,
            StringComparison.OrdinalIgnoreCase);
    }

    public Action? Close { get; set; }
}