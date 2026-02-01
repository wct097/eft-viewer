using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EftViewer.Desktop.ViewModels
{
    /// <summary>
    /// Base class for tree view nodes.
    /// </summary>
    public abstract partial class TreeNodeViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _displayName = string.Empty;

        [ObservableProperty]
        private bool _isExpanded = false;

        [ObservableProperty]
        private bool _isSelected = false;

        public ObservableCollection<TreeNodeViewModel> Children { get; } = new();

        /// <summary>
        /// Gets the detail text to display when this node is selected.
        /// </summary>
        public abstract string DetailText { get; }

        /// <summary>
        /// Indicates if this node has image data to display.
        /// </summary>
        public virtual bool HasImage => false;

        /// <summary>
        /// Gets the image data if this node represents an image.
        /// </summary>
        public virtual byte[]? ImageData => null;
    }
}
