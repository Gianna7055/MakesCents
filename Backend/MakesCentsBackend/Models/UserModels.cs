/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */

using TypeGen.Core.TypeAnnotations;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a user
    /// </summary>
    public class UserEntityModel
    {
        // Class Level Properties
        public int UserId { get; set; } = 0;
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public bool IsDarkMode { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        public DateTime LastUpdatedAt { get; set; } = DateTime.MinValue;
        public BudgetEntityModel? Budget { get; set; } = null;
    }

    /// <summary>
    /// Response model for an entity user
    /// </summary>
    [ExportTsInterface]
    public class UserEntityResponse : BaseResponse
    {
        // Class level properties
        public UserEntityModel User { get; set; }
        public string? Token { get; set; }

        /// <summary>
        /// Parameterized constructor for the register response model
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        public UserEntityResponse(UserEntityModel user, int status, string message) : base(status, message)
        {
            User = user;
            Token = null;
        }
    }

    /// <summary>
    /// Response model for a DTO user
    /// </summary>
    [ExportTsInterface]
    public class UserDTOResponse : BaseResponse
    {
        // Class level properties
        public UserDTOModel? User { get; set; } = null;
        public string? Token { get; set; } = null;
    }

    /// <summary>
    /// Model for a user
    /// </summary>
    [ExportTsInterface]
    public class UserDTOModel
    {
        // Class Level Properties
        public int UserId { get; set; } = 0;

        public string Username { get; set; } = "";

        public string Email { get; set; } = "";

        public bool IsDarkMode { get; set; } = false;
        public BudgetEntityModel? Budget { get; set; } = null;
    }


    [ExportTsInterface]
    public class GetUserResponse : BaseResponse
    {
        // Class Level Properties
        public GetUserDTOModel GetUserDTO { get; set; } = new GetUserDTOModel();

        /// <summary>
        /// Parameterized constructor that takes a status, message, and GetUserDTO object
        /// </summary>
        /// <param name="httpStatus"></param>
        /// <param name="message"></param>
        /// <param name="getUserDTO"></param>
        public GetUserResponse(int httpStatus, string message, GetUserDTOModel getUserDTO) : base(httpStatus, message)
        {
            GetUserDTO = getUserDTO;
        }

        /// <summary>
        /// Parameterized constructor that takes a status and message
        /// </summary>
        /// <param name="httpStatus"></param>
        /// <param name="message"></param>
        public GetUserResponse(int httpStatus, string message) : base(httpStatus, message) { }
    }

    /// <summary>
    /// DTO model for getting a user
    /// </summary>
    [ExportTsInterface]
    public class GetUserDTOModel
    {
        // Class Level Properties
        public int UserId { get; set; } = 0;

        public string Username { get; set; } = "";

        public string Email { get; set; } = "";

        public bool IsDarkMode { get; set; } = false;
    }

    /// <summary>
    /// Request model for editing a user
    /// </summary>
    [ExportTsClass]
    public class EditUserRequest
    {
        // Class Level Properties
        public int UserId { get; set; } = 0;

        public string? Username { get; set; } = null;

        public string? Email { get; set; } = null;
        public string? PasswordHash { get; set; } = null;

        public bool? IsDarkMode { get; set; } = null;
    }

    /// <summary>
    /// Request model for a user
    /// </summary>
    [ExportTsClass]
    public class LoginRequest
    {
        // Class Level Properties
        public string UsernameOrEmail { get; set; } = "";
        public string Password { get; set; } = "";
    }

    /// <summary>
    /// Response model for the login process
    /// </summary>
    [ExportTsInterface]
    public class LoginResponse : BaseIdResponse
    {
        public string PasswordHash { get; set; } = "";
        public string Token { get; set; } = "";

        /// <summary>
        /// Parameterized constructor for user id, username or email, password hash, and message
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="usernameOrEmail"></param>
        /// <param name="passwordHash"></param>
        /// <param name="message"></param>
        public LoginResponse(int status, string message, int userId, string passwordHash) : base(status, message, userId)
        {
            PasswordHash = passwordHash;
        }

        /// <summary>
        /// Parameterized constructor for status, message, and user id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        public LoginResponse(int status, string message, int userId) : base(status, message, userId) { }

        /// <summary>
        /// Parameterized constructor for status and message
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        public LoginResponse(int status, string message) : base(status, message, -1) { }
    }


    [ExportTsClass]
    public class RegisterRequest
    {
        // Class level properties
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
    }

    /// <summary>
    /// Response model for registration
    /// </summary>
    [ExportTsInterface]
    public class RegisterResponse : BaseIdResponse
    {
        // Class level properties
        public string? Token { get; set; }

        /// <summary>
        /// Parameterized constructor for the register response model
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        public RegisterResponse(int status, string message, int userId) : base(status, message, userId)
        {
            Token = null;
        }

        /// <summary>
        /// Parameterized constructor for the register response model
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        public RegisterResponse(int status, string message) : base (status, message, -1) 
        {
            Token = null;
        }
    }
}