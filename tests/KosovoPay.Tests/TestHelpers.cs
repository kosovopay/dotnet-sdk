using System.Text.Json;
using Flurl.Http.Testing;
using KosovoPay;
using KosovoPay.Dtos;
using KosovoPay.Enums;

namespace KosovoPay.Tests;

/// <summary>Shared test fixtures and factory helpers.</summary>
internal static class TestHelpers
{
    internal static readonly KosovoPayOptions DefaultOptions = new()
    {
        ApiKey = "sk_test_fake",
        MaxRetries = 1, // No retries in unit tests
    };

    /// <summary>
    /// Returns a payment row fixture with a given <paramref name="id"/>.
    /// </summary>
    internal static object PaymentRow(string id) => new
    {
        @object = "payment",
        id,
        status = "captured",
        mode = "test",
        amount = 100,
        amount_captured = 100,
        amount_refunded = 0,
        currency = "EUR",
        bank_code = (string?)null,
        merchant_reference = (string?)null,
        description = (string?)null,
        payer = (object?)null,
        line_items = (object?)null,
        metadata = new { },
        fx = (object?)null,
        last_error = (string?)null,
        expires_at = (long?)null,
        captured_at = (long?)null,
        created = 1_749_600_000L,
        refunds = Array.Empty<object>(),
        checkout_mode = (string?)null,
        hosted_url = (string?)null,
        redirect_url = (string?)null,
    };

    /// <summary>
    /// Builds a list-envelope JSON string for the given rows.
    /// </summary>
    internal static string ListEnvelope(IEnumerable<object> rows, bool hasMore, string url = "/api/sdk/payments")
    {
        var list = new
        {
            @object = "list",
            data = rows.ToArray(),
            has_more = hasMore,
            url,
        };
        return JsonSerializer.Serialize(list);
    }

    /// <summary>
    /// Builds an error envelope JSON string.
    /// </summary>
    internal static string ErrorBody(string type, string code, int status, string? param = "x", string? requestId = "req_1")
    {
        var envelope = new
        {
            error = new
            {
                type,
                code,
                message = "nope",
                param,
                request_id = requestId,
                doc_url = $"https://docs/{code}",
            },
        };
        return JsonSerializer.Serialize(envelope);
    }
}
