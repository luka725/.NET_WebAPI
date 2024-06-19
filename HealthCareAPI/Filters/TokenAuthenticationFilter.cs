using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using HealthCareAPI.Helpers;

public class TokenAuthenticationFilter : AuthorizationFilterAttribute
{
    private readonly DatabaseHelper _dbHelper;

    public TokenAuthenticationFilter()
    {
        _dbHelper = DatabaseHelper.Instance;
    }

    public override async Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
    {
        var tokenHeader = actionContext.Request.Headers.Authorization?.Parameter;

        if (tokenHeader == null)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            return;
        }

        var user = await _dbHelper.GetUserByTokenAsync(tokenHeader);

        if (user == null)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid token");
            return;
        }

        var principal = new UserPrincipal(user.Username);

        // Fetch user roles and populate UserPrincipal
        var roles = await _dbHelper.GetUserRolesAsync(user.UserID); // Implement this method in your DatabaseHelper
        foreach (var role in roles)
        {
            principal.Roles.Add(role.RoleName);
        }

        actionContext.RequestContext.Principal = principal;

        await base.OnAuthorizationAsync(actionContext, cancellationToken);
    }
}
