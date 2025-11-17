namespace AppHost.Extensions.Authentik;

public class AuthentikWorkerResource(string name, AuthentikResource parent, ParameterResource password) : 
    AuthentikResourceBase(name, parent, null, password)
{
}