using System.Text.Json.Serialization;

namespace KosovoPay.Dtos;

/// <summary>Refund capabilities for a bank.</summary>
public sealed record RefundCapability(
    [property: JsonPropertyName("supported")] bool Supported,
    [property: JsonPropertyName("partial")] bool Partial
);
