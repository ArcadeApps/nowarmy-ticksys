namespace AppHost.Extensions.Authentik;

public class AuthentikResourceBase(string name,AuthentikResource parent, ParameterResource? admin, ParameterResource adminPassword) :
    ContainerResource(name), IResourceWithServiceDiscovery, IResourceWithParent<AuthentikResource>
{
    protected const string DefaultAdmin = "admin";
    internal const string PrimaryEndpointName = "tcp";
    
    public ParameterResource? AdminUserNameParameter { get; } = admin;
    public AuthentikResource Parent { get; } = parent;

    internal ReferenceExpression AdminReference =>
        AdminUserNameParameter is not null ?
            ReferenceExpression.Create($"{AdminUserNameParameter}") :
            ReferenceExpression.Create($"{DefaultAdmin}");
    
    
    public ParameterResource AdminPasswordParameter { get; } = adminPassword ?? throw new ArgumentNullException(nameof(adminPassword));

}

public sealed class AuthentikResource(string name, AuthentikServerResource? server = null, AuthentikWorkerResource? worker = null) : 
    Resource(name), IResourceWithServiceDiscovery
{
    public AuthentikServerResource? Server { get; set; } = server;
    public AuthentikWorkerResource? Worker { get; set; } = worker;
}