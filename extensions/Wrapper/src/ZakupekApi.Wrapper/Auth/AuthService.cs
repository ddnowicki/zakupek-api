using System.Security.Claims;
using ErrorOr;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using ZakupekApi.Db.Data;
using ZakupekApi.Db.Models;
using ZakupekApi.Wrapper.Abstraction.Auth;
using ZakupekApi.Wrapper.Abstraction.Users;
using ZakupekApi.Wrapper.Contract.Auth.Request;
using ZakupekApi.Wrapper.Contract.Auth.Response;

namespace ZakupekApi.Wrapper.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IUserService _userService;

    public AuthService(AppDbContext dbContext, IUserService userService)
    {
        _dbContext = dbContext;
        _userService = userService;
    }

    public async Task<ErrorOr<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !verifyPassword(request.Password, user.HashedPassword))
        {
            return Error.Unauthorized();
        }

        var authResult = generateAuthenticationResult(user);

        return authResult;
    }

    public async Task<ErrorOr<AuthResponse>> Register(RegisterRequest request)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingUser != null)
        {
            return Error.Conflict("User already exists");
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var newUser = new User
        {
            Email = request.Email,
            UserName = request.UserName,
            HashedPassword = hashedPassword,
            HouseholdSize = request.HouseholdSize,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(newUser);

        // Process ages after user is created
        if (request.Ages != null && request.Ages.Any())
        {
            foreach (var age in request.Ages)
            {
                newUser.Ages.Add(new UserAge
                {
                    Age = age,
                    User = newUser
                });
            }
        }

        // Process dietary preferences after user is created
        if (request.DietaryPreferences != null && request.DietaryPreferences.Any())
        {
            foreach (var preference in request.DietaryPreferences)
            {
                newUser.DietaryPreferences.Add(new UserDietaryPreference
                {
                    Preference = preference,
                    User = newUser
                });
            }
        }

        await _dbContext.SaveChangesAsync();

        var authResult = generateAuthenticationResult(newUser);

        return authResult;
    }

    public async Task<ErrorOr<UserProfileResponse>> GetProfile(int userId)
    {
        // Delegate to UserService
        return await _userService.GetUserProfile(userId);
    }

    private bool verifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    private AuthResponse generateAuthenticationResult(User user)
    {
        var expiresAt = DateTime.UtcNow.AddDays(1);
        var jwtToken = JwtBearer.CreateToken(
            o =>
            {
                o.ExpireAt = expiresAt;
                o.User.Claims.Add((ClaimTypes.Email, user.Email));
                o.User.Claims.Add((ClaimTypes.Sid, user.Id.ToString()));
                o.User.Claims.Add((ClaimTypes.Name, user.UserName));
                o.User["UserId"] = user.Id.ToString();
            });

        return new(
            user.Id,
            user.UserName,
            jwtToken,
            expiresAt);
    }
}
