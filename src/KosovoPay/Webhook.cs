using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using KosovoPay.Dtos;
using KosovoPay.Exceptions;

namespace KosovoPay;

/// <summary>
/// Verifies inbound webhook signatures and constructs a typed <see cref="Event"/>.
///
/// Header format: <c>Kosovopay-Signature: t=&lt;unix&gt;,v1=&lt;hex hmac-sha256&gt;</c>
///
/// Signed payload string: <c>"{t}.{raw_body}"</c> — always verify against the raw body,
/// never a re-encoded representation.
/// </summary>
public static class Webhook
{
    /// <summary>The name of the signature header.</summary>
    public const string SignatureHeader = "Kosovopay-Signature";

    private const int DefaultToleranceSeconds = 300;

    /// <summary>
    /// Verifies the signature and deserialises the raw body into a typed <see cref="Event"/>.
    /// </summary>
    /// <param name="payload">The raw (unmodified) request body string.</param>
    /// <param name="signatureHeader">Value of the <c>Kosovopay-Signature</c> header.</param>
    /// <param name="secret">Your webhook signing secret (<c>whsec_…</c>).</param>
    /// <param name="toleranceSeconds">Allowed clock skew in seconds. Defaults to 300 (5 minutes).</param>
    /// <param name="nowUnix">
    /// Override the current Unix time (for deterministic testing). Defaults to the real clock.
    /// </param>
    /// <returns>The deserialised webhook event.</returns>
    /// <exception cref="WebhookSignatureException">
    /// Thrown when the signature is invalid, stale, or the payload is not valid JSON.
    /// </exception>
    public static Event ConstructEvent(
        string payload,
        string signatureHeader,
        string secret,
        int toleranceSeconds = DefaultToleranceSeconds,
        long? nowUnix = null)
    {
        if (!Verify(payload, signatureHeader, secret, toleranceSeconds, nowUnix))
            throw new WebhookSignatureException("Webhook signature verification failed.");

        try
        {
            return JsonSerializer.Deserialize(payload, KosovoPayJsonContext.Default.Event)
                ?? throw new WebhookSignatureException("Webhook payload deserialised to null.");
        }
        catch (JsonException ex)
        {
            throw new WebhookSignatureException($"Webhook payload is not valid JSON: {ex.Message}");
        }
    }

    /// <summary>
    /// Constant-time signature verification with a timestamp-tolerance (replay) window.
    /// </summary>
    /// <param name="payload">The raw request body.</param>
    /// <param name="signatureHeader">Value of the <c>Kosovopay-Signature</c> header.</param>
    /// <param name="secret">Your webhook signing secret.</param>
    /// <param name="toleranceSeconds">Allowed clock skew in seconds. Defaults to 300.</param>
    /// <param name="nowUnix">
    /// Override current Unix time for testing. Defaults to the real clock.
    /// </param>
    /// <returns><c>true</c> if the signature is valid and within the tolerance window.</returns>
    public static bool Verify(
        string payload,
        string signatureHeader,
        string secret,
        int toleranceSeconds = DefaultToleranceSeconds,
        long? nowUnix = null)
    {
        var parts = ParseHeader(signatureHeader);

        if (!parts.TryGetValue("t", out var tStr) || !long.TryParse(tStr, out var timestamp))
            return false;

        if (!parts.TryGetValue("v1", out var givenHex) || string.IsNullOrEmpty(givenHex))
            return false;

        if (timestamp <= 0)
            return false;

        var now = nowUnix ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (Math.Abs(now - timestamp) > toleranceSeconds)
            return false;

        var signedPayload = $"{timestamp}.{payload}";
        var secretBytes = Encoding.UTF8.GetBytes(secret);
        var payloadBytes = Encoding.UTF8.GetBytes(signedPayload);

        var expected = HMACSHA256.HashData(secretBytes, payloadBytes);
        var expectedHex = Convert.ToHexString(expected).ToLowerInvariant();

        // Constant-time comparison to prevent timing attacks
        var expectedBytes = Encoding.ASCII.GetBytes(expectedHex);
        var givenBytes = Encoding.ASCII.GetBytes(givenHex);

        return CryptographicOperations.FixedTimeEquals(expectedBytes, givenBytes);
    }

    private static Dictionary<string, string> ParseHeader(string header)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var piece in header.Split(','))
        {
            var trimmed = piece.Trim();
            var eq = trimmed.IndexOf('=', StringComparison.Ordinal);
            if (eq > 0)
                result[trimmed[..eq]] = trimmed[(eq + 1)..];
        }
        return result;
    }
}
