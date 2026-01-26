/*
 * Gianna Ross
 * File Created: 1/22/2026
 * File Last Updated: 1/22/2026
 * Makes Cents -Edit  User Models
 * Sources: 
 */
namespace MakesCentsBackend.Models
{
    /// <summary>
    /// DTO model for editing a user
    /// Contains nullable fields and flags for all bools
    /// </summary>
    public class EditUserDTO
    {
        // Class Level Properties
        public int UserId { get; set; } = 0;

        public string? Username { get; set; } = null;

        public string? Email { get; set; } = null;
        public string? PasswordHash { get; set; } = null;

        public bool? IsDarkMode { get; set; } = null;
    }

    /// <summary>
    /// Response for the edit user flow with an id, status, and message
    /// </summary>
    public class EditUserResponse
    {
        // CLass Level Properties
        public int UserId { get; set; } = 0;
        public int Status { get; set; } = 0;
        public string Message { get; set; } = "";

        /// <summary>
        /// Parameterized constructor for the edit user response
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <param name="message"></param>
        public EditUserResponse(int userId, int status, string message)
        {
            UserId = userId;
            Status = status;
            Message = message;
        }
    }
}
