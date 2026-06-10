using System.Text.Json.Serialization;

namespace KosovoPay.Enums;

/// <summary>Status of a Refund.</summary>
[JsonConverter(typeof(ForwardCompatibleEnumConverter<RefundStatus>))]
public enum RefundStatus
{
    /// <summary>Refund is processing.</summary>
    [JsonPropertyName("pending")]
    Pending,

    /// <summary>Refund completed successfully.</summary>
    [JsonPropertyName("succeeded")]
    Succeeded,

    /// <summary>Refund failed.</summary>
    [JsonPropertyName("failed")]
    Failed,

    /// <summary>Forward-compatible fallback for an unrecognised wire value.</summary>
    [JsonPropertyName("unknown")]
    Unknown,
}
