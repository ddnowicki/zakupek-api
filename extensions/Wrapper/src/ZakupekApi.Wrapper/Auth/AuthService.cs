using System.Security.Claims;
using ErrorOr;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using ZakupekApi.Db.Data;
using ZakupekApi.Db.Models;
using ZakupekApi.Wrapper.Abstraction.Auth;
using ZakupekApi.Wrapper.Contract.Auth.Response;

namespace ZakupekApi.Wrapper.Auth;

public class AuthService(AppDbContext dbContext) : IAuthService
{
    public async Task<ErrorOr<AuthResponse>> Login(string email, string password)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        
        if (user == null || !verifyPassword(password, user.HashedPassword))
        {
            return Error.Unauthorized();
        }
        
        var authResult = generateAuthenticationResult(user);

        return authResult;
    }

    public async Task<ErrorOr<AuthResponse>> Register(string email, string password)
    {
        var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        
        if (existingUser != null)
        {
            Error.Conflict("User already exists");
        }
        
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        
        var newUser = new User
        {
            Email = email,
            HashedPassword = hashedPassword
        };
        
        dbContext.Users.Add(newUser);
        
        await dbContext.SaveChangesAsync();
        
        var authResult = generateAuthenticationResult(newUser);
        
        return authResult;
    }

    private bool verifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
    
    private AuthResponse generateAuthenticationResult(User user)
    {
        var jwtToken = JwtBearer.CreateToken(
            o =>
            {
                o.ExpireAt = DateTime.UtcNow.AddDays(1);
                o.User.Claims.Add((ClaimTypes.Email, user.Email));
                o.User.Claims.Add((ClaimTypes.Sid, user.Id.ToString()));
                o.User["UserId"] = user.Id.ToString();
            });

        return new(user.Id, jwtToken);
    }
}
