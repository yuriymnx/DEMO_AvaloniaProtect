using AvaloniaApp.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace AvaloniaApp.ViewModels;

public sealed class MainWindowViewModel
{
    public ProtectionApplyViewModel ProtectionApply { get; }
    public ProtectionRemoveViewModel ProtectionRemove { get; }
    public IReadOnlyList<ProtectionViewModel> Tabs { get; }

    public MainWindowViewModel()
    {
        var dialogs = new DialogsServiceInner();
        ProtectionApply = new ProtectionApplyViewModel(NullLogger<ProtectionApplyViewModel>.Instance, dialogs);
        ProtectionRemove = new ProtectionRemoveViewModel(NullLogger<ProtectionRemoveViewModel>.Instance, dialogs);
        Tabs = new List<ProtectionViewModel> { ProtectionApply, ProtectionRemove };
    }
}

