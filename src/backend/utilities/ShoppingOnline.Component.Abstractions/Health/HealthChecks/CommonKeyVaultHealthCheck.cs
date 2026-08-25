using ShoppingOnline.Component.Abstractions.Extensions;
using ShoppingOnline.Component.Abstractions.Settings;

namespace ShoppingOnline.Component.Abstractions.Health.HealthChecks;

/// <summary>
/// Health check for validating the Key Vault settings.
/// </summary>
internal class CommonKeyVaultHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;
    private readonly string[] _sectionNames;

    /// <summary>
    ///  Constructor for the CommonKeyVaultHealthCheck class.
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="sectionNames"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public CommonKeyVaultHealthCheck(IConfiguration configuration, params string[] sectionNames)
    {
        if (!sectionNames.Any())
        {
            throw new ArgumentNullException(nameof(sectionNames));
        }

        _configuration = configuration;
        _sectionNames = sectionNames;
    }

    /// <summary>
    /// Asynchronously checks the health of the Key Vault settings.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var result = ValidationConfigurations();
        var status = result.SelectMany(s => s.Value as Dictionary<string, bool>).Any(w => !w.Value)
            ? HealthStatus.Unhealthy
            : HealthStatus.Healthy;

        return Task.FromResult(new HealthCheckResult(status, "Key vault settings.", null, result));
    }

    /// <summary>
    /// Validates the Key Vault settings and returns a dictionary with the results.
    /// </summary>
    /// <returns></returns>
    private Dictionary<string, object> ValidationConfigurations()
    {
        var values = new Dictionary<string, object>();
        foreach (var name in _sectionNames)
        {
            var settings = _configuration.GetOptions<AzureVaultSettings>(name);
            bool hasSettings = !settings.IsNullable() && !settings.IsValuePropertiesStringTypeEmpty();
            values.Add(name, new Dictionary<string, bool> { { "HasConfiguration", hasSettings } });
        }

        return values;
    }
}