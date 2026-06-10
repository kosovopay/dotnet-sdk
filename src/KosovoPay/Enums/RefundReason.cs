using System.Text.Json.Serialization;

namespace KosovoPay.Enums;

/// <summary>Reason code for a Refund.</summary>
[JsonConverter(typeof(ForwardCompatibleEnumConverter<RefundReason>))]
public enum RefundReason
{
    /// <summary>Customer requested the refund.</summary>
    [JsonPropertyName("requested_by_customer")]
    RequestedByCustomer,

    /// <summary>Duplicate transaction.</summary>
    [JsonPropertyName("duplicate")]
    Duplicate,

    /// <summary>Fraudulent transaction.</summary>
    [JsonPropertyName("fraudulent")]
    Fraudulent,

    /// <summary>Other reason.</summary>
    [JsonPropertyName("other")]
    Other,

    /// <summary>Forward-compatible fallback for an unrecognised wire value.</summary>
    [JsonPropertyName("unknown")]
    Unknown,
}
