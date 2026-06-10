using System.Text.Json;
using System.Text.Json.Serialization;
using KosovoPay.Enums;

namespace KosovoPay.Dtos;

/// <summary>
/// A webhook event delivered by KosovoPay. The <c>data.object</c> field carries
/// the affected resource. Use <see cref="AsPayment"/> or <see cref="AsRefund"/>
/// to hydrate it based on <see cref="Type"/>.
/// </summary>
public sealed record Event(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("type")] WebhookEventType Type,
    [property: JsonPropertyName("created")] long Created,
    [property: JsonPropertyName("livemode")] bool Livemode,
    [property: JsonPropertyName("api_version")] string ApiVersion,
    [property: JsonPropertyName("data")] EventData Data
)
{
    /// <summary>UTC creation time of this event.</summary>
    public DateTimeOffset CreatedAt => DateTimeOffset.FromUnixTimeSeconds(Created);

    /// <summary>
    /// Hydrates the embedded <c>data.object</c> as a <see cref="Payment"/>.
    /// Only valid for <c>payment.*</c> events.
    /// </summary>
    public Payment AsPayment()
    {
        var obj = Data.Object ?? throw new InvalidOperationException("Event data.object is null.");
        var json = obj.GetRawText();
        return JsonSerializer.Deserialize(json, KosovoPayJsonContext.Default.Payment)
            ?? throw new InvalidOperationException("Could not deserialise Payment from event data.");
    }

    /// <summary>
    /// Hydrates the embedded <c>data.object</c> as a <see cref="Refund"/>.
    /// Only valid for <c>refund.*</c> events.
    /// </summary>
    public Refund AsRefund()
    {
        var obj = Data.Object ?? throw new InvalidOperationException("Event data.object is null.");
        var json = obj.GetRawText();
        return JsonSerializer.Deserialize(json, KosovoPayJsonContext.Default.Refund)
            ?? throw new InvalidOperationException("Could not deserialise Refund from event data.");
    }
}

/// <summary>The <c>data</c> envelope inside a webhook event.</summary>
public sealed record EventData(
    [property: JsonPropertyName("object")] JsonElement? Object,
    [property: JsonPropertyName("previous_attributes")] JsonElement? PreviousAttributes
);
