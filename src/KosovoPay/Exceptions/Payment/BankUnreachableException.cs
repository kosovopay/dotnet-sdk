namespace KosovoPay.Exceptions.Payment;

/// <summary>The bank's API could not be reached to process the payment.</summary>
public sealed class BankUnreachableException : PaymentException
{
    /// <inheritdoc cref="PaymentException"/>
    public BankUnreachableException(
        string message,
        string? errorCode = null,
        string? errorType = null,
        string? param = null,
        string? requestId = null,
        string? docUrl = null,
        int statusCode = 0)
        : base(message, errorCode, errorType, param, requestId, docUrl, statusCode) { }
}
