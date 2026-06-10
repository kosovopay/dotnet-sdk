using KosovoPay.Enums;

namespace KosovoPay.Params;

/// <summary>Parameters for creating a new Payment.</summary>
public sealed class CreatePaymentParams
{
    /// <summary>
    /// Creates the params, validating required rules up-front.
    /// </summary>
    /// <param name="amount">Amount in minor units (must be &gt; 0).</param>
    /// <param name="currency">Currency code.</param>
    /// <param name="successUrl">URL to redirect to on success (http/https).</param>
    /// <param name="mode">Checkout mode. Defaults to <see cref="CheckoutMode.Hosted"/>.</param>
    /// <param name="bankCode">Required when <paramref name="mode"/> is <see cref="CheckoutMode.Direct"/>.</param>
    /// <param name="cancelUrl">Optional URL for canceled payment.</param>
    /// <param name="failUrl">Optional URL for failed payment.</param>
    /// <param name="description">Human-readable payment description.</param>
    /// <param name="lineItems">Optional list of line items.</param>
    /// <param name="metadata">Arbitrary key/value metadata (max 50 keys).</param>
    /// <param name="expiresAt">Unix timestamp of payment expiry.</param>
    /// <param name="merchantReference">Your order/reference identifier.</param>
    public CreatePaymentParams(
        int amount,
        CurrencyCode currency,
        string successUrl,
        CheckoutMode mode = CheckoutMode.Hosted,
        BankCode? bankCode = null,
        string? cancelUrl = null,
        string? failUrl = null,
        string? description = null,
        IReadOnlyList<LineItem>? lineItems = null,
        IReadOnlyDictionary<string, string>? metadata = null,
        long? expiresAt = null,
        string? merchantReference = null)
    {
        if (amount <= 0)
            throw new ArgumentException("amount must be a positive integer in minor units.", nameof(amount));

        if (mode == CheckoutMode.Direct && bankCode is null)
            throw new ArgumentException("bankCode is required for direct checkout mode.", nameof(bankCode));

        if (mode == CheckoutMode.Hosted && bankCode is not null)
            throw new ArgumentException("bankCode must be omitted for hosted checkout mode.", nameof(bankCode));

        AssertUrl(successUrl, nameof(successUrl));
        if (cancelUrl is not null) AssertUrl(cancelUrl, nameof(cancelUrl));
        if (failUrl is not null) AssertUrl(failUrl, nameof(failUrl));

        Amount = amount;
        Currency = currency;
        SuccessUrl = successUrl;
        Mode = mode;
        BankCode = bankCode;
        CancelUrl = cancelUrl;
        FailUrl = failUrl;
        Description = description;
        LineItems = lineItems;
        Metadata = metadata;
        ExpiresAt = expiresAt;
        MerchantReference = merchantReference;
    }

    /// <summary>Amount in minor units.</summary>
    public int Amount { get; }
    /// <summary>Currency code.</summary>
    public CurrencyCode Currency { get; }
    /// <summary>Redirect URL on success.</summary>
    public string SuccessUrl { get; }
    /// <summary>Checkout mode.</summary>
    public CheckoutMode Mode { get; }
    /// <summary>Bank code (direct mode only).</summary>
    public BankCode? BankCode { get; }
    /// <summary>Redirect URL on cancellation.</summary>
    public string? CancelUrl { get; }
    /// <summary>Redirect URL on failure.</summary>
    public string? FailUrl { get; }
    /// <summary>Human-readable description.</summary>
    public string? Description { get; }
    /// <summary>Line items.</summary>
    public IReadOnlyList<LineItem>? LineItems { get; }
    /// <summary>Arbitrary metadata.</summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; }
    /// <summary>Unix timestamp when the payment expires.</summary>
    public long? ExpiresAt { get; }
    /// <summary>Your order/reference identifier.</summary>
    public string? MerchantReference { get; }

    internal Dictionary<string, object?> ToBody()
    {
        var body = new Dictionary<string, object?>
        {
            ["amount"] = Amount,
            ["currency"] = GetCurrencyWireValue(Currency),
            ["mode"] = GetCheckoutModeWireValue(Mode),
            ["success_url"] = SuccessUrl,
        };

        if (BankCode.HasValue)
            body["bank_code"] = GetBankCodeWireValue(BankCode.Value);
        if (CancelUrl is not null)
            body["cancel_url"] = CancelUrl;
        if (FailUrl is not null)
            body["fail_url"] = FailUrl;
        if (Description is not null)
            body["description"] = Description;
        if (LineItems is { Count: > 0 })
            body["line_items"] = LineItems;
        if (Metadata is { Count: > 0 })
            body["metadata"] = Metadata;
        if (ExpiresAt.HasValue)
            body["expires_at"] = ExpiresAt.Value;
        if (MerchantReference is not null)
            body["merchant_reference"] = MerchantReference;

        return body;
    }

    private static void AssertUrl(string url, string paramName)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != "http" && uri.Scheme != "https"))
        {
            throw new ArgumentException($"{paramName} must be an http or https URL.", paramName);
        }
    }

    // Wire value helpers — avoids reflection on hot path
    private static string GetCurrencyWireValue(CurrencyCode code) =>
        EnumWireValues.CurrencyCodeValues.TryGetValue(code, out var v) ? v : code.ToString();

    private static string GetCheckoutModeWireValue(CheckoutMode mode) => mode switch
    {
        CheckoutMode.Hosted => "hosted",
        CheckoutMode.Direct => "direct",
        _ => mode.ToString().ToLowerInvariant(),
    };

    private static string GetBankCodeWireValue(Enums.BankCode code) => code switch
    {
        Enums.BankCode.Procredit => "procredit",
        Enums.BankCode.Procard => "procard",
        Enums.BankCode.Onefor => "onefor",
        _ => code.ToString().ToLowerInvariant(),
    };
}
