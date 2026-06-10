using System.Text.Json.Serialization;

namespace KosovoPay.Dtos;

/// <summary>A single step in a Payment's timeline.</summary>
public sealed record TimelineEvent(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("at")] long At
);
