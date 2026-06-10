using System.Text.Json.Serialization;
using KosovoPay.Enums;

namespace KosovoPay.Dtos;

/// <summary>A bank supported by KosovoPay.</summary>
public sealed record Bank(
    [property: JsonPropertyName("code")] BankCode Code,
    [property: JsonPropertyName("display_name")] string DisplayName,
    [property: JsonPropertyName("logo_url")] string? LogoUrl,
    [property: JsonPropertyName("enabled")] bool Enabled,
    [property: JsonPropertyName("modes")] IReadOnlyList<BankMode> Modes,
    [property: JsonPropertyName("capabilities")] BankCapabilities Capabilities
);
