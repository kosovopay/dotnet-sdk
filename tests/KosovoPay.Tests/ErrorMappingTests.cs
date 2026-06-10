using FluentAssertions;
using Flurl.Http.Testing;
using KosovoPay;
using KosovoPay.Enums;
using KosovoPay.Exceptions;
using KosovoPay.Exceptions.Payment;
using KosovoPay.Params;
using Xunit;

namespace KosovoPay.Tests;

/// <summary>
/// Verifies that HTTP error responses are mapped to the correct typed exceptions.
/// </summary>
public sealed class ErrorMappingTests
{
    [Fact]
    public async Task ValidationError_422_Throws_ValidationException_With_Envelope_Fields()
    {
        using var http = new HttpTest();
        http.RespondWith(TestHelpers.ErrorBody("validation_error", "invalid_request", 422), 422);

        var client = new KosovoPayClient(TestHelpers.DefaultOptions);

        var ex = await Assert.ThrowsAsync<ValidationException>(
            () => client.Rates.RetrieveAsync(CurrencyCode.Usd, CurrencyCode.Eur));

        ex.ErrorCode.Should().Be("invalid_request");
        ex.Param.Should().Be("x");
        ex.RequestId.Should().Be("req_1");
        ex.StatusCode.Should().Be(422);
    }

    [Fact]
    public async Task AuthenticationError_401_Throws_AuthenticationException()
    {
        using var http = new HttpTest();
        http.RespondWith(TestHelpers.ErrorBody("authentication_error", "invalid_key", 401), 401);

        var client = new KosovoPayClient(TestHelpers.DefaultOptions);

        await Assert.ThrowsAsync<AuthenticationException>(
            () => client.Rates.RetrieveAsync(CurrencyCode.Usd, CurrencyCode.Eur));
    }

    [Fact]
    public async Task PermissionError_403_Throws_PermissionException()
    {
        using var http = new HttpTest();
        http.RespondWith(TestHelpers.ErrorBody("permission_error", "insufficient_permissions", 403), 403);

        var client = new KosovoPayClient(TestHelpers.DefaultOptions);

        await Assert.ThrowsAsync<PermissionException>(
            () => client.Rates.RetrieveAsync(CurrencyCode.Usd, CurrencyCode.Eur));
    }

    [Fact]
    public async Task RateLimit_429_Throws_RateLimitException_With_RetryAfter()
    {
        using var http = new HttpTest();
        http.RespondWith(
            TestHelpers.ErrorBody("rate_limit_error", "rate_limited", 429),
            429,
            headers: new { Retry_After = "7" });

        var client = new KosovoPayClient(TestHelpers.DefaultOptions);

        var ex = await Assert.ThrowsAsync<RateLimitException>(
            () => client.Rates.RetrieveAsync(CurrencyCode.Usd, CurrencyCode.Eur));

        ex.RetryAfter.Should().Be(7);
        ex.StatusCode.Should().Be(429);
    }

    [Fact]
    public async Task PaymentErrorCode_MapsTo_SpecificSubclass()
    {
        using var http = new HttpTest();
        http.RespondWith(TestHelpers.ErrorBody("payment_error", "partial_refund_unsupported", 422), 422);

        var client = new KosovoPayClient(TestHelpers.DefaultOptions);

        await Assert.ThrowsAsync<PartialRefundUnsupportedException>(
            () => client.Refunds.CreateAsync(new CreateRefundParams(payment: "pi_1", amount: 500)));
    }

    [Fact]
    public async Task UnknownCode_WithKnownType_Falls_Back_To_TypeFamily()
    {
        using var http = new HttpTest();
        // Unknown code + known type → still a ValidationException, not a crash
        http.RespondWith(TestHelpers.ErrorBody("validation_error", "some_future_unknown_code", 422), 422);

        var client = new KosovoPayClient(TestHelpers.DefaultOptions);

        var ex = await Assert.ThrowsAsync<ValidationException>(
            () => client.Rates.RetrieveAsync(CurrencyCode.Usd, CurrencyCode.Eur));

        ex.ErrorCode.Should().Be("some_future_unknown_code");
    }

    [Fact]
    public async Task UnknownCode_UnknownType_Falls_Back_To_ApiException()
    {
        using var http = new HttpTest();
        http.RespondWith(TestHelpers.ErrorBody("totally_unknown_type", "totally_unknown_code", 500), 500);

        var client = new KosovoPayClient(TestHelpers.DefaultOptions);

        await Assert.ThrowsAsync<ApiException>(
            () => client.Rates.RetrieveAsync(CurrencyCode.Usd, CurrencyCode.Eur));
    }
}
