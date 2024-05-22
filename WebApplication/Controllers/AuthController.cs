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
        [System.Web.Http.Route("api/auth/getID")]
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> GetUserID(User email)
        {
            string mail = email.Email;
            try
            {
                if (string.IsNullOrEmpty(mail))
                {
                    return BadRequest("Invalid Email");
                }
                else
                {
                    int userID = await DatabaseHelper.Instance.GetUserIdByEmail(mail);
                    return Ok(userID);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [System.Web.Http.Route("api/auth/getUser")]
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> GetUser(User Id)
        {
            int id = Id.ID;
            try
            {
                UsersDTO user = await DatabaseHelper.Instance.GetUserById(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}