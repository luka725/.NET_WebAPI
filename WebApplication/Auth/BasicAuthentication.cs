using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApplication.Controllers;

namespace WebApplication.Auth
{
    public class BasicAuthentication : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {

            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return;
            }


            string authHeader = actionContext.Request.Headers.Authorization.Parameter;
            string decodedAuthHeader = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader));
            string[] credentials = decodedAuthHeader.Split(':');

            if (credentials.Length < 2)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return;
            }

            string username = credentials[0];
            string password = credentials[1];


            RegisterController registerController = new RegisterController();
            List<User> users = registerController.GetUsers();
            User user = users.FirstOrDefault(u => u.FirstName.ToLower() == username.ToLower());


            if (user == null || user.Password != password)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return;
            }

            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(username), null);
        }
    }
}