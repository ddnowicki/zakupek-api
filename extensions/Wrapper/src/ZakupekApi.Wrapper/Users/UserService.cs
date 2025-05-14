using ErrorOr;
using Microsoft.EntityFrameworkCore;
using ZakupekApi.Db.Data;
using ZakupekApi.Db.Models;
using ZakupekApi.Wrapper.Abstraction.Users;
using ZakupekApi.Wrapper.Contract.Auth.Request;
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

    public async Task<ErrorOr<UserProfileResponse>> UpdateProfile(int userId, UpdateUserProfileRequest request)
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

        user.UserName = request.UserName;
        user.HouseholdSize = request.HouseholdSize;

        // Update ages
        if (request.Ages != null)
        {
            user.Ages.Clear();
            foreach (var age in request.Ages)
            {
                user.Ages.Add(new UserAge
                {
                    Age = age,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        // Update dietary preferences
        if (request.DietaryPreferences != null)
        {
            user.DietaryPreferences.Clear();
            foreach (var preference in request.DietaryPreferences)
            {
                user.DietaryPreferences.Add(new UserDietaryPreference
                {
                    Preference = preference,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await dbContext.SaveChangesAsync();

        return new UserProfileResponse(
            user.Id,
            user.Email,
            user.UserName,
            user.HouseholdSize,
            user.Ages.Select(a => a.Age).ToList(),
            user.DietaryPreferences.Select(dp => dp.Preference).ToList(),
            user.CreatedAt,
            user.ShoppingLists.Count
        );
    }
}
