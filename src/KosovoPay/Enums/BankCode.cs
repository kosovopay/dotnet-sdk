using System.Text.Json.Serialization;

namespace KosovoPay.Enums;

/// <summary>Supported bank codes.</summary>
[JsonConverter(typeof(ForwardCompatibleEnumConverter<BankCode>))]
public enum BankCode
{
    /// <summary>ProCredit Bank Kosovo.</summary>
    [JsonPropertyName("procredit")]
    Procredit,

    /// <summary>ProCard.</summary>
    [JsonPropertyName("procard")]
    Procard,

    /// <summary>Onefor bank.</summary>
    [JsonPropertyName("onefor")]
    Onefor,

    /// <summary>Forward-compatible fallback for an unrecognised wire value.</summary>
    [JsonPropertyName("unknown")]
    Unknown,
}
