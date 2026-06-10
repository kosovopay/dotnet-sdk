using System.Text.Json.Serialization;
using KosovoPay.Enums;

namespace KosovoPay.Dtos;

/// <summary>Capabilities of a bank — currencies, minimum amount, step, refund support.</summary>
public sealed record BankCapabilities(
    [property: JsonPropertyName("currencies")] IReadOnlyList<CurrencyCode> Currencies,
    [property: JsonPropertyName("min_amount")] int MinAmount,
    [property: JsonPropertyName("amount_step")] int AmountStep,
    [property: JsonPropertyName("refunds")] RefundCapability Refunds
);
