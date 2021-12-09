using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjectExplorerTree.Dialog;

public sealed class ViewModel : INotifyPropertyChanged
{
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

    public bool Success { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}