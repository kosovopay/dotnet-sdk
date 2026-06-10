namespace KosovoPay.Dtos;

/// <summary>
/// Result of a client-side amount pre-check against a bank's capabilities.
/// <see cref="AmountValidator.Validate"/> returns this before any HTTP call.
/// </summary>
public sealed record AmountValidation(
    bool Valid,
    string? Code = null,
    string? Message = null,
    /// <summary>When code is <c>amount_step_invalid</c>, the nearest valid lower/upper amounts.</summary>
    (int Lower, int Upper)? NearestValid = null
);
