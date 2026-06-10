using Flurl.Http;
using KosovoPay.Dtos;
using KosovoPay.Enums;
using KosovoPay.Http;
using KosovoPay.Resources;

namespace KosovoPay;

/// <summary>
/// The KosovoPay API client. Construct with a secret key, then reach resources via
/// the typed properties.
///
/// <code>
/// var client = new KosovoPayClient("sk_test_…");
/// var payment = await client.Payments.CreateAsync(params);
/// </code>
/// </summary>
public sealed class KosovoPayClient : IDisposable
{
    /// <summary>SDK version string.</summary>
    public const string Version = "1.0.0";

    private readonly KosovoPayHttpClient _http;

    /// <summary>Payment operations.</summary>
    public PaymentsResource Payments { get; }

    /// <summary>Refund operations.</summary>
    public RefundsResource Refunds { get; }

    /// <summary>Bank listing and retrieval.</summary>
    public BanksResource Banks { get; }

    /// <summary>Currency listing.</summary>
    public CurrenciesResource Currencies { get; }

    /// <summary>FX rate retrieval.</summary>
    public RatesResource Rates { get; }

    /// <summary>Webhook endpoint management.</summary>
    public WebhookEndpointsResource WebhookEndpoints { get; }

    /// <summary>
    /// Creates a new <see cref="KosovoPayClient"/> with default options.
    /// </summary>
    /// <param name="apiKey">Your secret API key (<c>sk_live_…</c> or <c>sk_test_…</c>).</param>
    public KosovoPayClient(string apiKey)
        : this(new KosovoPayOptions { ApiKey = apiKey }) { }

    /// <summary>
    /// Creates a new <see cref="KosovoPayClient"/> with custom options.
    /// </summary>
    /// <param name="options">Client configuration options.</param>
    public KosovoPayClient(KosovoPayOptions options)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        _http = new KosovoPayHttpClient(options);
        Payments = new PaymentsResource(_http);
        Refunds = new RefundsResource(_http);
        Banks = new BanksResource(_http);
        Currencies = new CurrenciesResource(_http);
        Rates = new RatesResource(_http);
        WebhookEndpoints = new WebhookEndpointsResource(_http);
    }

    /// <summary>
    /// Creates a client backed by a custom <see cref="IFlurlClient"/>. Use this in
    /// tests to inject Flurl's <c>HttpTest</c> mock client.
    /// </summary>
    /// <param name="flurlClient">The Flurl client to use for all requests.</param>
    /// <param name="options">Options (API key, version headers, retry). Base URL / timeout are controlled by <paramref name="flurlClient"/>.</param>
    public KosovoPayClient(IFlurlClient flurlClient, KosovoPayOptions options)
    {
        if (flurlClient is null) throw new ArgumentNullException(nameof(flurlClient));
        if (options is null) throw new ArgumentNullException(nameof(options));
        _http = new KosovoPayHttpClient(flurlClient, options);
        Payments = new PaymentsResource(_http);
        Refunds = new RefundsResource(_http);
        Banks = new BanksResource(_http);
        Currencies = new CurrenciesResource(_http);
        Rates = new RatesResource(_http);
        WebhookEndpoints = new WebhookEndpointsResource(_http);
    }

    /// <summary>Identifies the API key — its team, mode, and usable banks.</summary>
    public Task<Me> MeAsync(CancellationToken cancellationToken = default) =>
        new MeResource(_http).RetrieveAsync(cancellationToken);

    /// <summary>
    /// Client-side amount pre-check against a bank's live capabilities. Catches
    /// <c>amount_below_minimum</c> / <c>amount_step_invalid</c> before a round-trip.
    /// </summary>
    public async Task<AmountValidation> ValidateAmountAsync(
        int amount,
        CurrencyCode currency,
        BankCode bank,
        CancellationToken cancellationToken = default)
    {
        var bankDto = await Banks.RetrieveAsync(bank, cancellationToken).ConfigureAwait(false);
        return AmountValidator.Validate(bankDto, amount, currency);
    }

    /// <inheritdoc />
    public void Dispose() => _http.Dispose();
}
