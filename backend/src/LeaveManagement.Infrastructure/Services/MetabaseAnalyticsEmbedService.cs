using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using LeaveManagement.Application.Abstractions;
using LeaveManagement.Application.Reports;
using LeaveManagement.Infrastructure.Analytics;
using Microsoft.Extensions.Options;

namespace LeaveManagement.Infrastructure.Services;

public sealed class MetabaseAnalyticsEmbedService : IAnalyticsEmbedService
{
    private readonly MetabaseOptions _options;

    public MetabaseAnalyticsEmbedService(IOptions<MetabaseOptions> options)
    {
        _options = options.Value;
    }

    public Task<AnalyticsEmbedResult> BuildDashboardEmbedUrlAsync(
        IReadOnlyCollection<string> roles,
        int? dashboardId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_options.Enabled)
        {
            throw new InvalidOperationException("Metabase integration is disabled.");
        }

        if (string.IsNullOrWhiteSpace(_options.SiteUrl) || string.IsNullOrWhiteSpace(_options.EmbedSecret))
        {
            throw new InvalidOperationException("Metabase configuration is incomplete.");
        }

        var resolvedDashboardId = dashboardId.GetValueOrDefault(ResolveDashboardIdByRole(roles));
        if (resolvedDashboardId <= 0)
        {
            throw new InvalidOperationException("A valid dashboard id is required for analytics embedding.");
        }

        var exp = DateTimeOffset.UtcNow.AddMinutes(Math.Max(1, _options.TokenTtlMinutes)).ToUnixTimeSeconds();

        var payload = new Dictionary<string, object?>
        {
            ["resource"] = new Dictionary<string, object?> { ["dashboard"] = resolvedDashboardId },
            ["params"] = new Dictionary<string, object?>(),
            ["exp"] = exp
        };

        var token = BuildJwt(payload, _options.EmbedSecret);
        var url = $"{_options.SiteUrl.TrimEnd('/')}/embed/dashboard/{token}#bordered=true&titled=true";

        return Task.FromResult(new AnalyticsEmbedResult(url, resolvedDashboardId, exp));
    }

    private int ResolveDashboardIdByRole(IReadOnlyCollection<string> roles)
    {
        if (roles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
        {
            return _options.AdminDashboardId > 0 ? _options.AdminDashboardId : _options.DefaultDashboardId;
        }

        if (roles.Any(r => r.Equals("Hr", StringComparison.OrdinalIgnoreCase)))
        {
            return _options.HrDashboardId > 0 ? _options.HrDashboardId : _options.DefaultDashboardId;
        }

        return _options.UserDashboardId > 0 ? _options.UserDashboardId : _options.DefaultDashboardId;
    }

    private static string BuildJwt(IReadOnlyDictionary<string, object?> payload, string secret)
    {
        var headerJson = JsonSerializer.Serialize(new Dictionary<string, object?>
        {
            ["alg"] = "HS256",
            ["typ"] = "JWT"
        });

        var payloadJson = JsonSerializer.Serialize(payload);

        var header = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
        var body = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));
        var unsignedToken = $"{header}.{body}";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(unsignedToken));
        var signature = Base64UrlEncode(signatureBytes);

        return $"{unsignedToken}.{signature}";
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
