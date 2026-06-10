using KosovoPay.Dtos;
using KosovoPay.Enums;
using KosovoPay.Http;

namespace KosovoPay.Resources;

/// <summary>Retrieves FX rates.</summary>
public sealed class RatesResource : ResourceBase
{
    internal RatesResource(KosovoPayHttpClient http) : base(http) { }

    /// <summary>Returns the exchange rate from <paramref name="from"/> to <paramref name="to"/>.</summary>
    public async Task<Rate> RetrieveAsync(
        CurrencyCode from,
        CurrencyCode to,
        CancellationToken cancellationToken = default)
    {
        var fromWire = EnumWireValues.CurrencyCodeValues.TryGetValue(from, out var fv) ? fv : from.ToString();
        var toWire = EnumWireValues.CurrencyCodeValues.TryGetValue(to, out var tv) ? tv : to.ToString();

        return await GetAsync("/rates",
            new Dictionary<string, object> { ["from"] = fromWire, ["to"] = toWire },
            KosovoPayJsonContext.Default.Rate,
            cancellationToken).ConfigureAwait(false);
    }
}
