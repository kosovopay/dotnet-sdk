namespace KosovoPay.Exceptions.Payment;

/// <summary>The payment amount is below the bank's minimum.</summary>
public sealed class AmountBelowMinimumException : PaymentException
{
    /// <inheritdoc cref="PaymentException"/>
    public AmountBelowMinimumException(
        string message,
        string? errorCode = null,
        string? errorType = null,
        string? param = null,
        string? requestId = null,
        string? docUrl = null,
        int statusCode = 0)
        : base(message, errorCode, errorType, param, requestId, docUrl, statusCode) { }
}
