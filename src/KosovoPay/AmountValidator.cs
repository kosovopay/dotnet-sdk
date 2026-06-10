using KosovoPay.Dtos;
using KosovoPay.Enums;

namespace KosovoPay;

/// <summary>
/// Mirrors the server's bank min/step checks so callers can catch
/// <c>amount_below_minimum</c> / <c>amount_step_invalid</c> locally before a round-trip.
/// Always reads the bank's live capabilities — never hardcodes a minimum or step.
/// </summary>
public static class AmountValidator
{
    /// <summary>
    /// Validates <paramref name="amount"/> against <paramref name="bank"/>'s capabilities
    /// for <paramref name="currency"/>.
    /// </summary>
    /// <param name="bank">The bank DTO (from <c>BanksResource.RetrieveAsync</c>).</param>
    /// <param name="amount">Amount in minor units.</param>
    /// <param name="currency">The currency to charge.</param>
    /// <returns>An <see cref="AmountValidation"/> describing validity.</returns>
    public static AmountValidation Validate(Bank bank, int amount, CurrencyCode currency)
    {
        var caps = bank.Capabilities;

        if (caps.Currencies.Count > 0 && !caps.Currencies.Contains(currency))
        {
            var wireVal = EnumWireValues.CurrencyCodeValues.TryGetValue(currency, out var cv) ? cv : currency.ToString();
            return new AmountValidation(false, "currency_not_supported",
                $"{bank.DisplayName} does not support {wireVal}.");
        }

        if (amount < caps.MinAmount)
        {
            return new AmountValidation(false, "amount_below_minimum",
                $"Amount is below the {bank.DisplayName} minimum of {caps.MinAmount}.");
        }

        var step = Math.Max(1, caps.AmountStep);
        if (amount % step != 0)
        {
            var lower = (amount / step) * step;
            var upper = lower + step;
            return new AmountValidation(false, "amount_step_invalid",
                $"{bank.DisplayName} requires amounts in steps of {step}.",
                NearestValid: (lower, upper));
        }

        return new AmountValidation(true);
    }
}
