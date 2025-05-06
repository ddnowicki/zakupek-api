using System.Security.Claims;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using ZakupekApi.Db.Data;
using ZakupekApi.Wrapper.Abstraction.Users;
using ZakupekApi.Wrapper.Contract.Auth.Response;

namespace ZakupekApi.Wrapper.Users;

public class UserService(AppDbContext dbContext) : IUserService
{
    public async Task<ErrorOr<UserProfileResponse>> GetUserProfile(int userId)
    {
        var user = await dbContext.Users
            .Include(u => u.Ages)
            .Include(u => u.DietaryPreferences)
            .Include(u => u.ShoppingLists)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return Error.NotFound("User not found");
        }

        var userProfileResponse = new UserProfileResponse(
            user.Id,
            user.Email,
            user.UserName,
            user.HouseholdSize,
            user.Ages.Select(a => a.Age).ToList(),
            user.DietaryPreferences.Select(dp => dp.Preference).ToList(),
            user.CreatedAt,
            user.ShoppingLists.Count
        );

        return userProfileResponse;
    }
}
