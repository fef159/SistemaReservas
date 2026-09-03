using System.Windows;
using System.Windows.Controls;

namespace SistemaReservas.Behaviors;

/// <summary>Adapta PasswordBox a un binding bidireccional. No contiene autenticación.</summary>
public static class PasswordBinding
{
    public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached(
        "Enabled", typeof(bool), typeof(PasswordBinding), new PropertyMetadata(false, OnEnabledChanged));
    public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(
        "Value", typeof(string), typeof(PasswordBinding),
        new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));
    private static readonly DependencyProperty UpdatingProperty = DependencyProperty.RegisterAttached(
        "Updating", typeof(bool), typeof(PasswordBinding), new PropertyMetadata(false));

    public static bool GetEnabled(DependencyObject element) => (bool)element.GetValue(EnabledProperty);
    public static void SetEnabled(DependencyObject element, bool value) => element.SetValue(EnabledProperty, value);
    public static string GetValue(DependencyObject element) => (string)element.GetValue(ValueProperty);
    public static void SetValue(DependencyObject element, string value) => element.SetValue(ValueProperty, value);

    private static void OnEnabledChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
    {
        if (target is not PasswordBox box) return;
        box.PasswordChanged -= OnPasswordChanged;
        if ((bool)args.NewValue)
        {
            box.PasswordChanged += OnPasswordChanged;
            box.Password = GetValue(box) ?? "";
        }
    }

    private static void OnValueChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
    {
        if (target is PasswordBox box && !(bool)box.GetValue(UpdatingProperty))
            box.Password = (string?)args.NewValue ?? "";
    }

    private static void OnPasswordChanged(object sender, RoutedEventArgs args)
    {
        var box = (PasswordBox)sender;
        box.SetValue(UpdatingProperty, true);
        try { box.SetCurrentValue(ValueProperty, box.Password); }
        finally { box.SetValue(UpdatingProperty, false); }
    }
}
