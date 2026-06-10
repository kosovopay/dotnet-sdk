using KosovoPay.Dtos;
using KosovoPay.Http;

namespace KosovoPay.Resources;

/// <summary>Identifies the authenticated API key: team, mode, and enabled banks.</summary>
public sealed class MeResource : ResourceBase
{
    internal MeResource(KosovoPayHttpClient http) : base(http) { }

    /// <summary>Returns identity information for the current API key.</summary>
    public Task<Me> RetrieveAsync(CancellationToken cancellationToken = default) =>
        GetAsync("/me", null, KosovoPayJsonContext.Default.Me, cancellationToken);
}
