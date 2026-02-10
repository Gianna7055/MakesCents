/*
 * Gianna Ross
 * Makes Cents
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
        private readonly UserLogic _userLogic;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="userLogic"></param>
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
        public async Task<ActionResult> RegisterUserAsync(RegisterRequest user)
        {
            // Declare and initialize
            RegisterResponse response;

            // Call the method in the logic class
            response = await _userLogic.RegisterUserAsync(user);

            // Check if the userId came back correctly
            if (response.HttpStatus == 400)
            {
                return BadRequest(response.Message);
            }

            // Return a success otherwise
            return Created("", new
            {
                status = response.HttpStatus,
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

            // Call the method from the logic class
            response = await _userLogic.LoginUserAsync(user);
            // Check if the userId came back correctly
            if (response.HttpStatus == 400)
            {
                return BadRequest(response.Message);
            }
            // Check if the userId came back correctly
            else if (response.HttpStatus == 401)
            {
                return Unauthorized(response.Message);
            }
            else
            {
                // Return a success otherwise
                return Ok(new
                {
                    status = response.HttpStatus,
                    userId = response.UserId,
                    token = response.Token,
                    message = response.Message
                });
            }
        }

        /// <summary>
        /// Method to get a user based on its id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetUserAsync()
        {
            // Declare and initialize
            GetUserResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);
            
            // Call the method from the logic class
            response = await _userLogic.GetUserAsync(userId);

            // Check if the status came back correctly
            if (response.HttpStatus == 400)
            {
                return BadRequest(response.Message);
            }
            else if (response.HttpStatus == 404)
            {
                return NotFound(response.Message);
            }
            else
            {
                // Return a success otherwise
                return Ok(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    user = response.GetUserDTO
                });
            }
        }

        /// <summary>
        /// Method to update a user based on the given fields
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        public async Task<ActionResult> UpdateUserAsync(EditUserRequest user)
        {
            // Declare and initialize
            BaseIdResponse response;
            // Get the user id from the JWT token
            int userId = ClaimsPrincipalExtensions.GetUserId(User);

            // Set the users id to the path id
            user.UserId = userId;
            // Call the method from the logic class
            response = await _userLogic.UpdateUserAsync(user);

            // Check if the status came back as a success
            if (response.HttpStatus == 400)
            {
                // Return a bad request
                return BadRequest(response.Message);
            }
            else if (response.HttpStatus == 404)
            {
                // Return a not found
                return NotFound(response.Message);
            }
            else
            {
                // Return Ok
                return Ok(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                    userId = response.Id
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

            if (response.HttpStatus == 404)
            {
                // Return a not found
                return NotFound(response.Message);
            }
            else
            {
                // Return Ok
                return Ok(new
                {
                    status = response.HttpStatus,
                    message = response.Message,
                });
            }
        }
    }
}
