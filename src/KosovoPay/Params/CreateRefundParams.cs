using KosovoPay.Enums;

namespace KosovoPay.Params;

/// <summary>Parameters for creating a Refund.</summary>
public sealed class CreateRefundParams
{
    /// <summary>
    /// Creates refund params.
    /// </summary>
    /// <param name="payment">The payment ID to refund (required).</param>
    /// <param name="amount">Amount in minor units. Omit for a full refund.</param>
    /// <param name="reason">Optional reason code.</param>
    public CreateRefundParams(string payment, int? amount = null, RefundReason? reason = null)
    {
        if (string.IsNullOrEmpty(payment))
            throw new ArgumentException("payment id is required.", nameof(payment));
        if (amount is <= 0)
            throw new ArgumentException("amount, when given, must be a positive integer in minor units.", nameof(amount));

        Payment = payment;
        Amount = amount;
        Reason = reason;
    }

    /// <summary>Payment ID.</summary>
    public string Payment { get; }
    /// <summary>Amount in minor units, or <c>null</c> for full refund.</summary>
    public int? Amount { get; }
    /// <summary>Refund reason.</summary>
    public RefundReason? Reason { get; }

    internal Dictionary<string, object?> ToBody()
    {
        var body = new Dictionary<string, object?> { ["payment"] = Payment };
        if (Amount.HasValue)
            body["amount"] = Amount.Value;
        if (Reason.HasValue && EnumWireValues.RefundReasonValues.TryGetValue(Reason.Value, out var r))
            body["reason"] = r;
        return body;
    }
}
