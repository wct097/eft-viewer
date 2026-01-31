using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EftViewer.Core.Models;
using EftViewer.Core.Parsing;
using EftViewer.Desktop.Services;

namespace EftViewer.Desktop.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IFileDialogService _fileDialogService;
        private readonly EftParser _parser;

        [ObservableProperty]
        private string _windowTitle = "EFT Viewer";

        [ObservableProperty]
        private string _statusText = "Ready";

        [ObservableProperty]
        private string _detailText = "Open an EFT file to view its contents.";

        [ObservableProperty]
        private TreeNodeViewModel? _selectedNode;

        [ObservableProperty]
        private EftFile? _currentFile;

        [ObservableProperty]
        private bool _hasImageData;

        [ObservableProperty]
        private string _imageStatusText = string.Empty;

        public ObservableCollection<TreeNodeViewModel> RootNodes { get; } = new();

        public MainWindowViewModel() : this(new FileDialogService())
        {
        }

        public MainWindowViewModel(IFileDialogService fileDialogService)
        {
            _fileDialogService = fileDialogService;
            _parser = new EftParser();
        }

        partial void OnSelectedNodeChanged(TreeNodeViewModel? value)
        {
            if (value != null)
            {
                DetailText = value.DetailText;
                HasImageData = value.HasImage;

                if (value.HasImage)
                {
                    ImageStatusText = "Fingerprint image (WSQ decoding not yet implemented)";
                }
                else
                {
                    ImageStatusText = string.Empty;
                }
            }
            else
            {
                DetailText = "Select an item to view details.";
                HasImageData = false;
                ImageStatusText = string.Empty;
            }
        }

        [RelayCommand]
        private async Task OpenFileAsync()
        {
            try
            {
                var filePath = await _fileDialogService.ShowOpenFileDialogAsync(
                    "Open EFT File",
                    new[] { "*.eft", "*.ansi", "*.*" });

                if (string.IsNullOrEmpty(filePath))
                    return;

                await LoadFileAsync(filePath);
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
            }
        }

        [RelayCommand]
        private void Exit()
        {
            if (Avalonia.Application.Current?.ApplicationLifetime is
                Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        }

        public async Task LoadFileAsync(string filePath)
        {
            try
            {
                StatusText = $"Loading {filePath}...";

                await Task.Run(() =>
                {
                    CurrentFile = _parser.Parse(filePath);
                });

                if (CurrentFile != null)
                {
                    RootNodes.Clear();
                    RootNodes.Add(new FileNodeViewModel(CurrentFile));

                    WindowTitle = $"EFT Viewer - {System.IO.Path.GetFileName(filePath)}";
                    StatusText = $"{filePath} | {CurrentFile.GetSummary()}";
                    DetailText = RootNodes[0].DetailText;
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error loading file: {ex.Message}";
                DetailText = $"Failed to load file:\n\n{ex.Message}\n\n{ex.StackTrace}";
            }
        }
    }
}
