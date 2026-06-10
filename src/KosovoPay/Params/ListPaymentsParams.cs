using KosovoPay.Enums;

namespace KosovoPay.Params;

/// <summary>Query parameters for listing Payments.</summary>
public sealed class ListPaymentsParams
{
    /// <summary>Max items per page.</summary>
    public int? Limit { get; init; }

    /// <summary>Cursor: return items after this ID.</summary>
    public string? StartingAfter { get; init; }

    /// <summary>Cursor: return items before this ID.</summary>
    public string? EndingBefore { get; init; }

    /// <summary>Filter by status.</summary>
    public PaymentStatus? Status { get; init; }

    /// <summary>Filter by bank code.</summary>
    public BankCode? BankCode { get; init; }

    /// <summary>Filter by currency.</summary>
    public CurrencyCode? Currency { get; init; }

    /// <summary>Filter by merchant reference.</summary>
    public string? MerchantReference { get; init; }

    /// <summary>Filter: created at or after this Unix timestamp.</summary>
    public long? CreatedGte { get; init; }

    /// <summary>Filter: created at or before this Unix timestamp.</summary>
    public long? CreatedLte { get; init; }

    internal Dictionary<string, object> ToQuery()
    {
        var q = new Dictionary<string, object>();
        if (Limit.HasValue) q["limit"] = Limit.Value;
        if (StartingAfter is not null) q["starting_after"] = StartingAfter;
        if (EndingBefore is not null) q["ending_before"] = EndingBefore;
        if (Status.HasValue) q["status"] = GetPaymentStatusWireValue(Status.Value);
        if (BankCode.HasValue) q["bank_code"] = GetBankCodeWireValue(BankCode.Value);
        if (Currency.HasValue && EnumWireValues.CurrencyCodeValues.TryGetValue(Currency.Value, out var cur))
            q["currency"] = cur;
        if (MerchantReference is not null) q["merchant_reference"] = MerchantReference;
        if (CreatedGte.HasValue) q["created[gte]"] = CreatedGte.Value;
        if (CreatedLte.HasValue) q["created[lte]"] = CreatedLte.Value;
        return q;
    }

    private static string GetPaymentStatusWireValue(PaymentStatus s) => s switch
    {
        PaymentStatus.Pending => "pending",
        PaymentStatus.Authorized => "authorized",
        PaymentStatus.Captured => "captured",
        PaymentStatus.PartiallyRefunded => "partially_refunded",
        PaymentStatus.Refunded => "refunded",
        PaymentStatus.Failed => "failed",
        PaymentStatus.Canceled => "canceled",
        _ => s.ToString().ToLowerInvariant(),
    };

    private static string GetBankCodeWireValue(Enums.BankCode code) => code switch
    {
        Enums.BankCode.Procredit => "procredit",
        Enums.BankCode.Procard => "procard",
        Enums.BankCode.Onefor => "onefor",
        _ => code.ToString().ToLowerInvariant(),
    };
}
