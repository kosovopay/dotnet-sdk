using System.Text.Json.Serialization;

namespace KosovoPay.Enums;

/// <summary>Types of webhook events dispatched by KosovoPay.</summary>
[JsonConverter(typeof(ForwardCompatibleEnumConverter<WebhookEventType>))]
public enum WebhookEventType
{
    /// <summary>A new payment was created.</summary>
    [JsonPropertyName("payment.created")]
    PaymentCreated,

    /// <summary>A payment was captured.</summary>
    [JsonPropertyName("payment.captured")]
    PaymentCaptured,

    /// <summary>A payment failed.</summary>
    [JsonPropertyName("payment.failed")]
    PaymentFailed,

    /// <summary>A payment was canceled.</summary>
    [JsonPropertyName("payment.canceled")]
    PaymentCanceled,

    /// <summary>A payment expired.</summary>
    [JsonPropertyName("payment.expired")]
    PaymentExpired,

    /// <summary>A refund succeeded.</summary>
    [JsonPropertyName("refund.succeeded")]
    RefundSucceeded,

    /// <summary>A refund failed.</summary>
    [JsonPropertyName("refund.failed")]
    RefundFailed,

    /// <summary>Forward-compatible fallback for an unrecognised wire value.</summary>
    [JsonPropertyName("unknown")]
    Unknown,
}
