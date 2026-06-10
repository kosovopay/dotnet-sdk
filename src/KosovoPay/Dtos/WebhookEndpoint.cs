using System.Text.Json.Serialization;
using KosovoPay.Enums;

namespace KosovoPay.Dtos;

/// <summary>
/// A configured webhook endpoint.
/// <c>Secret</c> is only present immediately after create or rotate-secret.
/// </summary>
public sealed record WebhookEndpoint(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("enabled_events")] IReadOnlyList<WebhookEventType> EnabledEvents,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("mode")] BankMode Mode,
    [property: JsonPropertyName("created")] long? Created,
    [property: JsonPropertyName("secret")] string? Secret
);
