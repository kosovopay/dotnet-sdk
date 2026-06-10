namespace KosovoPay.Exceptions;

/// <summary>Raised when a webhook signature cannot be verified — the payload should be rejected.</summary>
public sealed class WebhookSignatureException : KosovoPayException
{
    /// <inheritdoc cref="KosovoPayException"/>
    public WebhookSignatureException(string message)
        : base(message) { }
}
