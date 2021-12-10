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

public sealed class DialogViewModel : ObservableObject, ICloseDialog
{
    private readonly IEnumerable<TreeNodeBase> _treeItemSource;
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
            {
                binding.UpdateSource();
                
                if (_fileName == string.Empty)
                {
                    ErrorString = "File name can not be empty";
                    IsFileNameInvalid = true;
                } 
                else if (SearchDuplicateTreeNodeItem(_treeItemSource, _fileName))
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

    public Action? Close { get; set; }
}