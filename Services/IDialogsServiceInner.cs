using AvaloniaApp.Core.Services.Dto;

namespace AvaloniaApp.Services;

public interface IDialogsServiceInner
{
    Task<IFileInfo?> ChooseFileAsync();
}

