using System.Security.Claims;

namespace ZakupekApi.Wrapper.Users;

/// <summary>
/// Extension methods for ClaimsPrincipal
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the user ID from the claims principal.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <returns>The user ID or 0 if not found or invalid.</returns>
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(c => c.Type == ClaimTypes.Sid)?.Value;
        
        return string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId) 
            ? 0 
            : userId;
    }
}