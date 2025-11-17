using Projects;
using AppHost.Extensions.Authentik;

var builder = DistributedApplication.CreateBuilder(args);

// var auth = builder.AddKeycloak("keyauth", port: 4000)
//     .WithDataVolume()
//     .WithContainerRuntimeArgs("--add-host=host.docker.internal:host-gateway");

var pg = builder.AddPostgres("postgres");
var authDb = pg.AddDatabase("auth-db");
var auth = builder.AddAuthentik("authentik", authDb);

var api = builder.AddProject<Ticksys_Api>("api")
    .WithExternalHttpEndpoints()
    .WithReference(auth.Resource.Server!.GetEndpoint("http"))
    .WaitFor(auth);

var ui = builder.AddViteApp("ui", "../ticksys-ui", "pnpm")
    .WithReference(auth.Resource.Server!.GetEndpoint("http"))
    .WithReference(api)
    .WaitFor(api);
if (ui.Resource.TryGetAnnotationsOfType<EndpointAnnotation>(out var endpoints))
{
    var endpoint = endpoints.FirstOrDefault(x => x.Name == "http");
    if(endpoint is not null)
    {
        endpoint.Port = 3000;
    }
}
builder.Build().Run();