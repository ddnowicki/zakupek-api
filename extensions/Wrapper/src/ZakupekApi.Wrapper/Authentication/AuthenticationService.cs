using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ZakupekApi.Db.Data;
using ZakupekApi.Db.Models;
using ZakupekApi.Wrapper.Abstraction.Authentication;
using ZakupekApi.Wrapper.Contract.Commands.Auth;
using ZakupekApi.Wrapper.Contract.Dto.Auth;

namespace ZakupekApi.Wrapper.Authentication;

/// <summary>
/// Implementation of authentication service for user registration and login
/// </summary>
public class AuthenticationService(AppDbContext dbContext, IConfiguration configuration) : IAuthenticationService
{
    /// <inheritdoc />
    public async Task<AuthenticationResultDto> RegisterUserAsync(RegisterUserCommand command)
    {
        // Check if user with the same email already exists
        if (await dbContext.Users.AnyAsync(u => u.Email == command.Email))
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        // Hash the password
        var passwordHash = hashPassword(command.Password);

        // Create a new user
        var user = new User
        {
            Email = command.Email,
            HashedPassword = passwordHash,
            CreatedAt = DateTime.UtcNow,
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        // Generate JWT token
        var authResult = generateAuthenticationResult(user);

        return authResult;
    }

    /// <inheritdoc />
    public async Task<AuthenticationResultDto> LoginAsync(LoginUserCommand command)
    {
        // Find user by email
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == command.Email);
        
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Verify password hash
        if (!verifyPassword(command.Password, user.HashedPassword))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Generate JWT token
        var authResult = generateAuthenticationResult(user);

        return authResult;
    }

    /// <summary>
    /// Hashes a password using BCrypt
    /// </summary>
    private string hashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    /// <summary>
    /// Verifies a password against a hash using BCrypt
    /// </summary>
    private bool verifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    /// <summary>
    /// Generates JWT tokens and creates an authentication result
    /// </summary>
    private AuthenticationResultDto generateAuthenticationResult(User user)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var issuer = jwtSettings["Issuer"] ?? "ZakupekApi";
        var audience = jwtSettings["Audience"] ?? "ZakupekApiClients";

        var expiryTime = DateTime.UtcNow.AddYears(10);
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new([
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            ]),
            Expires = expiryTime,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);
        
        return new()
        {
            UserId = user.Id,
            Email = user.Email,
            AccessToken = accessToken,
            ExpiresAt = expiryTime,
        };
    }
}
