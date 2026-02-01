using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace EftViewer.Desktop.Services
{
    /// <summary>
    /// File dialog service implementation using Avalonia's storage provider.
    /// </summary>
    public class FileDialogService : IFileDialogService
    {
        public async Task<string?> ShowOpenFileDialogAsync(string title, string[] filters)
        {
            var window = GetMainWindow();
            if (window == null)
                return null;

            var storageProvider = window.StorageProvider;

            var options = new FilePickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
                FileTypeFilter = filters.Select(f => new FilePickerFileType(f)
                {
                    Patterns = new[] { f }
                }).ToList()
            };

            var result = await storageProvider.OpenFilePickerAsync(options);

            return result.FirstOrDefault()?.Path.LocalPath;
        }

        private static Window? GetMainWindow()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow;
            }
            return null;
        }
    }
}
