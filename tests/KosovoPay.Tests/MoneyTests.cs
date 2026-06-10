using FluentAssertions;
using KosovoPay;
using KosovoPay.Dtos;
using KosovoPay.Enums;
using Xunit;

namespace KosovoPay.Tests;

/// <summary>
/// Tests for <see cref="Money"/> formatting/conversion and <see cref="AmountValidator"/>.
/// </summary>
public sealed class MoneyTests
{
    // ── Money.Format ──────────────────────────────────────────────────────

    [Theory]
    [InlineData(4990, 2, "€", "€49.90")]
    [InlineData(5, 2, "€", "€0.05")]
    [InlineData(-1250, 2, "$", "-$12.50")]
    [InlineData(500, 0, "¥", "¥500")]
    [InlineData(100, 2, "", "1.00")]
    public void Format_ReturnsExpected(int amount, int decimals, string symbol, string expected)
    {
        Money.Format(amount, decimals, symbol).Should().Be(expected);
    }

    // ── Money.Convert ─────────────────────────────────────────────────────

    [Theory]
    [InlineData(4990, "0.9234", 4608)]
    [InlineData(10000, "1.0", 10000)]
    [InlineData(100, "0.0", 0)]
    [InlineData(1, "0.5", 1)] // rounds half-up
    public void Convert_UsesDecimalArithmetic(int amount, string rate, int expected)
    {
        Money.Convert(amount, rate).Should().Be(expected);
    }

    // ── AmountValidator ───────────────────────────────────────────────────

    private static Bank BuildBank(int minAmount = 150, int step = 50) =>
        new Bank(
            Code: BankCode.Onefor,
            DisplayName: "Onefor",
            LogoUrl: null,
            Enabled: true,
            Modes: new[] { BankMode.Test },
            Capabilities: new BankCapabilities(
                Currencies: new[] { CurrencyCode.Eur },
                MinAmount: minAmount,
                AmountStep: step,
                Refunds: new RefundCapability(Supported: true, Partial: false)
            )
        );

    [Fact]
    public void Validate_Valid_Amount_Returns_True()
    {
        var result = AmountValidator.Validate(BuildBank(), 200, CurrencyCode.Eur);
        result.Valid.Should().BeTrue();
    }

    [Fact]
    public void Validate_BelowMinimum_Returns_AmountBelowMinimum()
    {
        var result = AmountValidator.Validate(BuildBank(), 100, CurrencyCode.Eur);
        result.Valid.Should().BeFalse();
        result.Code.Should().Be("amount_below_minimum");
    }

    [Fact]
    public void Validate_BadStep_Returns_StepInvalid_With_NearestValid()
    {
        var result = AmountValidator.Validate(BuildBank(), 173, CurrencyCode.Eur);
        result.Valid.Should().BeFalse();
        result.Code.Should().Be("amount_step_invalid");
        result.NearestValid.Should().Be((150, 200));
    }

    [Fact]
    public void Validate_WrongCurrency_Returns_CurrencyNotSupported()
    {
        var result = AmountValidator.Validate(BuildBank(), 200, CurrencyCode.Usd);
        result.Valid.Should().BeFalse();
        result.Code.Should().Be("currency_not_supported");
    }
}
