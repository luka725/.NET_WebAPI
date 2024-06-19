using System.Collections.Generic;
using System.Security.Principal;

public class UserPrincipal : IPrincipal
{
    public UserPrincipal(string username)
    {
        Identity = new GenericIdentity(username);
        Roles = new List<string>();
    }

    public IIdentity Identity { get; }

    public List<string> Roles { get; }

    public bool IsInRole(string role)
    {
        return Roles.Contains(role);
    }
}
