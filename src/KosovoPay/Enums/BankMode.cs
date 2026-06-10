using System.Text.Json.Serialization;

namespace KosovoPay.Enums;

/// <summary>Whether the key / bank is in test or live mode.</summary>
[JsonConverter(typeof(ForwardCompatibleEnumConverter<BankMode>))]
public enum BankMode
{
    /// <summary>Test (sandbox) mode.</summary>
    [JsonPropertyName("test")]
    Test,

    /// <summary>Live (production) mode.</summary>
    [JsonPropertyName("live")]
    Live,

    /// <summary>Forward-compatible fallback for an unrecognised wire value.</summary>
    [JsonPropertyName("unknown")]
    Unknown,
}
