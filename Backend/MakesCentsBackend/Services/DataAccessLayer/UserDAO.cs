/*
 * Gianna Ross
 * Makes Cents
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
        private readonly MySqlConnection _connection;
        
        /// <summary>
        /// Parameterized constructor for the User DAO
        /// Takes in DI parameters
        /// </summary>
        /// <param name="connection"></param>
        public UserDAO(MySqlConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Add a new user based on a user model
        /// </summary>
        /// <param name="user">the data for the new user</param>
        /// <returns>The Id of the new user</returns>
        /// 
        public async Task<RegisterResponse> RegisterUserAsync(RegisterRequest user)
        {
            // Declare and initialize
            query =
                """
                INSERT INTO user (username, email, password_hash, is_dark_mode)
                VALUES (@Username, @Email, @PasswordHash, FALSE);
                SELECT LAST_INSERT_ID();
                """;
            int userId;
            string errorColumn = "";

            // Use dapper to execute the request and get the users new id
            try
            {
                // Execute the async scalar method using dapper
                userId = await _connection.ExecuteScalarAsync<int>(query, user);
            }
            catch (MySqlException ex)
            {
                // Check if the error is 1062
                if (ex.Number == 1062)
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
                    return new RegisterResponse(400, $"{errorColumn} already exists", -1);
                }
                return new RegisterResponse(500, $"{ex.Number}: {ex.Message}", -1);
            }
            catch (Exception ex)
            {
                // Return the issue
                return new RegisterResponse(500, $"{ex.Message}", -1);
            }
            // Return the new id
            return new RegisterResponse(201, "User registered successfully", userId);
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
            UserEntityModel? foundUser;

            // Normalize the passed in username
            usernameOrEmail = usernameOrEmail.ToLower();
            try
            {
                // Get the user from the executed query
                foundUser = await _connection.QuerySingleOrDefaultAsync<UserEntityModel>(query, new { UsernameOrEmail = usernameOrEmail });
            }
            catch (Exception ex)
            {
                // Return any errors
                return new LoginResponse(400, ex.Message);
            }
            // Make sure a user was found
            if (foundUser != null)
            {
                // Return the found users id and a success message
                return new LoginResponse(200, "User Found", foundUser.UserId, foundUser.PasswordHash);
            }
            else
            {
                // Return an error that the user was not found
                return new LoginResponse(400, "User not found");
            }
        }

        /// <summary>
        /// Get a user from the database based on a users id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<GetUserResponse> GetUserAsync(int userId)
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
            GetUserDTOModel? foundUser;

            try
            {
                // Get the user from the executed query
                foundUser = await _connection.QuerySingleOrDefaultAsync<GetUserDTOModel>(query, new { UserId = userId });
            }
            catch (Exception ex)
            {
                // Return any errors
                return new GetUserResponse(400, ex.Message);
            }
            // Make sure a user was found
            if (foundUser != null)
            {
                // Return the found users id and a success message
                return new GetUserResponse(200, "User Found", foundUser);
            }
            else
            {
                // Return an error that the user was not found
                return new GetUserResponse(404, "User not found");
            }
        }

        /// <summary>
        /// Async method to update a users information
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdateUserAsync(EditUserRequest user)
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
                return new BaseIdResponse(400, "No fields to update");
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
            catch (MySqlException ex)
            {
                // Check if the error is 1062
                if (ex.Number == 1062)
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
                    return new BaseIdResponse(400, $"{errorColumn} already exists");
                }
                // Check if the error was due to the input being too long
                if (ex.Number == 1406)
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
                    return new BaseIdResponse(400, $"{errorColumn} is too long");
                }
                return new BaseIdResponse(500, $"{ex.Number}: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Return the issue
                return new BaseIdResponse(500, $"{ex.Message}");
            }

            // Make sure the row was affected
            if (rowsAffected == 1)
            {
                if (user.UserId is int userId)
                {
                    return new BaseIdResponse(200, "Update was successful", userId);
                }
                else
                {
                    return new BaseIdResponse(400, "An error occurred");
                }
            }
            else if (rowsAffected == 0)
            {
                return new BaseIdResponse(404, "User not found");
            }
            else
            {
                return new BaseIdResponse(400, "An error occurred");
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
            query = """
                DELETE FROM user 
                WHERE user_id = @UserId
                """;
            int rowsAffected;

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
