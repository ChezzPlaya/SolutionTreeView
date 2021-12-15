using System.Windows;
using System.Windows.Controls;

namespace ProjectExplorerTree;

/// <summary>
/// Implements an attached property used for styling TreeViewItems when
/// they're a possible drop target.
/// </summary>
public static class TreeViewDropHighlighter
{

    #region private variables

    /// <summary>
    /// the TreeViewItem that is the current drop target
    /// </summary>
    private static TreeViewItem? _currentItem;

    /// <summary>
    /// Indicates whether the current TreeViewItem is a possible
    /// drop target
    /// </summary>
    private static bool _dropPossible;

    #endregion

    #region IsPossibleDropTarget

    /// <summary>
    /// Property key (since this is a read-only DP) for the IsPossibleDropTarget property.
    /// </summary>
    private static readonly DependencyPropertyKey IsPossibleDropTargetKey =
        DependencyProperty.RegisterAttachedReadOnly(
            "IsPossibleDropTarget",
            typeof(bool),
            typeof(TreeViewDropHighlighter),
            new FrameworkPropertyMetadata(null,
                CalculateIsPossibleDropTarget));


    /// <summary>
    /// Dependency Property IsPossibleDropTarget.
    /// Is true if the TreeViewItem is a possible drop target (i.e., if it would receive
    /// the OnDrop event if the mouse button is released right now).
    /// </summary>
    public static readonly DependencyProperty IsPossibleDropTargetProperty = IsPossibleDropTargetKey.DependencyProperty;

    /// <summary>
    /// Getter for IsPossibleDropTarget
    /// </summary>
    public static bool GetIsPossibleDropTarget(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsPossibleDropTargetProperty);
    }

    /// <summary>
    /// Coercion method which calculates the IsPossibleDropTarget property.
    /// </summary>
    private static object CalculateIsPossibleDropTarget(DependencyObject item, object value)
    {
        if ((item == _currentItem) && (_dropPossible))
            return true;
        else
            return false;
    }

    #endregion

    /// <summary>
    /// Initializes the <see cref="TreeViewDropHighlighter"/> class.
    /// </summary>
    static TreeViewDropHighlighter()
    {
        // Get all drag enter/leave events for TreeViewItem.
        EventManager.RegisterClassHandler(typeof(TreeViewItem),
            UIElement.PreviewDragEnterEvent,
            new DragEventHandler(OnDragEvent), true);
        EventManager.RegisterClassHandler(typeof(TreeViewItem),
            UIElement.PreviewDragLeaveEvent,
            new DragEventHandler(OnDragLeave), true);
        EventManager.RegisterClassHandler(typeof(TreeViewItem),
            UIElement.PreviewDragOverEvent,
            new DragEventHandler(OnDragEvent), true);
        EventManager.RegisterClassHandler(typeof(TreeViewItem),      
            UIElement.PreviewDropEvent, 
            new DragEventHandler(OnDragDrop), true);
    }
    static void OnDragDrop(object sender, DragEventArgs args)
    {
        lock (IsPossibleDropTargetProperty)
        {
            _dropPossible = false;

            if (_currentItem != null)
            {
                _currentItem.InvalidateProperty(IsPossibleDropTargetProperty);
            }

            if (sender is TreeViewItem tvi)
            {
                tvi.InvalidateProperty(IsPossibleDropTargetProperty);
            }
        }
    }

    #region event handlers

    /// <summary>
    /// Called when an item is dragged over the TreeViewItem.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
    static void OnDragEvent(object sender, DragEventArgs args)
    {
        lock (IsPossibleDropTargetProperty)
        {
            _dropPossible = false;

            if (_currentItem != null)
            {
                // Tell the item that previously had the mouse that it no longer does.
                DependencyObject oldItem = _currentItem;
                _currentItem = null;
                oldItem.InvalidateProperty(IsPossibleDropTargetProperty);
            }

            if (args.Effects != DragDropEffects.None)
            {
                _dropPossible = true;
            }

            if (sender is TreeViewItem tvi)
            {
                _currentItem = tvi;
                // Tell that item to re-calculate the IsPossibleDropTarget property
                _currentItem.InvalidateProperty(IsPossibleDropTargetProperty);
            }
        }
    }

    /// <summary>
    /// Called when the drag cursor leaves the TreeViewItem
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
    static void OnDragLeave(object sender, DragEventArgs args)
    {
        lock (IsPossibleDropTargetProperty)
        {
            _dropPossible = false;

            if (_currentItem != null)
            {
                // Tell the item that previously had the mouse that it no longer does.
                DependencyObject oldItem = _currentItem;
                _currentItem = null;
                oldItem.InvalidateProperty(IsPossibleDropTargetProperty);
            }

            if (sender is TreeViewItem tvi)
            {
                _currentItem = tvi;
                tvi.InvalidateProperty(IsPossibleDropTargetProperty);
            }
        }
    }

    #endregion

}