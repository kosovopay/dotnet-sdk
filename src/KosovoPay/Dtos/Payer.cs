using System.Text.Json.Serialization;

namespace KosovoPay.Dtos;

/// <summary>Optional payer identity attached to a Payment.</summary>
public sealed record Payer(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("email")] string? Email
);
