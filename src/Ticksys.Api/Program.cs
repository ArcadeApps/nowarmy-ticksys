using System.Text.Json.Serialization;

namespace Ticksys.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);
        builder.AddServiceDefaults();
        builder.Services.AddAuthentication()
            .AddKeycloakJwtBearer(
                serviceName: "keyauth",
                realm: "ticksys",
                configureOptions: options =>
                {
                    options.RequireHttpsMetadata = false;
                });

        builder.Services.AddAuthorizationBuilder();
        var app = builder.Build();
        app.MapDefaultEndpoints();

        app.Run();
    }
}
