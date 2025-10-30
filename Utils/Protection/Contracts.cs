namespace AvaloniaApp.Utils.Protection;

public enum ProtectionType
{
    TEMPLATE,
    CUSTOM
}

public interface IUiRight
{
    string VisualName { get; }
    string RightName { get; }
    string Tooltip { get; }
    bool IsRequired { get; }
    bool IsChecked { get; set; }
}

public interface IProtectionData
{
    ProtectionType ProtectionType { get; }
    string? Template { get; }
    IEnumerable<string>? Users { get; }
    IEnumerable<string>? Rights { get; }
    DateTime? ValidTo { get; }
}

