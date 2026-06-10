using KosovoPay.Exceptions.Payment;

namespace KosovoPay.Exceptions;

/// <summary>
/// Maps a server error envelope to the matching typed exception.
/// Resolution order: exact <c>code</c> → 429/rate-limit → <c>type</c> family → <see cref="ApiException"/>.
/// An unrecognised code never crashes — it falls back to its type family so a server
/// that adds a new code stays usable by an old SDK.
/// </summary>
internal static class ErrorMapper
{
    private static readonly Dictionary<string, Func<string, string?, string?, string?, string?, string?, int, KosovoPayException>> ByCode =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["amount_below_minimum"] = (m, c, t, p, r, d, s) => new AmountBelowMinimumException(m, c, t, p, r, d, s),
            ["amount_step_invalid"] = (m, c, t, p, r, d, s) => new AmountStepInvalidException(m, c, t, p, r, d, s),
            ["bank_not_enabled"] = (m, c, t, p, r, d, s) => new BankNotEnabledException(m, c, t, p, r, d, s),
            ["bank_unreachable"] = (m, c, t, p, r, d, s) => new BankUnreachableException(m, c, t, p, r, d, s),
            ["payment_not_cancelable"] = (m, c, t, p, r, d, s) => new PaymentNotCancelableException(m, c, t, p, r, d, s),
            ["payment_not_refundable"] = (m, c, t, p, r, d, s) => new PaymentNotRefundableException(m, c, t, p, r, d, s),
            ["refund_exceeds_remaining"] = (m, c, t, p, r, d, s) => new RefundExceedsRemainingException(m, c, t, p, r, d, s),
            ["partial_refund_unsupported"] = (m, c, t, p, r, d, s) => new PartialRefundUnsupportedException(m, c, t, p, r, d, s),
        };

    private static readonly Dictionary<string, Func<string, string?, string?, string?, string?, string?, int, KosovoPayException>> ByType =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["authentication_error"] = (m, c, t, p, r, d, s) => new AuthenticationException(m, c, t, p, r, d, s),
            ["permission_error"] = (m, c, t, p, r, d, s) => new PermissionException(m, c, t, p, r, d, s),
            ["validation_error"] = (m, c, t, p, r, d, s) => new ValidationException(m, c, t, p, r, d, s),
            ["idempotency_error"] = (m, c, t, p, r, d, s) => new IdempotencyException(m, c, t, p, r, d, s),
            ["payment_error"] = (m, c, t, p, r, d, s) => new PaymentException(m, c, t, p, r, d, s),
            ["api_error"] = (m, c, t, p, r, d, s) => new ApiException(m, c, t, p, r, d, s),
        };

    /// <summary>
    /// Creates the most specific <see cref="KosovoPayException"/> for the given response.
    /// </summary>
    internal static KosovoPayException Make(
        string? message,
        string? code,
        string? type,
        string? param,
        string? requestId,
        string? docUrl,
        int status,
        int? retryAfter)
    {
        var msg = string.IsNullOrEmpty(message) ? "The request failed." : message;

        if (type == "rate_limit_error" || status == 429)
        {
            return new RateLimitException(msg, retryAfter, code, type, param, requestId, docUrl, status);
        }

        if (code is not null && ByCode.TryGetValue(code, out var codeFactory))
        {
            return codeFactory(msg, code, type, param, requestId, docUrl, status);
        }

        if (type is not null && ByType.TryGetValue(type, out var typeFactory))
        {
            return typeFactory(msg, code, type, param, requestId, docUrl, status);
        }

        return new ApiException(msg, code, type, param, requestId, docUrl, status);
    }
}
