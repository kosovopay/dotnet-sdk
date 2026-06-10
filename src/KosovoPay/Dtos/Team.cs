using System.Text.Json.Serialization;

namespace KosovoPay.Dtos;

/// <summary>A KosovoPay team (merchant account).</summary>
public sealed record Team(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("logo_url")] string? LogoUrl
);
