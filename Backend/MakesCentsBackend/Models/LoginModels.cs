/*
 * Gianna Ross
 * File Created: 1/15/2026
 * File Last Updated: 1/15/2026
 * Makes Cents - Login Models
 * Sources: 
 */
using System.ComponentModel.DataAnnotations;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Request model for a user
    /// </summary>
    public class LoginRequest
    {
        // Class Level Properties
        [MaxLength(320)]
        public string UsernameOrEmail { get; set; } = "";
        public string Password { get; set; } = "";
    }

    /// <summary>
    /// Response model for the login process
    /// </summary>
    public class LoginResponse
    {
        public int UserId { get; set; } = 0;
        public string PasswordHash { get; set; } = "";
        public string Token { get; set; } = "";
        public int Status { get; set; } = 0;
        public string Message { get; set; } = "";

        /// <summary>
        /// Parameterized constructor for user id, username or email, password hash, and message
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="usernameOrEmail"></param>
        /// <param name="passwordHash"></param>
        /// <param name="message"></param>
        public LoginResponse(int userId, string passwordHash, int status, string message)
        {
            UserId = userId;
            PasswordHash = passwordHash;
            Status = status;
            Message = message;
        }

        /// <summary>
        /// Parameterized constructor for user id and message
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        public LoginResponse(int userId, int status, string message)
        {
            UserId = userId;
            Status = status;
            Message = message;
        }
    }
}
