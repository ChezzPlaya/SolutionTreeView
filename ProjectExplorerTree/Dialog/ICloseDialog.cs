using System;

namespace ProjectExplorerTree.Dialog;

public interface ICloseDialog
{
    Action? Close { get; set; }
}