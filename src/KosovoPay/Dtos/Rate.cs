using System.Text.Json.Serialization;
using KosovoPay.Enums;

namespace KosovoPay.Dtos;

/// <summary>An FX exchange rate between two currencies.</summary>
public sealed record Rate(
    [property: JsonPropertyName("from")] CurrencyCode From,
    [property: JsonPropertyName("to")] CurrencyCode To,
    [property: JsonPropertyName("rate")] string RateValue,
    [property: JsonPropertyName("synced_at")] string? SyncedAt,
    [property: JsonPropertyName("stale")] bool Stale
);
