namespace ZakupekApi.Wrapper.Contract.Dto.Auth;

/// <summary>
/// Data transfer object for authentication results containing tokens and user information
/// </summary>
public class AuthenticationResultDto
{
    /// <summary>
    /// User identifier
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = null!;
    
    /// <summary>
    /// Access token for API authorization (JWT)
    /// </summary>
    public string AccessToken { get; set; } = null!;
    
    /// <summary>
    /// Token expiration timestamp
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}
