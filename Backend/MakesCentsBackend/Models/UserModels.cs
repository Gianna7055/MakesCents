/*
 * Gianna Ross
 * File Created: 1/15/2026
 * File Last Updated: 1/15/2026
 * Makes Cents - User Models
 * Sources: 
 */
using System.ComponentModel.DataAnnotations;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a user
    /// </summary>
    public class UserEntity
    {
        // Class Level Properties
        public int UserId { get; set; } = 0;

        [MaxLength(30)]
        public string Username { get; set; } = "";

        [EmailAddress]
        [MaxLength(320)]
        public string Email { get; set; } = "";

        public string PasswordHash { get; set; } = "";
        public bool IsDarkMode { get; set; } = false;
        public DateTime CreatedAt { get; set; } = new DateTime();
        public DateTime LastUpdatedAt { get; set; } = new DateTime();
        public BudgetEntity? Budget { get; set; } = null;
    }
    
    /// <summary>
    /// Response model for an entity user
    /// </summary>
    public class UserEntityResponse
    {
        // Class level properties
        public UserEntity User { get; set; }
        public int Status { get; set; } = 0;
        public string? Message { get; set; }
        public string? Token { get; set; }

        /// <summary>
        /// Parameterized constructor for the register response model
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        public UserEntityResponse(UserEntity user, int status, string? message)
        {
            User = user;
            Status = status;
            Message = message;
            Token = null;
        }
    }

    /// <summary>
    /// Response model for a DTO user
    /// </summary>
    public class UserDTOResponse
    {
        // Class level properties
        public UserDTO? User { get; set; } = null;
        public int Status { get; set; } = 0;
        public string? Message { get; set; } = null;
        public string? Token { get; set; } = null;
    }
    /// <summary>
    /// Model for a user
    /// </summary>
    public class UserDTO
    {
        // Class Level Properties
        public int UserId { get; set; } = 0;

        [MaxLength(30)]
        public string Username { get; set; } = "";

        [EmailAddress]
        [MaxLength(320)]
        public string Email { get; set; } = "";

        public bool IsDarkMode { get; set; } = false;
        public BudgetEntity? Budget { get; set; } = null;
    }
}
