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
using WebApplication.Attributes;

namespace WebApplication.Controllers
{
    public class AuthController : ApiController
    {
        [BasicAuthentication]
        [System.Web.Http.Route("api/auth/login")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult Auth(User user)
        {
            return Ok(user);
        }
        [BasicAuthentication]
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
        [BasicAuthentication]
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

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/auth/register")]
        public async Task<IHttpActionResult> Register(UsersDTO registerUserDTO)
        {
            if (registerUserDTO == null || string.IsNullOrEmpty(registerUserDTO.Name) || string.IsNullOrEmpty(registerUserDTO.Email) || string.IsNullOrEmpty(registerUserDTO.Password))
            {
                return BadRequest("Invalid client request");
            }

            int? userId = await DatabaseHelper.Instance.RegisterUserAsync(registerUserDTO);
            if (userId.HasValue)
            {
                return Ok(new { message = "Registration successful", userId = userId.Value });
            }
            else
            {
                return BadRequest("User already exists or registration failed");
            }
        }
    }
}