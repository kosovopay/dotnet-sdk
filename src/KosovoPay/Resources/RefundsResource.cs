using KosovoPay.Dtos;
using KosovoPay.Http;
using KosovoPay.Params;

namespace KosovoPay.Resources;

/// <summary>Manages refunds against captured payments.</summary>
public sealed class RefundsResource : ResourceBase
{
    internal RefundsResource(KosovoPayHttpClient http) : base(http) { }

    /// <summary>Creates a new refund.</summary>
    /// <param name="params">Refund parameters.</param>
    /// <param name="idempotencyKey">Optional idempotency key. A GUID is auto-generated if omitted.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<Refund> CreateAsync(
        CreateRefundParams @params,
        string? idempotencyKey = null,
        CancellationToken cancellationToken = default) =>
        await PostAsync("/refunds", @params.ToBody(),
                        KosovoPayJsonContext.Default.Refund,
                        idempotencyKey, cancellationToken).ConfigureAwait(false);

    /// <summary>Retrieves a refund by ID.</summary>
    public async Task<Refund> RetrieveAsync(string id, CancellationToken cancellationToken = default) =>
        await GetAsync($"/refunds/{Uri.EscapeDataString(id)}", null,
                       KosovoPayJsonContext.Default.Refund, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Streams every matching refund across pages (cursor pagination).
    /// </summary>
    public IAsyncEnumerable<Refund> ListAsync(
        ListRefundsParams? @params = null,
        CancellationToken cancellationToken = default)
    {
        var query = @params?.ToQuery() ?? new Dictionary<string, object>();
        return PaginateAsync(
            "/refunds",
            query,
            KosovoPayJsonContext.Default.ListEnvelopeRefund,
            r => r.Id,
            cancellationToken);
    }
}
