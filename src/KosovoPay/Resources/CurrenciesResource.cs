using KosovoPay.Dtos;
using KosovoPay.Http;

namespace KosovoPay.Resources;

/// <summary>Retrieves supported currencies.</summary>
public sealed class CurrenciesResource : ResourceBase
{
    internal CurrenciesResource(KosovoPayHttpClient http) : base(http) { }

    /// <summary>Returns all currencies supported by KosovoPay.</summary>
    public async Task<ListEnvelope<Currency>> ListAsync(CancellationToken cancellationToken = default) =>
        await GetAsync("/currencies", null, KosovoPayJsonContext.Default.ListEnvelopeCurrency, cancellationToken)
              .ConfigureAwait(false);
}
