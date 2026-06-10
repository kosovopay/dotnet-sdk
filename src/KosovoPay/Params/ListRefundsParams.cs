namespace KosovoPay.Params;

/// <summary>Query parameters for listing Refunds.</summary>
public sealed class ListRefundsParams
{
    /// <summary>Filter refunds to a specific payment ID.</summary>
    public string? Payment { get; init; }

    /// <summary>Max items per page.</summary>
    public int? Limit { get; init; }

    /// <summary>Cursor: return items after this ID.</summary>
    public string? StartingAfter { get; init; }

    /// <summary>Cursor: return items before this ID.</summary>
    public string? EndingBefore { get; init; }

    internal Dictionary<string, object> ToQuery()
    {
        var q = new Dictionary<string, object>();
        if (Payment is not null) q["payment"] = Payment;
        if (Limit.HasValue) q["limit"] = Limit.Value;
        if (StartingAfter is not null) q["starting_after"] = StartingAfter;
        if (EndingBefore is not null) q["ending_before"] = EndingBefore;
        return q;
    }
}
