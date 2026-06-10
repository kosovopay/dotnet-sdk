using System.Text.Json.Serialization;
using KosovoPay.Enums;

namespace KosovoPay.Dtos;

/// <summary>A configured webhook endpoint.</summary>
public sealed record WebhookEndpoint(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("enabled_events")] IReadOnlyList<WebhookEventType> EnabledEvents,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("mode")] BankMode Mode,
    [property: JsonPropertyName("created")] long? Created,
    /// <summary>Only present immediately after create or rotate-secret.</summary>
    [property: JsonPropertyName("secret")] string? Secret
);
