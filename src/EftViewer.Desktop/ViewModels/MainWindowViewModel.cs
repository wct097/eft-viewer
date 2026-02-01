using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EftViewer.Core.Imaging;
using EftViewer.Core.Models;
using EftViewer.Core.Parsing;
using EftViewer.Desktop.Services;

namespace EftViewer.Desktop.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IFileDialogService _fileDialogService;
        private readonly EftParser _parser;
        private readonly WsqDecoder _wsqDecoder;

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

        [ObservableProperty]
        private WriteableBitmap? _fingerprintImage;

        [ObservableProperty]
        private bool _isDecodingImage;

        public ObservableCollection<TreeNodeViewModel> RootNodes { get; } = new();

        public MainWindowViewModel() : this(new FileDialogService())
        {
        }

        public MainWindowViewModel(IFileDialogService fileDialogService)
        {
            _fileDialogService = fileDialogService;
            _parser = new EftParser();
            _wsqDecoder = new WsqDecoder();
        }

        partial void OnSelectedNodeChanged(TreeNodeViewModel? value)
        {
            if (value != null)
            {
                DetailText = value.DetailText;
                HasImageData = value.HasImage;

                if (value.HasImage && value.ImageData != null)
                {
                    DecodeAndDisplayImage(value.ImageData);
                }
                else
                {
                    FingerprintImage = null;
                    ImageStatusText = string.Empty;
                }
            }
            else
            {
                DetailText = "Select an item to view details.";
                HasImageData = false;
                FingerprintImage = null;
                ImageStatusText = string.Empty;
            }
        }

        private async void DecodeAndDisplayImage(byte[] wsqData)
        {
            try
            {
                IsDecodingImage = true;
                ImageStatusText = "Decoding WSQ image...";

                var decoded = await Task.Run(() => _wsqDecoder.Decode(wsqData));

                // Create Avalonia WriteableBitmap from grayscale pixels
                var bitmap = new WriteableBitmap(
                    new PixelSize(decoded.Width, decoded.Height),
                    new Vector(96, 96),
                    Avalonia.Platform.PixelFormat.Bgra8888,
                    Avalonia.Platform.AlphaFormat.Opaque);

                using (var fb = bitmap.Lock())
                {
                    unsafe
                    {
                        var ptr = (byte*)fb.Address;
                        var stride = fb.RowBytes;

                        for (int y = 0; y < decoded.Height; y++)
                        {
                            for (int x = 0; x < decoded.Width; x++)
                            {
                                var gray = decoded.Pixels[y * decoded.Width + x];
                                var offset = y * stride + x * 4;

                                // BGRA format
                                ptr[offset + 0] = gray; // B
                                ptr[offset + 1] = gray; // G
                                ptr[offset + 2] = gray; // R
                                ptr[offset + 3] = 255;  // A
                            }
                        }
                    }
                }

                FingerprintImage = bitmap;
                ImageStatusText = $"WSQ decoded: {decoded.Width}x{decoded.Height} @ {decoded.Ppi} PPI";
            }
            catch (Exception ex)
            {
                ImageStatusText = $"WSQ decode failed: {ex.Message}";
                FingerprintImage = null;
            }
            finally
            {
                IsDecodingImage = false;
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
                FingerprintImage = null;

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
