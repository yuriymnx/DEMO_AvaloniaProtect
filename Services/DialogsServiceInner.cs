using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaApp.Core.Services.Dto;

namespace AvaloniaApp.Services;

public sealed class DialogsServiceInner : IDialogsServiceInner
{
    public async Task<IFileInfo?> ChooseFileAsync()
    {
        var dialog = new OpenFileDialog
        {
            AllowMultiple = false
        };

        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop || desktop.MainWindow is null)
        {
            return null;
        }

        var result = await dialog.ShowAsync(desktop.MainWindow);

        var path = result?.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(path))
            return null;

        return new FileInfoAdapter(path);
    }

    private sealed class FileInfoAdapter : IFileInfo
    {
        public FileInfoAdapter(string path)
        {
            FullPath = path;
            Name = Path.GetFileName(path);
        }

        public string Name { get; }
        public string FullPath { get; }
    }
}

