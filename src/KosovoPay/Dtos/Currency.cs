using System.Text.Json.Serialization;
using KosovoPay.Enums;

namespace KosovoPay.Dtos;

/// <summary>A currency supported by KosovoPay.</summary>
public sealed record Currency(
    [property: JsonPropertyName("code")] CurrencyCode Code,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("symbol")] string? Symbol,
    [property: JsonPropertyName("decimals")] int Decimals,
    [property: JsonPropertyName("is_default")] bool IsDefault
);
