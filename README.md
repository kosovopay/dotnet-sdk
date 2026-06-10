# KosovoPay .NET SDK

Official .NET SDK for the [KosovoPay](https://kosovo.sh) payment API.
Supports **.NET 8** and **.NET 9**.

## Installation

```bash
dotnet add package KosovoPay
```

## Quick start

```csharp
using KosovoPay;
using KosovoPay.Enums;
using KosovoPay.Params;

var client = new KosovoPayClient("sk_test_…");
```

---

## Hosted checkout

```csharp
var payment = await client.Payments.CreateAsync(new CreatePaymentParams(
    amount: 4990,              // €49.90 in minor units
    currency: CurrencyCode.Eur,
    successUrl: "https://shop.example/ok",
    cancelUrl: "https://shop.example/cancel",
    merchantReference: "order-42"
));

// Redirect the customer
Console.WriteLine(payment.HostedUrl);
```

## Direct (bank-select) checkout

```csharp
var payment = await client.Payments.CreateAsync(new CreatePaymentParams(
    amount: 4990,
    currency: CurrencyCode.Eur,
    successUrl: "https://shop.example/ok",
    mode: CheckoutMode.Direct,
    bankCode: BankCode.Onefor
));

Console.WriteLine(payment.RedirectUrl);
```

---

## Retrieve a payment

```csharp
var payment = await client.Payments.RetrieveAsync("pi_…");
Console.WriteLine(payment.Status); // PaymentStatus.Captured
```

## Cancel a payment

```csharp
var canceled = await client.Payments.CancelAsync("pi_…");
```

---

## Refunds

```csharp
// Full refund
var refund = await client.Refunds.CreateAsync(new CreateRefundParams(payment: "pi_…"));

// Partial refund
var partial = await client.Refunds.CreateAsync(new CreateRefundParams(
    payment: "pi_…",
    amount: 1000,
    reason: RefundReason.RequestedByCustomer
));

// Retrieve
var fetched = await client.Refunds.RetrieveAsync("re_…");
```

---

## Cursor pagination

`Payments.ListAsync()` and `Refunds.ListAsync()` return an `IAsyncEnumerable<T>` that
auto-fetches pages using `starting_after` until `has_more` is `false`.

```csharp
await foreach (var payment in client.Payments.ListAsync(new ListPaymentsParams
{
    Status = PaymentStatus.Captured,
    CreatedGte = DateTimeOffset.UtcNow.AddDays(-7).ToUnixTimeSeconds(),
}))
{
    Console.WriteLine($"{payment.Id} — {payment.Amount}");
}
```

---

## Webhook verification

Always verify webhooks using the **raw request body** (never re-encoded):

```csharp
// ASP.NET Core minimal API example
app.MapPost("/webhooks", async (HttpContext ctx) =>
{
    using var reader = new StreamReader(ctx.Request.Body);
    var payload = await reader.ReadToEndAsync();
    var signature = ctx.Request.Headers["Kosovopay-Signature"].ToString();

    KosovoPay.Dtos.Event evt;
    try
    {
        evt = Webhook.ConstructEvent(payload, signature, "whsec_…");
    }
    catch (WebhookSignatureException)
    {
        return Results.Unauthorized();
    }

    switch (evt.Type)
    {
        case WebhookEventType.PaymentCaptured:
            var payment = evt.AsPayment();
            Console.WriteLine($"Captured: {payment.Id}");
            break;
        case WebhookEventType.RefundSucceeded:
            var refund = evt.AsRefund();
            Console.WriteLine($"Refunded: {refund.Id}");
            break;
    }

    return Results.Ok();
});
```

---

## Webhook endpoints

```csharp
// Register
var endpoint = await client.WebhookEndpoints.CreateAsync(new CreateWebhookEndpointParams(
    url: "https://shop.example/webhooks",
    enabledEvents: new[] { WebhookEventType.PaymentCaptured, WebhookEventType.RefundSucceeded }
));
Console.WriteLine(endpoint.Secret); // store this securely — shown once

// List
var list = await client.WebhookEndpoints.ListAsync();

// Delete
await client.WebhookEndpoints.DeleteAsync("we_…");

// Rotate secret
var rotated = await client.WebhookEndpoints.RotateSecretAsync("we_…");
```

---

## Error handling

```csharp
using KosovoPay.Exceptions;
using KosovoPay.Exceptions.Payment;

try
{
    var payment = await client.Payments.CreateAsync(params);
}
catch (AmountBelowMinimumException ex)
{
    Console.WriteLine($"Amount too low: {ex.Message}");
}
catch (RateLimitException ex)
{
    var wait = ex.RetryAfter ?? 60;
    Console.WriteLine($"Rate-limited, retry in {wait}s");
}
catch (ValidationException ex)
{
    Console.WriteLine($"Validation error on field '{ex.Param}': {ex.Message}");
}
catch (AuthenticationException)
{
    Console.WriteLine("Invalid API key");
}
catch (KosovoPayException ex)
{
    Console.WriteLine($"API error [{ex.ErrorCode}] — request {ex.RequestId}: {ex.Message}");
    // ex.DocUrl links to the documentation
}
```

**Exception hierarchy:**
- `KosovoPayException` (base)
  - `AuthenticationException` — 401
  - `PermissionException` — 403
  - `ValidationException` — 422
  - `IdempotencyException` — 409
  - `RateLimitException` — 429 — has `RetryAfter` seconds
  - `PaymentException` — payment-domain errors
    - `AmountBelowMinimumException` — `amount_below_minimum`
    - `AmountStepInvalidException` — `amount_step_invalid`
    - `BankNotEnabledException` — `bank_not_enabled`
    - `BankUnreachableException` — `bank_unreachable`
    - `PaymentNotCancelableException` — `payment_not_cancelable`
    - `PaymentNotRefundableException` — `payment_not_refundable`
    - `RefundExceedsRemainingException` — `refund_exceeds_remaining`
    - `PartialRefundUnsupportedException` — `partial_refund_unsupported`
  - `ApiException` — catch-all server errors
  - `WebhookSignatureException` — invalid webhook signature

---

## Money helpers

```csharp
// Format minor units
Money.Format(4990, 2, "€");    // "€49.90"
Money.Format(500, 0, "¥");     // "¥500"

// Format using a Currency DTO
Money.FormatCurrency(4990, currency); // uses currency.Symbol and currency.Decimals

// FX conversion (decimal arithmetic, no float drift)
Money.Convert(4990, "0.9234");  // 4608
```

## Client-side amount validation

Pre-check amounts locally without a round-trip:

```csharp
var bank = await client.Banks.RetrieveAsync(BankCode.Onefor);
var result = AmountValidator.Validate(bank, 173, CurrencyCode.Eur);

if (!result.Valid)
{
    Console.WriteLine(result.Code);    // "amount_step_invalid"
    Console.WriteLine(result.Message);
    if (result.NearestValid is var (lower, upper))
        Console.WriteLine($"Try {lower} or {upper}");
}
```

---

## Advanced configuration

```csharp
var client = new KosovoPayClient(new KosovoPayOptions
{
    ApiKey = "sk_test_…",
    BaseUrl = "https://api.kosovo.sh",       // default
    ApiVersion = "2026-06-01",               // default
    RequestTimeoutSeconds = 30,              // default
    MaxRetries = 3,                          // default
});
```

Retry policy: **exponential backoff with jitter** via Polly, applied to:
- Network / transport errors
- HTTP 429 (Too Many Requests)
- HTTP 5xx on safe (GET) requests; mutating (POST) requests must carry an `Idempotency-Key` (auto-generated)

---

## License

**KosovoPay License 1.0** — free to use, including commercially, at no charge.
Modifying, forking, redistributing, or reverse-engineering the SDK is **not**
permitted; it is maintained solely by KosovoPay. See [LICENSE](LICENSE).
