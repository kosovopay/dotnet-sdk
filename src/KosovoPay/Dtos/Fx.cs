using System.Text.Json.Serialization;
using KosovoPay.Enums;

namespace KosovoPay.Dtos;

/// <summary>FX conversion details on a Payment.</summary>
public sealed record Fx(
    [property: JsonPropertyName("from")] CurrencyCode From,
    [property: JsonPropertyName("to")] CurrencyCode To,
    [property: JsonPropertyName("rate")] string Rate
);
