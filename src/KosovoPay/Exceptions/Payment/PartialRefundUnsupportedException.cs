namespace KosovoPay.Exceptions.Payment;

/// <summary>The bank does not support partial refunds for this payment.</summary>
public sealed class PartialRefundUnsupportedException : PaymentException
{
    /// <inheritdoc cref="PaymentException"/>
    public PartialRefundUnsupportedException(
        string message,
        string? errorCode = null,
        string? errorType = null,
        string? param = null,
        string? requestId = null,
        string? docUrl = null,
        int statusCode = 0)
        : base(message, errorCode, errorType, param, requestId, docUrl, statusCode) { }
}
