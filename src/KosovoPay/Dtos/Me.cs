using System.Text.Json.Serialization;
using KosovoPay.Enums;

namespace KosovoPay.Dtos;

/// <summary>Authenticated identity: team, mode, enabled banks for this key.</summary>
public sealed record Me(
    [property: JsonPropertyName("team")] Team Team,
    [property: JsonPropertyName("mode")] BankMode Mode,
    [property: JsonPropertyName("key_prefix")] string KeyPrefix,
    [property: JsonPropertyName("enabled_banks")] IReadOnlyList<BankCode> EnabledBanks,
    [property: JsonPropertyName("default_currency")] CurrencyCode? DefaultCurrency
);
