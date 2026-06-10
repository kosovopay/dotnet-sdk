namespace KosovoPay.Exceptions.Payment;

/// <summary>The payment amount is not a valid step for the bank.</summary>
public sealed class AmountStepInvalidException : PaymentException
{
    /// <inheritdoc cref="PaymentException"/>
    public AmountStepInvalidException(
        string message,
        string? errorCode = null,
        string? errorType = null,
        string? param = null,
        string? requestId = null,
        string? docUrl = null,
        int statusCode = 0)
        : base(message, errorCode, errorType, param, requestId, docUrl, statusCode) { }
}
