namespace AppHost.Extensions.Authentik;

public static class AuthentikResourceBuilderExtensions
{
    private const string ManagementEndpointName = "management";
    private const int DefaultContainerPort = 9000;

    private static IResourceBuilder<T> WithPostgresData<T>(
        this IResourceBuilder<T> builder,
        IResourceBuilder<PostgresDatabaseResource> database) where T : AuthentikResource
    {
        var pgServer = database.Resource.Parent;
        var ep = pgServer.GetEndpoint("tcp");

        var dbName = database.Resource.Name;

        builder
            .WithEnvironment("AUTHENTIK_POSTGRESQL__HOST", ep.Property(EndpointProperty.Host))
            .WithEnvironment("AUTHENTIK_POSTGRESQL__NAME", dbName)
            .WaitFor(database);

        return builder;
    }

    private static ReferenceExpression ToRef(ParameterResource? value)
    {
        return value is null ? ReferenceExpression.Create($"postgres") : ReferenceExpression.Create($"{value}");
    }

    private static IResourceBuilder<T> WithPostgres<T>(
        this IResourceBuilder<T> builder,
        IResourceBuilder<PostgresDatabaseResource> database,
        IResourceBuilder<ParameterResource>? username = null,
        IResourceBuilder<ParameterResource>? password = null) where T : AuthentikResource
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(database);

        return WithPostgresData(builder, database)
            .WithEnvironment("AUTHENTIK_POSTGRESQL__USER", ToRef(username?.Resource ??
                                                                 database.Resource.Parent.UserNameParameter))
            .WithEnvironment("AUTHENTIK_POSTGRESQL__PASSWORD", ToRef(password?.Resource ??
                                                                     database.Resource.Parent.PasswordParameter));
    }

    private static IResourceBuilder<AuthentikWorkerResource> AddAuthentikWorker(
        this IResourceBuilder<AuthentikServerResource> builder,
        string name, IResourceBuilder<PostgresDatabaseResource> database)
    {
        
        AuthentikWorkerResource workerResource = new($"{name}-worker", builder.Resource, builder.Resource.AdminPasswordParameter);

        var worker = builder.ApplicationBuilder
            .AddResource(workerResource)
            .WithImage("goauthentik/server")
            .WithImageRegistry("ghcr.io")
            .WithImageTag("2025.10.0")
            .WithArgs("worker")
            .WithEnvironment("AUTHENTIK_SECRET_KEY", workerResource.AdminPasswordParameter).WithPostgres(database);
        return worker;
    }
    
    public static IResourceBuilder<AuthentikServerResource> AddAuthentik(
        this IDistributedApplicationBuilder builder,
        string name,
        IResourceBuilder<PostgresDatabaseResource> database,
        int? port = null,
        IResourceBuilder<ParameterResource>? adminUsername = null,
        IResourceBuilder<ParameterResource>? adminPassword = null)
    {
        var secret = adminPassword?.Resource ?? ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter(builder, "secret-key", false);
        AuthentikServerResource serverResource = new(name, adminUsername?.Resource, secret);


        var authentik = builder
            .AddResource(serverResource)
            .WithImage("goauthentik/server")
            .WithImageRegistry("ghcr.io")
            .WithImageTag("2025.10.0")
            .WithArgs("server")
            .WithHttpEndpoint(port: port, targetPort: DefaultContainerPort)
            .WithHttpHealthCheck(path: "/-/health/live/")
            .WithEnvironment("AUTHENTIK_SECRET_KEY", serverResource.AdminPasswordParameter).WithPostgres(database);
        authentik.AddAuthentikWorker(name, database);
        return authentik;
    }
}