using System.Security.Claims;

namespace Plantpedia.Helper
{
    public static class UserClaimsHelper
    {
        public static int? GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                return null;
            }

            var userIdString = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdString, out int userId))
            {
                return userId;
            }

            return null;
        }

        public static string? GetUsername(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                return null;
            }
            return principal.FindFirstValue(ClaimTypes.Name);
        }
    }
}
