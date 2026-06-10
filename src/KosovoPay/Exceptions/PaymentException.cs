namespace KosovoPay.Exceptions;

/// <summary>A payment-specific error. Specialised subclasses cover common codes.</summary>
public class PaymentException : KosovoPayException
{
    /// <inheritdoc cref="KosovoPayException"/>
    public PaymentException(
        string message,
        string? errorCode = null,
        string? errorType = null,
        string? param = null,
        string? requestId = null,
        string? docUrl = null,
        int statusCode = 0)
        : base(message, errorCode, errorType, param, requestId, docUrl, statusCode) { }
}
