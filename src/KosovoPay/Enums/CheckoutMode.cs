using System.Text.Json.Serialization;

namespace KosovoPay.Enums;

/// <summary>Payment checkout mode.</summary>
[JsonConverter(typeof(ForwardCompatibleEnumConverter<CheckoutMode>))]
public enum CheckoutMode
{
    /// <summary>KosovoPay-hosted checkout page.</summary>
    [JsonPropertyName("hosted")]
    Hosted,

    /// <summary>Direct bank redirect — requires a <see cref="BankCode"/>.</summary>
    [JsonPropertyName("direct")]
    Direct,

    /// <summary>Forward-compatible fallback for an unrecognised wire value.</summary>
    [JsonPropertyName("unknown")]
    Unknown,
}
