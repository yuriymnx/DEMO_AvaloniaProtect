using AvaloniaApp.Core.Services.Dto;
using AvaloniaApp.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Windows.Input;

namespace AvaloniaApp;

public sealed class ProtectionRemoveViewModel : ProtectionViewModel
{
    internal override OperationType Operation => OperationType.Unprotect;

    public string OperationName => "Снятие защиты";

    private readonly ILogger<ProtectionRemoveViewModel> _logger;

    public ProtectionRemoveViewModel(ILogger<ProtectionRemoveViewModel> logger, IDialogsServiceInner dialogsService) : base(dialogsService)
    {
        _logger = logger;
        UnprotectCommand = new AsyncRelayCommand(Unprotect);
    }

    public ICommand UnprotectCommand { get; }


    public async Task Unprotect() => await Unprotect(FileItems, new SaveOptions(Overwrite, Destination));

    private Task Unprotect(IEnumerable<IFileItem> items, SaveOptions saveOptions)
    {
        _logger.LogInformation("Unprotecting files [{file}], Overwrite={Overwrite}, Destination={Destination}", string.Join(",", items), saveOptions.Overwrite, saveOptions.Destination);
        return Task.CompletedTask;
    }
}

