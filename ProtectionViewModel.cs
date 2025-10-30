using AvaloniaApp.Core.Services.Dto;
using AvaloniaApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace AvaloniaApp;

public abstract partial class ProtectionViewModel : ObservableObject
{
    protected readonly System.Collections.ObjectModel.ObservableCollection<IFileItem> FileItems = new();
    private readonly IDialogsServiceInner _dialogsService;

    /// <summary>
    /// Заменить исходные файлы после снятия защиты
    /// </summary>
    [ObservableProperty]
    private bool overwrite = true;

    /// <summary>
    /// Сохранить файлы в
    /// </summary>
    [ObservableProperty]
    private string? destination;

    public ICommand AddFileCommand { get; }

    public ICommand ClearFilesCommand { get; }

    public IEnumerable<IFileItem> Files => FileItems;

    public ProtectionViewModel(IDialogsServiceInner dialogsService)
    {
        Overwrite = true;
        AddFileCommand = new AsyncRelayCommand(AddFile);
        ClearFilesCommand = new RelayCommand(ClearFiles);
        _dialogsService = dialogsService;
}
    internal abstract OperationType Operation { get; }

    private async Task AddFile()
    {
        var file = await _dialogsService.ChooseFileAsync();
        if (file is null)
            return;
        var fileItem = ReadFile(file);
        FileItems.Add(fileItem);
    }

    private void ClearFiles()
    {
        FileItems.Clear();
    }

    private Task CheckFiles(IEnumerable<string> files) => Task.CompletedTask;

    private Task<IEnumerable<IFileItem>> GetFiles(IEnumerable<string> files) => Task.FromResult<IEnumerable<IFileItem>>(Array.Empty<IFileItem>());

    private IFileItem ReadFile(IFileInfo? file)
    {
        if (file == null)
            throw new ArgumentNullException(nameof(file));
        return new FileItem(file.Name, file.FullPath);
    }

    public enum OperationType
    {
        Protect,
        Unprotect
    }

    protected record SaveOptions(bool Overwrite, string? Destination);

    private sealed record FileItem(string Name, string FullPath) : IFileItem;
}