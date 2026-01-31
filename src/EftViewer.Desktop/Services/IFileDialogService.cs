using System.Threading.Tasks;

namespace EftViewer.Desktop.Services
{
    /// <summary>
    /// Service for showing file dialogs.
    /// </summary>
    public interface IFileDialogService
    {
        /// <summary>
        /// Show an open file dialog and return the selected file path.
        /// Returns null if the user cancels.
        /// </summary>
        Task<string?> ShowOpenFileDialogAsync(string title, string[] filters);
    }
}
