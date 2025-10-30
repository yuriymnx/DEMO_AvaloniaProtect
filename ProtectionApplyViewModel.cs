using AvaloniaApp.Core.Services.Dto;
using AvaloniaApp.Services;
using AvaloniaApp.Utils.Protection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Windows.Input;

namespace AvaloniaApp;
public class ProtectionApplyViewModel : ProtectionViewModel
{
    internal override OperationType Operation => OperationType.Protect;

    public string OperationName => "Наложение защиты";

    private readonly ILogger<ProtectionApplyViewModel> _logger;

    private readonly List<UiProtectionType> _protectionTypes;
    private readonly List<TemplateInfo> _templates;
    private readonly List<UiRight> _rights;

    private UiProtectionType _selectedProtectionType;
    public UiProtectionType SelectedProtectionType
    {
        get => _selectedProtectionType;
        set => SetProperty(ref _selectedProtectionType, value);
    }

    private TemplateInfo? _selectedTemplateInfo;
    public TemplateInfo? SelectedTemplateInfo
    {
        get => _selectedTemplateInfo;
        set => SetProperty(ref _selectedTemplateInfo, value);
    }

    private DateTime? _validTo;
    public DateTime? ValidTo
    {
        get => _validTo;
        set => SetProperty(ref _validTo, value);
    }

    private IEnumerable<string>? _userEmails;
    public IEnumerable<string>? UserEmails
    {
        get => _userEmails;
        set => SetProperty(ref _userEmails, value);
    }

    public ICommand ProtectCommand { get; }

    public IEnumerable<UiProtectionType> ProtectionTypes => _protectionTypes;
    public IEnumerable<UiRight>? Rights => _rights;
    public IEnumerable<TemplateInfo> Templates => _templates;

    public ProtectionApplyViewModel(ILogger<ProtectionApplyViewModel> logger, IDialogsServiceInner dialogsService) : base(dialogsService)
    {
        _logger = logger;
        _protectionTypes = GetProtectionTypes().ToList();
        _selectedProtectionType = _protectionTypes.First();
        _templates = GetTemplates().ToList();
        _rights = GetRights().ToList();
        ProtectCommand = new AsyncRelayCommand(Protect);
    }
    private IEnumerable<string>? SelectedRights => _rights.Where(x => x.IsChecked && !string.IsNullOrEmpty(x.RightName)).Select(x => x.RightName);

    private async Task Protect() => await Protect(FileItems);

    private async Task Protect(IEnumerable<IFileItem> items)
    {
        ProtectionData ProtectionData = SelectedProtectionType.Type switch
        {
            ProtectionType.TEMPLATE => GetTemplateProtectionData(),
            ProtectionType.CUSTOM => GetCustomProtectionData(),
            _ => throw new Exception("Cannot create ProtectionData")
        };
        await Protect(items, ProtectionData);

        ProtectionData GetCustomProtectionData()
        {
            if (UserEmails == null || !UserEmails.Any() || SelectedRights == null || !SelectedRights.Any())
            {
                _logger.LogError("Выбрано сохранение с помощью cписока получателей, но UserEmails = [{UserEmails}], SelectedRights = [{SelectedRights}]", string.Join(", ", UserEmails ?? []), string.Join(", ", SelectedRights ?? []));
                throw new Exception();
            }
            return new ProtectionData(UserEmails, SelectedRights, ValidTo);
        }

        ProtectionData GetTemplateProtectionData()
        {
            if (SelectedTemplateInfo == null)
            {
                _logger.LogError("Выбрано сохранение с помощью шаблона, но шаблон не выбран");
                throw new Exception();
            }
            return new ProtectionData(SelectedTemplateInfo.TemplateName);
        }
    }

    private Task Protect(IEnumerable<IFileItem> items, IProtectionData ProtectionData)
    {
        _logger.LogInformation("Protecting files [{files}] with {type}", string.Join(",", items.Select(i => i.FullPath)), ProtectionData.ProtectionType);
        return Task.CompletedTask;
    }

    public IEnumerable<TemplateInfo> GetTemplates()
    {
        yield return new TemplateInfo { TemplateId = "tpl-1", TemplateName = "Default", TemplateVisualName = "Шаблон: Default", TemplateTooltip = "Базовый шаблон" };
        yield return new TemplateInfo { TemplateId = "tpl-2", TemplateName = "Internal", TemplateVisualName = "Шаблон: Internal", TemplateTooltip = "Для внутреннего использования" };
    }

    public Task<IEnumerable<TemplateInfo>> GetTemplatesAsync()
    {
        return Task.FromResult(Templates);
    }

    // просто для отладки формулировки из БТ, RightName - просто для примера
    private IEnumerable<UiRight> GetRights()
    {
        yield return new("Просмотр", "1", "Просмотр", true);
        yield return new("Изменение", "2", "Изменение");
        yield return new("Копирование ", "3", "Копирование");
        yield return new("Печать", "4", "Печать");
        yield return new("Полный доступ", "5", "Полный доступ");
    }
    private IEnumerable<UiProtectionType> GetProtectionTypes()
    {
        yield return new("Template", ProtectionType.TEMPLATE);
        yield return new("Recipient list", ProtectionType.CUSTOM);
    }

    public record UiProtectionType(string VisualName, ProtectionType Type);

    public class UiRight : ObservableObject, IUiRight
    {
        public string VisualName { get; }
        public string RightName { get; }
        public string Tooltip { get; }
        public bool IsRequired { get; }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                var coerced = IsRequired && value == false ? true : value;
                SetProperty(ref _isChecked, coerced);
            }
        }

        public UiRight(string visualName, string rightName, string tooltip, bool isRequired = false, bool isChecked = false)
        {
            VisualName = visualName;
            RightName = rightName;
            Tooltip = tooltip;
            IsRequired = isRequired;
            IsChecked = isChecked;
        }

        // Validation is handled in IsChecked setter to avoid partial methods
    }

    public class TemplateInfo
    {
        // просто что бы что то было, позже поменять на нормальные поля
        public string TemplateId { get; set; } = nameof(TemplateId);
        public string TemplateVisualName { get; set; } = nameof(TemplateVisualName);
        public string TemplateName { get; set; } = nameof(TemplateName);
        public string TemplateTooltip { get; set; } = nameof(TemplateTooltip);
    }

    private class ProtectionData : IProtectionData
    {
        public ProtectionData(string template)
        {
            ProtectionType = ProtectionType.TEMPLATE;
            Template = template;
        }

        public ProtectionData(IEnumerable<string> users, IEnumerable<string> rights, DateTime? validTo)
        {
            ProtectionType = ProtectionType.CUSTOM;
            Users = users;
            Rights = rights;
            ValidTo = validTo;
        }

        public ProtectionType ProtectionType { get; }

        public string? Template { get; }

        public IEnumerable<string>? Users { get; }

        public IEnumerable<string>? Rights { get; }

        public DateTime? ValidTo { get; }
    }
}
