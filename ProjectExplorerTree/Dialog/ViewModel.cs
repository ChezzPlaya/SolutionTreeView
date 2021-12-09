using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using HandyControl.Tools.Command;

namespace ProjectExplorerTree.Dialog;

public sealed class ViewModel : INotifyPropertyChanged, ICloseWindow
{
    private string _fileName = string.Empty;

    public string FileName
    {
        get => _fileName;
        set
        {
            _fileName = value;
            OnPropertyChanged();
            CloseDialog();
        }
    }

    public ICommand UpdateTextBoxBindingOnEnterCommand { get; }
    public ICommand EscapeOnTextBoxCommand { get; }

    public ViewModel()
    {
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

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public Action? Close { get; set; }
}