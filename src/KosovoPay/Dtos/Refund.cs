using System.Text.Json.Serialization;
using KosovoPay.Enums;

namespace KosovoPay.Dtos;

/// <summary>A refund against a captured Payment.</summary>
public sealed record Refund(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("payment")] string Payment,
    [property: JsonPropertyName("amount")] int Amount,
    [property: JsonPropertyName("status")] RefundStatus Status,
    [property: JsonPropertyName("reason")] RefundReason? Reason,
    [property: JsonPropertyName("failure_reason")] string? FailureReason,
    [property: JsonPropertyName("created")] long? Created,
    [property: JsonPropertyName("succeeded_at")] long? SucceededAt
)
{
    /// <summary>UTC creation time, or <c>null</c> if not set.</summary>
    public DateTimeOffset? CreatedAt =>
        Created.HasValue ? DateTimeOffset.FromUnixTimeSeconds(Created.Value) : null;
}
