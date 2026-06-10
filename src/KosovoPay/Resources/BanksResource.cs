using KosovoPay.Dtos;
using KosovoPay.Enums;
using KosovoPay.Http;

namespace KosovoPay.Resources;

/// <summary>Retrieves bank configuration and capabilities.</summary>
public sealed class BanksResource : ResourceBase
{
    internal BanksResource(KosovoPayHttpClient http) : base(http) { }

    /// <summary>Returns all supported banks.</summary>
    public async Task<ListEnvelope<Bank>> ListAsync(CancellationToken cancellationToken = default) =>
        await GetAsync("/banks", null, KosovoPayJsonContext.Default.ListEnvelopeBank, cancellationToken)
              .ConfigureAwait(false);

    /// <summary>Returns a specific bank by its code.</summary>
    public async Task<Bank> RetrieveAsync(BankCode code, CancellationToken cancellationToken = default)
    {
        var wireCode = EnumWireValues.BankCodeValues.TryGetValue(code, out var v) ? v : code.ToString();
        return await GetAsync($"/banks/{Uri.EscapeDataString(wireCode)}", null,
                              KosovoPayJsonContext.Default.Bank, cancellationToken)
                     .ConfigureAwait(false);
    }
}
