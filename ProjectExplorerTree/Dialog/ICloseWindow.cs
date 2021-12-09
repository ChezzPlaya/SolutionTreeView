using System;

namespace ProjectExplorerTree.Dialog;

public interface ICloseWindow
{
    Action Close { get; set; }
}