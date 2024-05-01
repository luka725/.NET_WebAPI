using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class User
    {
        public string UserID { get; set; }
        public string FirstName { get; set; }
        public string Password { get; set; }
    }
    public class RegisterController : ApiController
    {
        public static List<User> _users= new List<User>
        {
            new User {
                UserID = "1",
                FirstName = "Luka",
                Password = "Password",
            }
        };

        [System.Web.Http.Route("api/register")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult RegisterUser([FromBody] User newUser)
        {
            if (newUser == null)
            {
                return BadRequest("Invalid user data");
            }

            if (_users.Any(u => u.FirstName.ToLower() == newUser.FirstName.ToLower()))
            {
                return Conflict();
            }

            newUser.UserID = Guid.NewGuid().ToString();

            _users.Add(newUser);

            return Ok(newUser);
        }

        public List<User> GetUsers()
        {
            return _users;
        }

    }
}