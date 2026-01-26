/*
 * Gianna Ross
 * File Created: 12/15/2025
 * File Last Updated: 12/15/2025
 * Makes Cents - User DAO
 * Sources: 
 */
using Dapper;
using MakesCentsBackend.Models;
using MySqlConnector;
using System.Data;

namespace MakesCentsBackend.Services.DataAccessLayer
{
    /// <summary>
    /// Data Access Object for Users
    /// </summary>
    public class UserDAO
    {
        // Class level variables
        string query = "";
        private readonly IDbConnection _connection;
        
        /// <summary>
        /// Parameterized constructor for the User DAO
        /// Takes in DI parameters
        /// </summary>
        /// <param name="connection"></param>
        public UserDAO(IDbConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Add a new user based on a user model
        /// </summary>
        /// <param name="user">the data for the new user</param>
        /// <returns>The Id of the new user</returns>
        /// 
        public async Task<RegisterResponse> RegisterUserAsync(UserEntity user)
        {
            // Declare and initialize
            query =
                """
                INSERT INTO user (username, email, password_hash, is_dark_mode)
                VALUES (@Username, @Email, @PasswordHash, FALSE);
                SELECT LAST_INSERT_ID();
                """;
            int id = -1;
            string errorColumn = "";

            // Normalize the username and email
            user.Username = user.Username.ToLower();
            user.Email = user.Email.ToLower();
            // Use dapper to execute the request and get the users new id
            try
            {
                // Execute the async scalar method using dapper
                id = await _connection.ExecuteScalarAsync<int>(query, user);
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                // Make sure the email was not duplicated (check error)
                if (ex.Message.Contains("'email'"))
                {
                    errorColumn = "Email";
                }
                // Check if the username was duplicated
                else if (ex.Message.Contains("'username'"))
                {
                    errorColumn = "Username";
                }
                // Return the model ith a duplication error
                return new RegisterResponse(-1, 400, $"{errorColumn} already exists");
            }          
            // Return the new id
            return new RegisterResponse(id, 201, "User registered successfully");
        }

        /// <summary>
        /// Finds a user based on a username/email
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<LoginResponse> FindUserByUsernameOrEmailAsync(string usernameOrEmail)
        {
            // Declare and initialize
            query =
                """
                    SELECT user_id AS UserId, 
                           username AS Username, 
                           email AS Email, 
                           password_hash AS PasswordHash
                    FROM user
                    WHERE username = @UsernameOrEmail OR email = @UsernameOrEmail
                    LIMIT 1;
                """;
            UserEntity? foundUser;

            // Normalize the passed in username
            usernameOrEmail = usernameOrEmail.ToLower();
            try
            {
                // Get the user from the executed query
                foundUser = await _connection.QuerySingleOrDefaultAsync<UserEntity>(query, new { UsernameOrEmail = usernameOrEmail });
            }
            catch (Exception ex)
            {
                // Return any errors
                return new LoginResponse(-1, 400, ex.Message);
            }
            // Make sure a user was found
            if (foundUser != null)
            {
                // Return the found users id and a success message
                return new LoginResponse(foundUser.UserId, foundUser.PasswordHash, 201, "User Found");
            }
            else
            {
                // Return an error that the user was not found
                return new LoginResponse(-1, 400, "User not found");
            }
        }

        /// <summary>
        /// Get a user from the database based on a users id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserEntityResponse> FindUserFromIdAsync(int userId)
        {
            // Declare and initialize
            query =
                """
                    SELECT user_id AS UserId,
                       username AS Username,
                       email AS Email,
                       is_dark_mode AS IsDarkMode
                    FROM user
                    WHERE user_id = @UserId
                    LIMIT 1;
                """;
            UserEntity? foundUser;

            try
            {
                // Get the user from the executed query
                foundUser = await _connection.QuerySingleOrDefaultAsync<UserEntity>(query, new { UserId = userId });
            }
            catch (Exception ex)
            {
                // Return any errors
                return new UserEntityResponse(new UserEntity { UserId = -1 }, 400, ex.Message);
            }
            // Make sure a user was found
            if (foundUser != null)
            {
                // Return the found users id and a success message
                return new UserEntityResponse(foundUser, 200, "User Found");
            }
            else
            {
                // Return an error that the user was not found
                return new UserEntityResponse(new UserEntity { UserId = -1 }, 400, "User not found");
            }
        }

        /// <summary>
        /// Async method to update a users information
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<EditUserResponse> UpdateUserAsync(EditUserDTO user)
        {
            // Declare and initialize
            List<string> updates = new List<string>();
            int rowsAffected;
            string errorColumn = "";

            // Check each nullable field to see if update is necessary
            if (user.Username != null)
            {
                updates.Add("username = @Username");
            }
            if (user.Email != null)
            {
                updates.Add("email = @Email");
            }
            if (user.PasswordHash != null)
            {
                updates.Add("password_hash = @PasswordHash");
            }
            if (user.IsDarkMode != null)
            {
                updates.Add("is_dark_mode = @IsDarkMode");
            }
            // If no fields to update, return early
            if (updates.Count == 0)
            {
                return new EditUserResponse(user.UserId, 400, "No fields to update");
            }
            // Assemble the query
            query = $"""
                UPDATE user 
                SET
                {string.Join(", ", updates)}
                WHERE user_id = @UserId
                """;
            try
            {
                // Execute the query
                rowsAffected = await _connection.ExecuteAsync(query, user);
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                // Make sure the email was not duplicated (check error)
                if (ex.Message.Contains("'email'"))
                {
                    errorColumn = "Email";
                }
                // Check if the username was duplicated
                else if (ex.Message.Contains("'username'"))
                {
                    errorColumn = "Username";
                }
                // Return the model ith a duplication error
                return new EditUserResponse(user.UserId, 400, $"{errorColumn} already exists");
            }
            // Make sure the row was affected
            if (rowsAffected == 1)
            {
                return new EditUserResponse(user.UserId, 200, "Update was successful");
            }
            else if (rowsAffected == 0)
            {
                return new EditUserResponse(user.UserId, 404, "User not found");
            }
            else
            {
                return new EditUserResponse(user.UserId, 400, "An error occurred");
            }
        }

        /// <summary>
        /// Async method to delete a user based on a userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteUserAsync(int userId)
        {
            // Declare and initialize
            query = "DELETE FROM user WHERE user_id = @UserId";
            int rowsAffected = 0;

            // Execute the query
            rowsAffected = await _connection.ExecuteAsync(query, new { UserId = userId });

            // Check the number of rows found
            if (rowsAffected == 0)
            {
                // Return that the user was not found
                return new BaseResponse(404, "User not found");
            }
            // Return the success
            return new BaseResponse(200, "User account deleted successfully");
        }
    }
}
