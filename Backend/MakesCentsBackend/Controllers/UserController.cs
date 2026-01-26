/*
 * Gianna Ross
 * File Created: 11/16/2025
 * File Last Updated: 11/16/2025
 * Makes Cents - User Controller
 * Sources: 
 */
using MakesCentsBackend.Models;
using MakesCentsBackend.Services.BusinessLogicLayer;
using MakesCentsBackend.Services.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MakesCentsBackend.Controllers
{
    /// <summary>
    /// API controller for users
    /// </summary>
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // Class level variables
        private UserLogic _userLogic;

        public UserController(UserLogic userLogic)
        {
            _userLogic = userLogic;
        }

        /// <summary>
        /// Method to register a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>The code status, user id, token, and message</returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> RegisterUserAsync(UserEntity user)
        {
            // Declare and initialize
            RegisterResponse response;

            // Validate the model
            if (!ModelState.IsValid)
            {
                return BadRequest("Missing information for registration");
            }

            // Call the method in the logic class
            response = await _userLogic.RegisterUserAsync(user);

            // Check if the userId came back correctly
            if (response.Status == 400)
            {
                return BadRequest(response.Message);
            }

            // Return a success otherwise
            return Created("", new
            {
                status = response.Status,
                userId = response.UserId,
                token = response.Token,
                message = response.Message
            });
        }

        /// <summary>
        /// Method to login an existing user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>The code status, user id, token, and message</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> LoginUserAsync(LoginRequest user)
        {
            // Declare and initialize
            LoginResponse response;

            // Validate the model
            if (!ModelState.IsValid)
            {
                return BadRequest("Missing information for login");
            }
            // Call the method from the logic class
            response = await _userLogic.LoginUserAsync(user);

            // Check if the userId came back correctly
            if (response.Status == 400)
            {
                return BadRequest(response.Message);
            }

            // Return a success otherwise
            return Created("", new
            {
                status = response.Status,
                userId = response.UserId,
                token = response.Token,
                message = response.Message
            });
        }

        /// <summary>
        /// Method to get a user based on its id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetUserByIdAsync()
        {
            // Declare and initialize
            UserDTOResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);
            
            // Call the method from the logic class
            response = await _userLogic.FindUserFromIdAsync(userId);

            // Check if the status came back correctly
            if (response.Status == 400)
            {
                return BadRequest(response.Message);
            }

            // Return a success otherwise
            return Ok(new
            {
                status = response.Status,
                user = response.User,
                message = response.Message
            });
        }

        /// <summary>
        /// Method to update a user based on the given fields
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        public async Task<ActionResult> UpdateUserAsync(EditUserDTO user)
        {
            // Declare and initialize
            EditUserResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the users id to the path id
            user.UserId = userId;
            // Call the method from the logic class
            response = await _userLogic.UpdateUserAsync(user);

            // Check if the status came back as a success
            if (response.Status == 400)
            {
                // Return a bad request
                return BadRequest(response.Message);
            }
            else if (response.Status == 404)
            {
                // Return a not found
                return NotFound(response.Message);
            }
            else
            {
                // Return Ok
                return Ok(new
                {
                    status = response.Status,
                    message = response.Message,
                    userId = response.UserId
                });
            }
        }

        /// <summary>
        /// Delete a user based on a userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteUserAsync()
        {
            // Declare and initialize
            BaseResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Call the logic method to delete the user
            response = await _userLogic.DeleteUserAsync(userId);

            if (response.Status == 40)
            {
                // Return a not found
                return NotFound(response.Message);
            }
            else
            {
                // Return Ok
                return Ok(new
                {
                    status = response.Status,
                    message = response.Message,
                });
             }
        }
    }
}
