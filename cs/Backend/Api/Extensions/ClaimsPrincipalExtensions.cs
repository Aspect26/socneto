using System.Security.Claims;

namespace Socneto.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return null;

            var identifierString = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(identifierString, out var identifierInt))
            {
                return identifierInt;
            }

            return null;
        }
    }
}