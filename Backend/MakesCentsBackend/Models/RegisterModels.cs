/*
 * Gianna Ross
 * File Created: 1/15/2026
 * File Last Updated: 1/15/2026
 * Makes Cents - Register Models
 * Sources: 
 */
namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Response model for registration
    /// </summary>
    public class RegisterResponse
    {
        // Class level properties
        public int UserId { get; set; }
        public int Status { get; set; } = 0;
        public string? Message { get; set; }
        public string? Token { get; set; }

        /// <summary>
        /// Parameterized constructor for the register response model
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        public RegisterResponse(int userId, int status, string? message)
        {
            UserId = userId;
            Status = status;
            Message = message;
            Token = null;
        }
    }
}
