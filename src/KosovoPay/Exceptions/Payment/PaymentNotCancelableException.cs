namespace KosovoPay.Exceptions.Payment;

/// <summary>The payment is in a terminal state and cannot be canceled.</summary>
public sealed class PaymentNotCancelableException : PaymentException
{
    /// <inheritdoc cref="PaymentException"/>
    public PaymentNotCancelableException(
        string message,
        string? errorCode = null,
        string? errorType = null,
        string? param = null,
        string? requestId = null,
        string? docUrl = null,
        int statusCode = 0)
        : base(message, errorCode, errorType, param, requestId, docUrl, statusCode) { }
}
