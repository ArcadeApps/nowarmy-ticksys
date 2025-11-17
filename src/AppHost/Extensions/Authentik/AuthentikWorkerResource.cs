namespace AppHost.Extensions.Authentik;

public class AuthentikWorkerResource(string name, AuthentikServerResource parent, ParameterResource password) : 
    AuthentikResource(name, null, password), IResourceWithParent<AuthentikServerResource>
{
    public AuthentikServerResource Parent { get; } = parent;
}