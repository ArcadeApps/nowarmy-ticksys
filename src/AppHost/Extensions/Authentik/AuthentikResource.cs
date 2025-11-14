namespace AppHost.Extensions.Authentik;

public sealed class AuthentikResource(string name) : 
    ContainerResource(name), IResourceWithServiceDiscovery
{
    internal const string PrimaryEndpointName = "tcp";
    
}