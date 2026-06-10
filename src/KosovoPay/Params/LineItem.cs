using System.Text.Json.Serialization;

namespace KosovoPay.Params;

/// <summary>A single line item in a payment's item list.</summary>
public sealed record LineItem(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("quantity")] int Quantity,
    [property: JsonPropertyName("unit_amount_cents")] int UnitAmountCents,
    [property: JsonPropertyName("sku")] string? Sku = null,
    [property: JsonPropertyName("image_url")] string? ImageUrl = null,
    [property: JsonPropertyName("variant")] string? Variant = null
);
