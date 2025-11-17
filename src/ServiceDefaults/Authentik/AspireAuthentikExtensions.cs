using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting.Authentik;

public static class AspireAuthentikExtensions
{
    private const string AuthentikBackchannel = nameof(AuthentikBackchannel);
    
    /// <summary>
    /// Adds Authentik OpenID Connect authentication to the application.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder" /> to add services to.</param>
    /// <param name="serviceName">The name of the service used to resolve the Authentik server URL.</param>
    /// <param name="realm">The realm of the Authentik server to connect to.</param>
    /// <remarks>
    /// The <paramref name="serviceName"/> is used to resolve the Authentik server URL and is combined with the realm to form the authority URL.
    /// For example, if <paramref name="serviceName"/> is "Authentik" and <paramref name="realm"/> is "myrealm", the authority URL will be "https+http://Authentik/realms/myrealm".
    /// </remarks>
    public static AuthenticationBuilder AddAuthentikOpenIdConnect(this AuthenticationBuilder builder, string serviceName, string realm)
        => AddAuthentikOpenIdConnect(builder, serviceName, realm, OpenIdConnectDefaults.AuthenticationScheme, null);

    /// <summary>
    /// Adds Authentik OpenID Connect authentication to the application.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder" /> to add services to.</param>
    /// <param name="serviceName">The name of the service used to resolve the Authentik server URL.</param>
    /// <param name="realm">The realm of the Authentik server to connect to.</param>
    /// <param name="authenticationScheme">The OpenID Connect authentication scheme name. Default is "OpenIdConnect".</param>
    /// <remarks>
    /// The <paramref name="serviceName"/> is used to resolve the Authentik server URL and is combined with the realm to form the authority URL.
    /// For example, if <paramref name="serviceName"/> is "Authentik" and <paramref name="realm"/> is "myrealm", the authority URL will be "https+http://Authentik/realms/myrealm".
    /// </remarks>
    public static AuthenticationBuilder AddAuthentikOpenIdConnect(this AuthenticationBuilder builder, string serviceName, string realm, string authenticationScheme)
        => AddAuthentikOpenIdConnect(builder, serviceName, realm, authenticationScheme, null);

    /// <summary>
    /// Adds Authentik OpenID Connect authentication to the application.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder" /> to add services to.</param>
    /// <param name="serviceName">The name of the service used to resolve the Authentik server URL.</param>
    /// <param name="realm">The realm of the Authentik server to connect to.</param>
    /// <param name="configureOptions">An action to configure the <see cref="OpenIdConnectOptions"/>.</param>
    /// <remarks>
    /// The <paramref name="serviceName"/> is used to resolve the Authentik server URL and is combined with the realm to form the authority URL.
    /// For example, if <paramref name="serviceName"/> is "Authentik" and <paramref name="realm"/> is "myrealm", the authority URL will be "https+http://Authentik/realms/myrealm".
    /// </remarks>
    public static AuthenticationBuilder AddAuthentikOpenIdConnect(this AuthenticationBuilder builder, string serviceName, string realm, Action<OpenIdConnectOptions>? configureOptions)
        => AddAuthentikOpenIdConnect(builder, serviceName, realm, OpenIdConnectDefaults.AuthenticationScheme, configureOptions);
    public static AuthenticationBuilder AddAuthentikOpenIdConnect(
        this AuthenticationBuilder builder,
        string serviceName,
        string realm,
        string authenticationScheme,
        Action<OpenIdConnectOptions>? configureOptions)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(serviceName);
        ArgumentException.ThrowIfNullOrEmpty(realm);
        ArgumentException.ThrowIfNullOrEmpty(authenticationScheme);

        builder.AddOpenIdConnect(authenticationScheme, options => { });

        builder.Services.AddHttpClient(AuthentikBackchannel);

        builder.Services
            .AddOptions<OpenIdConnectOptions>(authenticationScheme)
            .Configure<IConfiguration, IHttpClientFactory, IHostEnvironment>((options, configuration, httpClientFactory, hostEnvironment) =>
            {
                options.Backchannel = httpClientFactory.CreateClient(AuthentikBackchannel);
                options.Authority = GetAuthorityUri(serviceName, realm);

                configureOptions?.Invoke(options);
            });

        return builder;
    }
    private static string GetAuthorityUri(
        string serviceName,
        string realm)
    {
        return $"https+http://{serviceName}/application/o/{realm}/";
    }
}