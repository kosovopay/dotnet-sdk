using System.Text.Json.Serialization;

namespace KosovoPay.Dtos;

/// <summary>Confirmation that a resource was deleted.</summary>
public sealed record DeletedResource(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("deleted")] bool Deleted
);
