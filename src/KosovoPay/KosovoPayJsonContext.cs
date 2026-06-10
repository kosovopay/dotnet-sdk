using System.Text.Json;
using System.Text.Json.Serialization;
using KosovoPay.Dtos;
using KosovoPay.Enums;

namespace KosovoPay;

/// <summary>
/// Source-generated <see cref="JsonSerializerContext"/> for all KosovoPay DTOs.
/// Provides fast, reflection-free JSON serialisation.
/// </summary>
[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false,
    NumberHandling = JsonNumberHandling.AllowReadingFromString
)]
[JsonSerializable(typeof(Me))]
[JsonSerializable(typeof(Team))]
[JsonSerializable(typeof(Bank))]
[JsonSerializable(typeof(BankCapabilities))]
[JsonSerializable(typeof(RefundCapability))]
[JsonSerializable(typeof(Currency))]
[JsonSerializable(typeof(Rate))]
[JsonSerializable(typeof(Payment))]
[JsonSerializable(typeof(Refund))]
[JsonSerializable(typeof(Fx))]
[JsonSerializable(typeof(Payer))]
[JsonSerializable(typeof(TimelineEvent))]
[JsonSerializable(typeof(WebhookEndpoint))]
[JsonSerializable(typeof(DeletedResource))]
[JsonSerializable(typeof(Event))]
[JsonSerializable(typeof(EventData))]
[JsonSerializable(typeof(ListEnvelope<Bank>))]
[JsonSerializable(typeof(ListEnvelope<Currency>))]
[JsonSerializable(typeof(ListEnvelope<Payment>))]
[JsonSerializable(typeof(ListEnvelope<Refund>))]
[JsonSerializable(typeof(ListEnvelope<WebhookEndpoint>))]
[JsonSerializable(typeof(ListEnvelope<TimelineEvent>))]
[JsonSerializable(typeof(ErrorEnvelope))]
[JsonSerializable(typeof(ErrorBody))]
[JsonSerializable(typeof(IReadOnlyList<BankCode>))]
[JsonSerializable(typeof(IReadOnlyList<CurrencyCode>))]
[JsonSerializable(typeof(IReadOnlyList<WebhookEventType>))]
[JsonSerializable(typeof(IReadOnlyList<BankMode>))]
[JsonSerializable(typeof(JsonElement))]
[JsonSerializable(typeof(IReadOnlyList<JsonElement>))]
[JsonSerializable(typeof(IReadOnlyDictionary<string, JsonElement>))]
public partial class KosovoPayJsonContext : JsonSerializerContext
{
}

/// <summary>Wire shape of an error response envelope.</summary>
public sealed record ErrorEnvelope(
    [property: JsonPropertyName("error")] ErrorBody? Error
);

/// <summary>The nested error object inside an error envelope.</summary>
public sealed record ErrorBody(
    [property: JsonPropertyName("message")] string? Message,
    [property: JsonPropertyName("code")] string? Code,
    [property: JsonPropertyName("type")] string? Type,
    [property: JsonPropertyName("param")] string? Param,
    [property: JsonPropertyName("request_id")] string? RequestId,
    [property: JsonPropertyName("doc_url")] string? DocUrl
);
