namespace KosovoPay.Exceptions.Payment;

/// <summary>The selected bank is not enabled for this merchant account.</summary>
public sealed class BankNotEnabledException : PaymentException
{
    /// <inheritdoc cref="PaymentException"/>
    public BankNotEnabledException(
        string message,
        string? errorCode = null,
        string? errorType = null,
        string? param = null,
        string? requestId = null,
        string? docUrl = null,
        int statusCode = 0)
        : base(message, errorCode, errorType, param, requestId, docUrl, statusCode) { }
}
