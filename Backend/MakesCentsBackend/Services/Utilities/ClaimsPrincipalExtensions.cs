/*
 * Gianna Ross
 * Makes Cents
 * Sources: https://chatgpt.com/c/6962a61c-d0f8-8333-99ec-aee778f83ea2
 */
using System.Security.Claims;

namespace MakesCentsBackend.Services.Utilities
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Get the UserId out of the JWT token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public static int GetUserId(this ClaimsPrincipal user)
        {
            Claim? userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User ID claim missing");
            }

            return int.Parse(userIdClaim.Value);
        }
    }
}
