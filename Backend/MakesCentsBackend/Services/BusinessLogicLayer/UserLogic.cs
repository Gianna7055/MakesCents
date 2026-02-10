/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using AutoMapper;
using MakesCentsBackend.Models;
using MakesCentsBackend.Services.DataAccessLayer;
using MakesCentsBackend.Services.Utilities;
using System.ComponentModel.DataAnnotations;

namespace MakesCentsBackend.Services.BusinessLogicLayer
{
    /// <summary>
    /// Class for all business rules for users
    /// </summary>
    public class UserLogic
    {
        // Class level variables
        private readonly UserDAO _userDAO;
        private readonly JwtService _jwtService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Parameterized constructor for UserLogic 
        /// to pull in necessary dependencies
        /// </summary>
        /// <param name="userDAO"></param>
        public UserLogic(UserDAO userDAO, JwtService jwtService, IMapper mapper)
        {
            _userDAO = userDAO;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        /// <summary>
        /// Create a user from a userModel
        /// </summary>
        /// <param name="user">The new user to create</param>
        /// <returns></returns>
        public async Task<RegisterResponse> RegisterUserAsync(RegisterRequest user)
        {
            // Declare and initialize
            RegisterResponse response;
            int maxUsernameLength = 30;
            int maxEmailLength = 320;

            // Make sure the user has the required fields
            // Check for missing fields
            if (String.IsNullOrEmpty(user.Username) || String.IsNullOrEmpty(user.Email) || String.IsNullOrEmpty(user.PasswordHash))
            {
                return new RegisterResponse(400, "Missing information for registration");
            }
            // Check if the email is in a valid format
            else if (!new EmailAddressAttribute().IsValid(user.Email))
            {
                return new RegisterResponse(400, "Invalid email format");
            }
            // Check if the username is a valid length
            else if (user.Username.Length > maxUsernameLength)
            {
                return new RegisterResponse(400, "Username exceeds maximum length of " + maxUsernameLength);
            }
            // Check if the email is a valid length
            else if (user.Email.Length > maxEmailLength)
            {
                return new RegisterResponse(400, "Email exceeds maximum length of " + maxEmailLength);
            }

            // Hash the users password
            user.PasswordHash = PasswordHasher.HashPassword(user.PasswordHash);
            // Normalize the username and email
            user.Username = user.Username.ToLower();
            user.Email = user.Email.ToLower();
            // Call the Create User method in the DAO
            response = await _userDAO.RegisterUserAsync(user);
            // Check if the response came back with an error status
            if (response.HttpStatus == 400)
            {
                // Return the existing error response
                return response;
            }
            // Generate a JWT token and add it to the response
            response.Token = _jwtService.GenerateToken(response.UserId);
            // Return the response
            return response;
        }

        /// <summary>
        /// Authenticate a user based on a username or email and a password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<LoginResponse> LoginUserAsync(LoginRequest user)
        {
            // Declare and initialize
            LoginResponse response;

            // Make sure a username/email (saved in usernameOrEmail) and password were provided
            if (String.IsNullOrEmpty(user.UsernameOrEmail) || String.IsNullOrEmpty(user.Password))
            {
                return new LoginResponse(-1, 400, "Missing information for login");
            }
            // Check to make sure the usernameOrEmail is the correct length
            else if (user.UsernameOrEmail.Length > 320)
            {
                return new LoginResponse(-1, 400, "Username or email exceeds maximum length");
            }

            // Get the user from the DAO based on the username
            response = await _userDAO.FindUserByUsernameOrEmailAsync(user.UsernameOrEmail);
            // Check if the response status is 400
            if (response.HttpStatus == 400)
            {
                // Check if the error was due to an unfound user
                if (response.Message == "User not found")
                {
                    response.HttpStatus = 401;
                    // Return the invalid login response
                    response.Message = "Invalid username or password";
                }
                // Else, return the error as is
                return response;
            }
            // Validate the users password
            if (PasswordHasher.VerifyPassword(user.Password, response.PasswordHash))
            {
                // Generate a JWT token and add it to the response
                response.Token = _jwtService.GenerateToken(response.UserId);
                response.Message = "Login successful";
                // Return the successful login
                return response;
            }
            // Else, return the invalid login response
            response.HttpStatus = 401;
            response.Message = "Invalid password or password";
            return response;
        }

        /// <summary>
        /// Get a user based on a user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<GetUserResponse> GetUserAsync(int userId)
        {
            // Declare and initialize
            GetUserResponse response;

            // Call and return the Get User From Id Async method from the DAO
            response = await _userDAO.GetUserAsync(userId);

            // Return the response
            return response;
        }

        /// <summary>
        /// Async method to update a user
        /// </summary>
        /// <param name="user">The new user to create</param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdateUserAsync(EditUserRequest user)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Make sure the user has the required fields
            if (user.UserId == null)
            {
                return new BaseIdResponse(400, "Missing information for update");
            }
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                // Hash the users password
                user.PasswordHash = PasswordHasher.HashPassword(user.PasswordHash);
            }
            // Call the Create User method in the DAO
            response = await _userDAO.UpdateUserAsync(user);
            // Return the response
            return response;
        }

        /// <summary>
        /// Async method to delete a user based on a userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteUserAsync(int userId)
        {
            // Return a call the the DAO method
            return await _userDAO.DeleteUserAsync(userId);
        }
    }
}
