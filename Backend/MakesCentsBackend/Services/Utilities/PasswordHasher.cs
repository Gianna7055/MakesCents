/*
 * Gianna Ross
 * File Created: 1/10/2025
 * File Last Updated: 1/10/2025
 * Makes Cents - Password Hasher
 * Sources: 
 */

namespace MakesCentsBackend.Services.Utilities
{
    /// <summary>
    /// Static class to handle password hashing
    /// </summary>
    public static class PasswordHasher
    {
        /// <summary>
        /// Hash a password and return the hash
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string HashPassword(string password)
        {
            // Salt, hash and return password
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            return BCrypt.Net.BCrypt.HashPassword(password, salt);
        }

        /// <summary>
        /// Verify a hashed password with a plaintext one
        /// </summary>
        /// <param name="password"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool VerifyPassword(string password, string hash)
        {
            // Verify the passed in password and the hash
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
