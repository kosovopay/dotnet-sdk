using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using KosovoPay;
using KosovoPay.Enums;
using KosovoPay.Exceptions;
using Xunit;

namespace KosovoPay.Tests;

/// <summary>
/// Tests for webhook signature verification and event construction.
/// </summary>
public sealed class WebhookTests
{
    private const string Secret = "whsec_test";

    private static string Sign(string body, long timestamp)
    {
        var signedPayload = $"{timestamp}.{body}";
        var secretBytes = Encoding.UTF8.GetBytes(Secret);
        var payloadBytes = Encoding.UTF8.GetBytes(signedPayload);
        var hash = HMACSHA256.HashData(secretBytes, payloadBytes);
        return $"t={timestamp},v1={Convert.ToHexString(hash).ToLowerInvariant()}";
    }

    [Fact]
    public void ConstructEvent_Valid_Returns_TypedEvent()
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var paymentObject = new
        {
            @object = "payment",
            id = "pi_1",
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
            created = 1L,
            refunds = Array.Empty<object>(),
        };

        var body = JsonSerializer.Serialize(new
        {
            id = "evt_1",
            @object = "event",
            type = "payment.captured",
            created = now,
            livemode = false,
            api_version = "2026-06-01",
            data = new { @object = paymentObject },
        });

        var header = Sign(body, now);
        var evt = Webhook.ConstructEvent(body, header, Secret, toleranceSeconds: 300, nowUnix: now);

        evt.Type.Should().Be(WebhookEventType.PaymentCaptured);
        evt.AsPayment().Id.Should().Be("pi_1");
    }

    [Fact]
    public void Verify_ValidSignature_ReturnsTrue()
    {
        var now = 1_749_600_000L;
        var body = "{\"hello\":\"world\"}";
        var header = Sign(body, now);

        Webhook.Verify(body, header, Secret, nowUnix: now).Should().BeTrue();
    }

    [Fact]
    public void Verify_TamperedBody_ReturnsFalse()
    {
        var now = 1_749_600_000L;
        var body = "{\"hello\":\"world\"}";
        var header = Sign(body, now);

        Webhook.Verify(body + "x", header, Secret, nowUnix: now).Should().BeFalse();
    }

    [Fact]
    public void Verify_WrongSecret_ReturnsFalse()
    {
        var now = 1_749_600_000L;
        var body = "{\"hello\":\"world\"}";
        var header = Sign(body, now);

        Webhook.Verify(body, header, "wrong_secret", nowUnix: now).Should().BeFalse();
    }

    [Fact]
    public void Verify_StaleTimestamp_ReturnsFalse()
    {
        var now = 1_749_600_000L;
        var body = "{\"hello\":\"world\"}";
        var header = Sign(body, now);

        // 9999 seconds in the future — outside the 300s window
        Webhook.Verify(body, header, Secret, nowUnix: now + 9999).Should().BeFalse();
    }

    [Fact]
    public void Verify_MalformedHeader_ReturnsFalse()
    {
        var now = 1_749_600_000L;
        var body = "{\"hello\":\"world\"}";

        Webhook.Verify(body, "garbage_header", Secret, nowUnix: now).Should().BeFalse();
    }

    [Fact]
    public void ConstructEvent_BadSignature_Throws_WebhookSignatureException()
    {
        var act = () => Webhook.ConstructEvent("{\"a\":1}", "t=1,v1=badhex", Secret);
        act.Should().Throw<WebhookSignatureException>();
    }

    [Fact]
    public void ConstructEvent_InvalidJson_Throws_WebhookSignatureException()
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var body = "not_json";
        var header = Sign(body, now);

        var act = () => Webhook.ConstructEvent(body, header, Secret, nowUnix: now);
        act.Should().Throw<WebhookSignatureException>()
            .WithMessage("*not valid JSON*");
    }
}
