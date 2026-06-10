namespace KosovoPay.Dtos;

/// <summary>
/// Result of a client-side amount pre-check against a bank's capabilities.
/// <see cref="AmountValidator.Validate"/> returns this before any HTTP call.
/// </summary>
/// <param name="Valid">Whether the amount is valid.</param>
/// <param name="Code">Error code when invalid (e.g. <c>amount_below_minimum</c>, <c>amount_step_invalid</c>).</param>
/// <param name="Message">Human-readable description of the validation failure.</param>
/// <param name="NearestValid">When code is <c>amount_step_invalid</c>, the nearest valid lower/upper amounts.</param>
public sealed record AmountValidation(
    bool Valid,
    string? Code = null,
    string? Message = null,
    (int Lower, int Upper)? NearestValid = null
);
