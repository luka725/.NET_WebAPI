using DataAccesLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebApplication.DBHelper;
using DataTransferObjects;

namespace WebApplication.Controllers
{
    public class AuthController : ApiController
    {
        [System.Web.Http.Route("api/auth")]
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> Auth(User user)
        {
            string email = user.Email;
            string password = user.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Invalid email or password");
            }
            else
            {
                bool isAuthenticated = await DatabaseHelper.Instance.AuthenticateUserAsync(email, password);
                if (isAuthenticated)
                {
                    return Ok("Authentication success");
                }
                else
                {
                    return BadRequest("Invalid email or password");
                }
            }
        }
    }
}