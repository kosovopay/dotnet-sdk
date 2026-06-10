using KosovoPay.Dtos;

namespace KosovoPay;

/// <summary>
/// Integer-only money helpers. All amounts are minor units; rates are decimal strings.
/// No <see cref="float"/> or <see cref="double"/> is used for storage — only a final
/// rounding to <see cref="int"/>.
/// </summary>
public static class Money
{
    /// <summary>
    /// Formats minor units as a human-readable string.
    /// Example: <c>(4990, 2, "€")</c> → <c>"€49.90"</c>.
    /// </summary>
    /// <param name="amount">Amount in minor units (may be negative).</param>
    /// <param name="decimals">Number of decimal places.</param>
    /// <param name="symbol">Optional currency symbol prefix.</param>
    public static string Format(int amount, int decimals, string symbol = "")
    {
        var negative = amount < 0;
        var abs = Math.Abs(amount);
        var divisor = (int)Math.Pow(10, Math.Max(0, decimals));
        var major = abs / divisor;
        var minor = abs % divisor;

        var formatted = decimals > 0
            ? $"{major}.{minor.ToString().PadLeft(decimals, '0')}"
            : major.ToString();

        return (negative ? "-" : "") + symbol + formatted;
    }

    /// <summary>
    /// Formats minor units using a <see cref="Currency"/> DTO's symbol and decimals.
    /// </summary>
    public static string FormatCurrency(int amount, Currency currency) =>
        Format(amount, currency.Decimals, currency.Symbol ?? string.Empty);

    /// <summary>
    /// Converts minor units by a decimal rate string, returning minor units.
    /// Uses <see cref="decimal"/> arithmetic to avoid float drift.
    /// </summary>
    /// <param name="amount">Amount in minor units.</param>
    /// <param name="rate">Decimal rate as a string, e.g. <c>"0.9234"</c>.</param>
    public static int Convert(int amount, string rate)
    {
        if (!decimal.TryParse(rate, System.Globalization.NumberStyles.Any,
                              System.Globalization.CultureInfo.InvariantCulture, out var r))
        {
            return 0;
        }

        return (int)Math.Round((decimal)amount * r, MidpointRounding.AwayFromZero);
    }
}
