using System.Text.Json.Serialization;

namespace KosovoPay.Dtos;

/// <summary>
/// A single-page list response envelope: <c>{ object: "list", data, has_more, url }</c>.
/// </summary>
/// <typeparam name="T">The DTO type of each item.</typeparam>
public sealed record ListEnvelope<T>(
    [property: JsonPropertyName("data")] IReadOnlyList<T> Data,
    [property: JsonPropertyName("has_more")] bool HasMore,
    [property: JsonPropertyName("url")] string Url
);
