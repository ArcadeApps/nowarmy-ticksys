namespace AppHost.Extensions.Authentik;

public sealed class AuthentikServerResource(string name, AuthentikResource parent, ParameterResource? admin, ParameterResource adminPassword) :
    AuthentikResourceBase(name, parent, admin, adminPassword);
    