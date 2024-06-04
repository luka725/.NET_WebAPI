using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApplication.DBHelper;

namespace WebApplication.Attributes
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override async Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Authorization header is missing");
                return;
            }

            var authenticationToken = actionContext.Request.Headers.Authorization.Parameter;
            var decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));
            var emailAndPassword = decodedAuthenticationToken.Split(':');

            var email = emailAndPassword[0];
            var password = emailAndPassword[1];

            if (!await ValidateUserAsync(email, password))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid email or password");
            }
        }

        private async Task<bool> ValidateUserAsync(string email, string password)
        {
            return await DatabaseHelper.Instance.AuthenticateUserAsync(email, password);
        }
    }
}