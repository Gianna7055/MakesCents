/*
 * Gianna Ross
 * File Created: 1/10/2025
 * File Last Updated: 1/10/2025
 * Makes Cents - User Logic
 * Sources: 
 */
using AutoMapper;
using MakesCentsBackend.Models;
using MakesCentsBackend.Services.DataAccessLayer;
using MakesCentsBackend.Services.Utilities;
using System.Data.Common;

namespace MakesCentsBackend.Services.BusinessLogicLayer
{
    /// <summary>
    /// Class for all business rules for users
    /// </summary>
    public class UserLogic
    {
        // Class level variables
        private UserDAO _userDAO;
        private JwtService _jwtService;
        private IMapper _mapper;

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
        public async Task<RegisterResponse> RegisterUserAsync(UserEntity user)
        {
            // Declare and initialize
            RegisterResponse response;

            // Make sure the user has the required fields
            if (String.IsNullOrEmpty(user.Username) || String.IsNullOrEmpty(user.Email) || String.IsNullOrEmpty(user.PasswordHash))
            {
                return new RegisterResponse(-1, 400, "Missing information for registration");
            }
            // Hash the users password
            user.PasswordHash = PasswordHasher.HashPassword(user.PasswordHash);
            // Call the Create User method in the DAO
            response = await _userDAO.RegisterUserAsync(user);
            // Check if the response came back with an error status
            if (response.Status == 400)
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

            // Make sure a username/email (saved in username) and password were provided
            if (String.IsNullOrEmpty(user.UsernameOrEmail) || String.IsNullOrEmpty(user.Password))
            {
                return new LoginResponse(-1, 400, "Missing information for login");
            }
            // Get the user from the DAO based on the username
            response = await _userDAO.FindUserByUsernameOrEmailAsync(user.UsernameOrEmail);
            // Check if the response status is 400
            if (response.Status == 400)
            {
                // Check if the error was due to an unfound user
                if (response.Message == "User not found")
                {
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
            response.Message = "Invalid username or password";
            return response;
        }

        /// <summary>
        /// Get a user based on a user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserDTOResponse> FindUserFromIdAsync(int userId)
        {
            // Declare and initialize
            UserEntityResponse entityResponse;
            UserDTOResponse dtoResponse;

            // Call and return the Get User From Id Async method from the DAO
            entityResponse = await _userDAO.FindUserFromIdAsync(userId);
            dtoResponse = _mapper.Map<UserDTOResponse>(entityResponse);

            // Return the DTO response
            return dtoResponse;
        }

        /// <summary>
        /// Async method to update a user
        /// </summary>
        /// <param name="user">The new user to create</param>
        /// <returns></returns>
        public async Task<EditUserResponse> UpdateUserAsync(EditUserDTO user)
        {
            // Declare and initialize
            EditUserResponse response;

            // Make sure the user has the required fields
            if (user.UserId == 0)
            {
                return new EditUserResponse(user.UserId, -1, "Missing information for update");
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
