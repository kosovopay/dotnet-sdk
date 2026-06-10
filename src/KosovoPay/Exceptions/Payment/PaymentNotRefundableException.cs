namespace KosovoPay.Exceptions.Payment;

/// <summary>The payment cannot be refunded (wrong status or bank doesn't support refunds).</summary>
public sealed class PaymentNotRefundableException : PaymentException
{
    /// <inheritdoc cref="PaymentException"/>
    public PaymentNotRefundableException(
        string message,
        string? errorCode = null,
        string? errorType = null,
        string? param = null,
        string? requestId = null,
        string? docUrl = null,
        int statusCode = 0)
        : base(message, errorCode, errorType, param, requestId, docUrl, statusCode) { }
}
