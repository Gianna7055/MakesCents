/*
 * Gianna Ross
 * File Created: 11/20/2025
 * File Last Updated: 12/5/2025
 * Makes Cents - User Model
 * Sources: 
 */
using System.ComponentModel.DataAnnotations;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// Model for a user
    /// </summary>
    public class UserModel
    {
        // Class Level Properties
        public int UserId { get; set; } = 0;
        public string Username { get; set; } = "";

        [EmailAddress]
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public bool IsDarkMode { get; set; } = false;
        public DateTime CreatedAt { get; set; } = new DateTime();
        public DateTime LastUpdatedAt { get; set; } = new DateTime();
        public BudgetModel? Budget { get; set; } = null;
    }
}
