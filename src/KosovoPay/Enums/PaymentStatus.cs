using System.Text.Json.Serialization;

namespace KosovoPay.Enums;

/// <summary>Lifecycle status of a Payment.</summary>
[JsonConverter(typeof(ForwardCompatibleEnumConverter<PaymentStatus>))]
public enum PaymentStatus
{
    /// <summary>Awaiting bank confirmation.</summary>
    [JsonPropertyName("pending")]
    Pending,

    /// <summary>Authorized by the bank, awaiting capture.</summary>
    [JsonPropertyName("authorized")]
    Authorized,

    /// <summary>Funds captured successfully.</summary>
    [JsonPropertyName("captured")]
    Captured,

    /// <summary>Partially refunded.</summary>
    [JsonPropertyName("partially_refunded")]
    PartiallyRefunded,

    /// <summary>Fully refunded.</summary>
    [JsonPropertyName("refunded")]
    Refunded,

    /// <summary>Payment failed.</summary>
    [JsonPropertyName("failed")]
    Failed,

    /// <summary>Payment was canceled.</summary>
    [JsonPropertyName("canceled")]
    Canceled,

    /// <summary>Forward-compatible fallback for an unrecognised wire value.</summary>
    [JsonPropertyName("unknown")]
    Unknown,
}
