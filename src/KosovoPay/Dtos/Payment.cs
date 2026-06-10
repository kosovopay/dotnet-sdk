using System.Text.Json;
using System.Text.Json.Serialization;
using KosovoPay.Enums;

namespace KosovoPay.Dtos;

/// <summary>A KosovoPay payment.</summary>
public sealed record Payment(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("status")] PaymentStatus Status,
    [property: JsonPropertyName("mode")] BankMode Mode,
    [property: JsonPropertyName("amount")] int Amount,
    [property: JsonPropertyName("amount_captured")] int AmountCaptured,
    [property: JsonPropertyName("amount_refunded")] int AmountRefunded,
    [property: JsonPropertyName("currency")] CurrencyCode Currency,
    [property: JsonPropertyName("bank_code")] BankCode? BankCode,
    [property: JsonPropertyName("merchant_reference")] string? MerchantReference,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("payer")] Payer? Payer,
    [property: JsonPropertyName("line_items")] IReadOnlyList<JsonElement>? LineItems,
    [property: JsonPropertyName("metadata")] IReadOnlyDictionary<string, JsonElement> Metadata,
    [property: JsonPropertyName("fx")] Fx? FxDetails,
    [property: JsonPropertyName("last_error")] string? LastError,
    [property: JsonPropertyName("expires_at")] long? ExpiresAt,
    [property: JsonPropertyName("captured_at")] long? CapturedAt,
    [property: JsonPropertyName("created")] long Created,
    [property: JsonPropertyName("refunds")] IReadOnlyList<Refund> Refunds,
    [property: JsonPropertyName("checkout_mode")] CheckoutMode? CheckoutMode = null,
    [property: JsonPropertyName("hosted_url")] string? HostedUrl = null,
    [property: JsonPropertyName("redirect_url")] string? RedirectUrl = null
)
{
    /// <summary>UTC creation time.</summary>
    public DateTimeOffset CreatedAt => DateTimeOffset.FromUnixTimeSeconds(Created);
}
