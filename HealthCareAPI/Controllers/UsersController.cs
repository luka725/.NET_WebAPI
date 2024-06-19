using HealthCareAPI.DTOs;
using HealthCareAPI.Helpers;
using HealthCareDAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace HealthCareAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private readonly DatabaseHelper _dbHelper;

        public UsersController()
        {
            _dbHelper = DatabaseHelper.Instance;
        }
        // GET: User
        [TokenAuthenticationFilter]
        [RoleAuthenticationFilter("Editor")]
        [HttpGet]
        [Route("test/user")]
        public async Task<IHttpActionResult> TestUserRetrieval()
        {
            try
            {
                // Example: Retrieve any user from the database
                var userId = 1; // Replace with a valid user ID in your database
                var user = await _dbHelper.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound();
                }

                // Return user data as JSON
                return Ok(user);
            }
            catch (Exception ex)
            {
                // Log any exceptions for debugging purposes
                Debug.WriteLine($"Exception occurred during user retrieval test: {ex.Message}");
                return InternalServerError();
            }
        }
        // GET: Users
        [HttpGet]
        [Route("test/userrole")]
        public async Task<IHttpActionResult> TestUserRolesRetrieval()
        {
            var user = await _dbHelper.GetUserWithRolesByIdAsync(1);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        [TokenAuthenticationFilter]
        [RoleAuthenticationFilter("Administrator")]
        [HttpGet]
        [Route("test/urt")]
        public async Task<IHttpActionResult> GetUserWithRolesAndTokens()
        {
            var user = await _dbHelper.GetUserWithRolesAndTokensByIdAsync(1);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        [TokenAuthenticationFilter]
        [RoleAuthenticationFilter("Administrator")]
        [HttpGet]
        [Route("{userId}")]
        public async Task<IHttpActionResult> GetUser(int userId)
        {
            var user = await _dbHelper.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound(); // User not found
            }

            return Ok(user); // Return user details
        }

        [TokenAuthenticationFilter]
        [RoleAuthenticationFilter("Administrator")]
        [HttpPut]
        [Route("{userId}")]
        public async Task<IHttpActionResult> UpdateUser(int userId, UserDTO updatedUser)
        {
            var success = await _dbHelper.UpdateUserAsync(userId, updatedUser);

            if (!success)
            {
                return NotFound(); // User not found
            }

            return Ok(); // User updated successfully
        }


        [TokenAuthenticationFilter]
        [RoleAuthenticationFilter("Administrator")]
        [HttpDelete]
        [Route("{userId}")]
        public async Task<IHttpActionResult> DeleteUser(int userId)
        {
            var success = await _dbHelper.DeleteUserAsync(userId);

            if (!success)
            {
                return NotFound(); // User not found
            }

            return Ok(); // User deleted successfully
        }
        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var user = await _dbHelper.ValidateUserAsync(loginDto.Username, loginDto.Password);
            if (user == null)
            {
                return Unauthorized();
            }

            var existingToken = user.Tokens.FirstOrDefault(t => t.ExpiryDateTime > DateTime.UtcNow);
            if (existingToken != null)
            {
                return Ok(new TokenResponseDTO { Token = existingToken.TokenValue, ExpiryDate = existingToken.ExpiryDateTime ?? DateTime.UtcNow });
            }

            var newToken = await _dbHelper.GenerateTokenAsync(user);
            return Ok(new TokenResponseDTO { Token = newToken.TokenValue, ExpiryDate = newToken.ExpiryDateTime });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IHttpActionResult> Register([FromBody] RegisterDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var registeredUser = await _dbHelper.RegisterUser(user.Username, user.Password, user.FirstName, user.LastName, user.Email, user.PhoneNumber, user.RoleId);

                return Ok(registeredUser); // Return the registered user DTO
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Registration failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred during registration: {ex.Message}");
            }
        }
    }
}