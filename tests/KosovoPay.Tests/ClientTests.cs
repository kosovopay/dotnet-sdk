using System.Text.Json;
using FluentAssertions;
using Flurl.Http.Testing;
using KosovoPay;
using KosovoPay.Dtos;
using KosovoPay.Enums;
using KosovoPay.Params;
using Xunit;

namespace KosovoPay.Tests;

/// <summary>
/// End-to-end resource tests using Flurl's HttpTest to intercept requests.
/// </summary>
public sealed class ClientTests
{
    // ── Me ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task MeAsync_Returns_TypedMe()
    {
        using var http = new HttpTest();

        var meJson = JsonSerializer.Serialize(new
        {
            @object = "me",
            team = new { id = "team_1", name = "Acme", logo_url = (string?)null },
            mode = "test",
            key_prefix = "sk_test_ab",
            enabled_banks = new[] { "onefor" },
            default_currency = "EUR",
        });

        http.RespondWith(meJson, 200);

        var client = new KosovoPayClient(TestHelpers.DefaultOptions);
        var me = await client.MeAsync();

        me.Should().NotBeNull();
        me.Team.Name.Should().Be("Acme");
        me.Mode.Should().Be(BankMode.Test);
        me.EnabledBanks.Should().ContainSingle(b => b == BankCode.Onefor);
        me.DefaultCurrency.Should().Be(CurrencyCode.Eur);
    }

    // ── Banks ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Banks_ListAsync_Returns_TypedCollection()
    {
        using var http = new HttpTest();

        var json = JsonSerializer.Serialize(new
        {
            @object = "list",
            data = new[]
            {
                new
                {
                    @object = "bank",
                    code = "onefor",
                    display_name = "Onefor",
                    logo_url = (string?)null,
                    enabled = true,
                    modes = new[] { "test" },
                    capabilities = new
                    {
                        currencies = new[] { "EUR" },
                        min_amount = 150,
                        amount_step = 1,
                        refunds = new { supported = true, partial = false },
                    },
                },
            },
            has_more = false,
            url = "/api/sdk/banks",
        });

        http.RespondWith(json, 200);

        var client = new KosovoPayClient(TestHelpers.DefaultOptions);
        var banks = await client.Banks.ListAsync();

        banks.Data.Should().HaveCount(1);
        banks.Data[0].Code.Should().Be(BankCode.Onefor);
        banks.Data[0].Capabilities.MinAmount.Should().Be(150);
        banks.Data[0].Capabilities.Currencies.Should().Contain(CurrencyCode.Eur);
    }

    // ── Payments ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Payments_CreateAsync_Returns_TypedPayment()
    {
        using var http = new HttpTest();

        var json = JsonSerializer.Serialize(new
        {
            @object = "payment",
            id = "pi_1",
            status = "pending",
            mode = "test",
            amount = 4990,
            amount_captured = 0,
            amount_refunded = 0,
            currency = "EUR",
            bank_code = "onefor",
            merchant_reference = "O-1",
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
            checkout_mode = "direct",
            hosted_url = (string?)null,
            redirect_url = "https://bank/redirect",
        });

        http.RespondWith(json, 201);

        var client = new KosovoPayClient(TestHelpers.DefaultOptions);
        var payment = await client.Payments.CreateAsync(new CreatePaymentParams(
            amount: 4990,
            currency: CurrencyCode.Eur,
            successUrl: "https://shop.test/ok",
            mode: CheckoutMode.Direct,
            bankCode: BankCode.Onefor,
            merchantReference: "O-1"));

        payment.Id.Should().Be("pi_1");
        payment.Status.Should().Be(PaymentStatus.Pending);
        payment.Amount.Should().Be(4990);
        payment.CheckoutMode.Should().Be(CheckoutMode.Direct);
        payment.RedirectUrl.Should().Be("https://bank/redirect");
    }

    [Fact]
    public async Task Payments_CreateAsync_Sends_IdempotencyKey()
    {
        using var http = new HttpTest();

        http.RespondWith(JsonSerializer.Serialize(TestHelpers.PaymentRow("pi_1")), 201);

        var client = new KosovoPayClient(TestHelpers.DefaultOptions);
        await client.Payments.CreateAsync(new CreatePaymentParams(
            amount: 200,
            currency: CurrencyCode.Eur,
            successUrl: "https://x.test/ok"));

        http.ShouldHaveMadeACall()
            .WithHeader("Idempotency-Key");
    }

    [Fact]
    public async Task Payments_CreateAsync_Uses_CallerSupplied_IdempotencyKey()
    {
        using var http = new HttpTest();

        http.RespondWith(JsonSerializer.Serialize(TestHelpers.PaymentRow("pi_1")), 201);

        var client = new KosovoPayClient(TestHelpers.DefaultOptions);
        await client.Payments.CreateAsync(
            new CreatePaymentParams(200, CurrencyCode.Eur, "https://x.test/ok"),
            idempotencyKey: "my-idempotency-key");

        http.ShouldHaveMadeACall()
            .WithHeader("Idempotency-Key", "my-idempotency-key");
    }

    [Fact]
    public async Task Payments_RetrieveAsync_Hits_Correct_Path()
    {
        using var http = new HttpTest();

        http.RespondWith(JsonSerializer.Serialize(TestHelpers.PaymentRow("pi_abc")), 200);

        var client = new KosovoPayClient(TestHelpers.DefaultOptions);
        var payment = await client.Payments.RetrieveAsync("pi_abc");

        payment.Id.Should().Be("pi_abc");
        http.ShouldHaveCalled("*/payments/pi_abc").WithVerb(HttpMethod.Get);
    }

    [Fact]
    public async Task Payments_CancelAsync_Posts_To_Cancel_Endpoint()
    {
        using var http = new HttpTest();

        http.RespondWith(JsonSerializer.Serialize(TestHelpers.PaymentRow("pi_1")), 200);

        var client = new KosovoPayClient(TestHelpers.DefaultOptions);
        var payment = await client.Payments.CancelAsync("pi_1");

        payment.Id.Should().Be("pi_1");
        http.ShouldHaveCalled("*/payments/pi_1/cancel").WithVerb(HttpMethod.Post);
    }

    // ── Refunds ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Refunds_CreateAsync_Returns_TypedRefund()
    {
        using var http = new HttpTest();

        var json = JsonSerializer.Serialize(new
        {
            @object = "refund",
            id = "re_1",
            payment = "pi_1",
            amount = 1000,
            status = "succeeded",
            reason = "requested_by_customer",
            failure_reason = (string?)null,
            created = 1_749_601_000L,
            succeeded_at = 1_749_601_002L,
        });

        http.RespondWith(json, 201);

        var client = new KosovoPayClient(TestHelpers.DefaultOptions);
        var refund = await client.Refunds.CreateAsync(new CreateRefundParams(
            payment: "pi_1",
            amount: 1000,
            reason: RefundReason.RequestedByCustomer));

        refund.Id.Should().Be("re_1");
        refund.Amount.Should().Be(1000);
        refund.Reason.Should().Be(RefundReason.RequestedByCustomer);
        refund.Status.Should().Be(RefundStatus.Succeeded);
    }

    // ── Webhooks ────────────────────────────────────────────────────────────

    [Fact]
    public async Task WebhookEndpoints_CreateAsync_Returns_EndpointWithSecret()
    {
        using var http = new HttpTest();

        var json = JsonSerializer.Serialize(new
        {
            @object = "webhook_endpoint",
            id = "we_1",
            url = "https://shop.test/hooks",
            description = (string?)null,
            enabled_events = new[] { "payment.captured" },
            status = "enabled",
            mode = "test",
            created = 1_749_600_000L,
            secret = "whsec_abc",
        });

        http.RespondWith(json, 201);

        var client = new KosovoPayClient(TestHelpers.DefaultOptions);
        var endpoint = await client.WebhookEndpoints.CreateAsync(new CreateWebhookEndpointParams(
            url: "https://shop.test/hooks",
            enabledEvents: new[] { WebhookEventType.PaymentCaptured }));

        endpoint.Id.Should().Be("we_1");
        endpoint.Secret.Should().Be("whsec_abc");
        endpoint.EnabledEvents.Should().ContainSingle(e => e == WebhookEventType.PaymentCaptured);
    }

    // ── Param validation ────────────────────────────────────────────────────

    [Fact]
    public void CreatePaymentParams_Rejects_ZeroAmount()
    {
        var act = () => new CreatePaymentParams(
            amount: 0,
            currency: CurrencyCode.Eur,
            successUrl: "https://x/ok");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreatePaymentParams_Direct_Requires_BankCode()
    {
        var act = () => new CreatePaymentParams(
            amount: 100,
            currency: CurrencyCode.Eur,
            successUrl: "https://x/ok",
            mode: CheckoutMode.Direct);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreatePaymentParams_Hosted_Rejects_BankCode()
    {
        var act = () => new CreatePaymentParams(
            amount: 100,
            currency: CurrencyCode.Eur,
            successUrl: "https://x/ok",
            mode: CheckoutMode.Hosted,
            bankCode: BankCode.Onefor);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateRefundParams_Rejects_EmptyPaymentId()
    {
        var act = () => new CreateRefundParams(payment: "");
        act.Should().Throw<ArgumentException>();
    }
}
