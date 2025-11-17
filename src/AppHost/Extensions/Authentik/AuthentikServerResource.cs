namespace AppHost.Extensions.Authentik;

public sealed class AuthentikServerResource(string name, ParameterResource? admin, ParameterResource adminPassword) :
    AuthentikResource(name, admin, adminPassword);

public class AuthentikResource(string name, ParameterResource? admin, ParameterResource adminPassword) :
    ContainerResource(name), IResourceWithServiceDiscovery
{
    protected const string DefaultAdmin = "admin";
    internal const string PrimaryEndpointName = "tcp";
    
    public ParameterResource? AdminUserNameParameter { get; } = admin;

    internal ReferenceExpression AdminReference =>
        AdminUserNameParameter is not null ?
            ReferenceExpression.Create($"{AdminUserNameParameter}") :
            ReferenceExpression.Create($"{DefaultAdmin}");
    
    
    public ParameterResource AdminPasswordParameter { get; } = adminPassword ?? throw new ArgumentNullException(nameof(adminPassword));
    
}