using System.Reflection;
using System.Text.Json.Serialization;

namespace KosovoPay.Enums;

/// <summary>
/// Pre-computed enum → wire-value dictionaries, built once at startup from the
/// <see cref="JsonPropertyNameAttribute"/> annotations on each enum member.
/// </summary>
internal static class EnumWireValues
{
    internal static readonly IReadOnlyDictionary<CurrencyCode, string> CurrencyCodeValues =
        BuildWireValues<CurrencyCode>();

    internal static readonly IReadOnlyDictionary<BankCode, string> BankCodeValues =
        BuildWireValues<BankCode>();

    internal static readonly IReadOnlyDictionary<RefundReason, string> RefundReasonValues =
        BuildWireValues<RefundReason>();

    internal static readonly IReadOnlyDictionary<WebhookEventType, string> WebhookEventTypeValues =
        BuildWireValues<WebhookEventType>();

    private static IReadOnlyDictionary<TEnum, string> BuildWireValues<TEnum>()
        where TEnum : struct, Enum
    {
        var dict = new Dictionary<TEnum, string>();
        foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var enumVal = (TEnum)field.GetValue(null)!;
            var attr = field.GetCustomAttribute<JsonPropertyNameAttribute>();
            dict[enumVal] = attr?.Name ?? field.Name;
        }
        return dict;
    }
}
