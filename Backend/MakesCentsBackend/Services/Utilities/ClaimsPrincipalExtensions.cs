/*
 * Gianna Ross
 * File Created: 1/23/2026
 * File Last Updated: 1/23/2026
 * Makes Cents - Claims Principal Extensions
 * Sources: https://chatgpt.com/c/6962a61c-d0f8-8333-99ec-aee778f83ea2
 */
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace MakesCentsBackend.Services.Utilities
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            Claim? userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID claim missing");

            return int.Parse(userIdClaim.Value);
        }
    }
}
