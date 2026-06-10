using KosovoPay.Dtos;
using KosovoPay.Http;
using KosovoPay.Params;

namespace KosovoPay.Resources;

/// <summary>
/// Manages payments: hosted and direct checkout, retrieve, list (auto-paginating),
/// timeline, and cancel.
/// </summary>
public sealed class PaymentsResource : ResourceBase
{
    internal PaymentsResource(KosovoPayHttpClient http) : base(http) { }

    /// <summary>Creates a new hosted or direct checkout payment.</summary>
    /// <param name="params">Payment parameters.</param>
    /// <param name="idempotencyKey">
    /// Optional caller-supplied idempotency key. A GUID is auto-generated if omitted.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<Payment> CreateAsync(
        CreatePaymentParams @params,
        string? idempotencyKey = null,
        CancellationToken cancellationToken = default) =>
        await PostAsync("/payments", @params.ToBody(),
                        KosovoPayJsonContext.Default.Payment,
                        idempotencyKey, cancellationToken).ConfigureAwait(false);

    /// <summary>Retrieves a payment by ID.</summary>
    public async Task<Payment> RetrieveAsync(string id, CancellationToken cancellationToken = default) =>
        await GetAsync($"/payments/{Uri.EscapeDataString(id)}", null,
                       KosovoPayJsonContext.Default.Payment, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Streams every matching payment across pages (newest-first cursor pagination).
    /// The first page is fetched lazily when enumeration begins.
    /// </summary>
    public IAsyncEnumerable<Payment> ListAsync(
        ListPaymentsParams? @params = null,
        CancellationToken cancellationToken = default)
    {
        var query = @params?.ToQuery() ?? new Dictionary<string, object>();
        return PaginateAsync(
            "/payments",
            query,
            KosovoPayJsonContext.Default.ListEnvelopePayment,
            p => p.Id,
            cancellationToken);
    }

    /// <summary>Returns the timeline events for a payment.</summary>
    public async Task<ListEnvelope<TimelineEvent>> TimelineAsync(
        string id,
        CancellationToken cancellationToken = default) =>
        await GetAsync($"/payments/{Uri.EscapeDataString(id)}/timeline", null,
                       KosovoPayJsonContext.Default.ListEnvelopeTimelineEvent, cancellationToken).ConfigureAwait(false);

    /// <summary>Cancels a payment that has not yet been captured.</summary>
    /// <param name="id">Payment ID.</param>
    /// <param name="reason">Optional cancellation reason.</param>
    /// <param name="idempotencyKey">Optional idempotency key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<Payment> CancelAsync(
        string id,
        string? reason = null,
        string? idempotencyKey = null,
        CancellationToken cancellationToken = default)
    {
        var body = reason is null
            ? new Dictionary<string, object?>()
            : new Dictionary<string, object?> { ["reason"] = reason };

        return await PostAsync($"/payments/{Uri.EscapeDataString(id)}/cancel",
                               body,
                               KosovoPayJsonContext.Default.Payment,
                               idempotencyKey, cancellationToken).ConfigureAwait(false);
    }
}
