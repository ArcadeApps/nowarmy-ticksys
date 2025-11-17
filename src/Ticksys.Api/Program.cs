using System.Text.Json.Serialization;
using Microsoft.Extensions.Hosting.Authentik;

namespace Ticksys.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);
        builder.AddServiceDefaults();
        builder.Services.AddAuthentication()
            .AddAuthentikOpenIdConnect(
                serviceName: "authentik",
                realm: "ticksys",
                configureOptions: options =>
                {
                    options.ClientId = "D9256wxFeM5vbKA07KNVPWzTLxOwpienxFR4j1Xk";
                    options.RequireHttpsMetadata = false;
                });

        builder.Services.AddAuthorizationBuilder();
        var app = builder.Build();
        
        
        
        app.MapDefaultEndpoints();

        app.Run();
    }
}
