using System.Text.Json.Serialization;

namespace Ticksys.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);
        builder.AddServiceDefaults();

        builder.Services
            .AddAuthentication()
            .AddKeycloakJwtBearer("keyauth", "ticksys", options => { options.RequireHttpsMetadata = false; });
        builder.Services.AddAuthorization();

        var app = builder.Build();
        app.MapDefaultEndpoints();


        await app.RunAsync();
    }
}