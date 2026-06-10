namespace KosovoPay.Exceptions.Payment;

/// <summary>The refund amount exceeds the remaining refundable amount on the payment.</summary>
public sealed class RefundExceedsRemainingException : PaymentException
{
    /// <inheritdoc cref="PaymentException"/>
    public RefundExceedsRemainingException(
        string message,
        string? errorCode = null,
        string? errorType = null,
        string? param = null,
        string? requestId = null,
        string? docUrl = null,
        int statusCode = 0)
        : base(message, errorCode, errorType, param, requestId, docUrl, statusCode) { }
}
