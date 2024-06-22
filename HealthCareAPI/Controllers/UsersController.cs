using HealthCareAPI.DTOs;
using HealthCareAPI.Helpers;
using HealthCareDAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace HealthCareAPI.Controllers
{
    /// <summary>
    /// Controller for managing users.
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private readonly DatabaseHelper _dbHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        public UsersController()
        {
            _dbHelper = DatabaseHelper.Instance;
        }
        /// <summary>
        /// Retrieves all roles from the database.
        /// </summary>
        /// <returns>HTTP response containing a list of RoleDTOs.</returns>
        [HttpGet]
        [Route("roles")]
        public async Task<IHttpActionResult> GetRoles()
        {
            try
            {
                var roles = await _dbHelper.GetRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                return InternalServerError();
            }
        }
        /// <summary>
        /// Tests user retrieval.
        /// </summary>
        /// <returns>Returns user data as JSON.</returns>
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

        /// <summary>
        /// Tests user roles retrieval.
        /// </summary>
        /// <returns>Returns user data with roles as JSON.</returns>
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

        /// <summary>
        /// Gets user with roles and tokens.
        /// </summary>
        /// <returns>Returns user data with roles and tokens as JSON.</returns>
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

        /// <summary>
        /// Retrieves details of a specific user by their ID.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns>Returns user details as JSON.</returns>
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

        /// <summary>
        /// Updates an existing user by their ID.
        /// </summary>
        /// <param name="userId">ID of the user to update.</param>
        /// <param name="updatedUser">Updated details of the user.</param>
        /// <returns>Status of the update operation.</returns>
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

        /// <summary>
        /// Deletes an existing user by their ID.
        /// </summary>
        /// <param name="userId">ID of the user to delete.</param>
        /// <returns>Status of the delete operation.</returns>
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

        /// <summary>
        /// Authenticates a user and returns a token if successful.
        /// </summary>
        /// <param name="loginDto">Login details.</param>
        /// <returns>Token and its expiry date.</returns>
        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var user = await _dbHelper.ValidateUserAsync(loginDto.Username, loginDto.Password);
            if (user == null)
            {
                return Unauthorized();
            }
            await _dbHelper.CleanupExpiredTokensAsync();
            var existingToken = user.Tokens.FirstOrDefault(t => t.ExpiryDateTime > DateTime.UtcNow);
            if (existingToken != null)
            {
                return Ok(new TokenResponseDTO { Token = existingToken.TokenValue, ExpiryDate = existingToken.ExpiryDateTime ?? DateTime.UtcNow });
            }

            var newToken = await _dbHelper.GenerateTokenAsync(user);
            return Ok(new TokenResponseDTO { Token = newToken.TokenValue, ExpiryDate = newToken.ExpiryDateTime });
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="user">Details of the new user.</param>
        /// <returns>Registered user DTO.</returns>
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

        /// <summary>
        /// Retrieves the current authenticated user.
        /// </summary>
        /// <returns>Current user data with roles as JSON.</returns>
        [TokenAuthenticationFilter]
        [Route("me")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCurrentUser()
        {
            try
            {
                // Get token from request header
                var tokenHeader = Request.Headers.Authorization?.Parameter;

                if (string.IsNullOrEmpty(tokenHeader))
                {
                    return Unauthorized();
                }

                // Retrieve user ID using token
                var userId = await _dbHelper.GetUserIdByTokenAsync(tokenHeader);

                if (!userId.HasValue)
                {
                    return Unauthorized();
                }

                // Retrieve user with roles by user ID
                var userWithRoles = await _dbHelper.GetUserWithRolesByIdAsync(userId.Value);

                if (userWithRoles == null)
                {
                    return NotFound();
                }

                return Ok(userWithRoles);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Exception in GetCurrentUser: {ex.Message}");
                return InternalServerError();
            }
        }
    }
}
