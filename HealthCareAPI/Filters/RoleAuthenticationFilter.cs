using System.Net.Http;
using System.Net;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

public class RoleAuthenticationFilter : AuthorizationFilterAttribute
{
    private readonly string[] _allowedRoles;

    public RoleAuthenticationFilter(params string[] allowedRoles)
    {
        _allowedRoles = allowedRoles;
    }

    public override void OnAuthorization(HttpActionContext actionContext)
    {
        var principal = actionContext.RequestContext.Principal as UserPrincipal;

        if (principal == null || !IsInAllowedRoles(principal))
        {
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            return;
        }

        base.OnAuthorization(actionContext);
    }

    private bool IsInAllowedRoles(UserPrincipal principal)
    {
        foreach (var role in _allowedRoles)
        {
            if (principal.IsInRole(role))
            {
                return true;
            }
        }
        return false;
    }
}
