using Avalonia;
using Avalonia.Controls;

namespace AvaloniaApp.Views.Protection;

public partial class ProtectionCommonView : UserControl
{
    public static readonly StyledProperty<object?> ChildProperty =
        AvaloniaProperty.Register<ProtectionCommonView, object?>(nameof(Child));

    public object? Child
    {
        get => GetValue(ChildProperty);
        set => SetValue(ChildProperty, value);
    }

    public ProtectionCommonView()
    {
        InitializeComponent();
    }
}


