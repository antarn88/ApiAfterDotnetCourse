using ApiAfterDotnetCourse.Data.Entities;
using ApiAfterDotnetCourse.Data.Enums;
using ApiAfterDotnetCourse.Data.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ApiAfterDotnetCourse.Data.SeedIdentityData;

public class UserSeedService : IUserSeedService
{
    private readonly UserManager<ApplicationUser> userManager;

    public UserSeedService(UserManager<ApplicationUser> userManager)
    {
        this.userManager = userManager;
    }

    public async Task SeedUserAsync()
    {
        if (!(await userManager.GetUsersInRoleAsync("Admins")).Any())
        {
            var user = new ApplicationUser
            {
                UserName = "admin@example.hu",
                Email = "admin@example.hu",
                EmailConfirmed = true,
                Name = "MockAdmin",
                UserType = UserType.Admin,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var createResult = await userManager.CreateAsync(user, "P@ssword1");
            if (userManager.Options.SignIn.RequireConfirmedAccount)
            {
                // Regisztrációt meg kell erősíteni.
                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var result = await userManager.ConfirmEmailAsync(user, code);
            }
            var addToRoleResult = await userManager.AddToRoleAsync(user, "Admins");
            if (!createResult.Succeeded || !addToRoleResult.Succeeded)
            {
                throw new ApplicationException("Nem sikerült létrehozni az adminisztrátor felhasználót: " +
                  string.Join(", ", createResult.Errors.Concat(addToRoleResult.Errors).Select(x => x.Description)));
            }
        }

        if (!(await userManager.GetUsersInRoleAsync("Users")).Any())
        {
            var user = new ApplicationUser
            {
                UserName = "mock-user@example.hu",
                Email = "mock-user@example.hu",
                EmailConfirmed = true,
                Name = "MockUser",
                UserType = UserType.User,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var createResult = await userManager.CreateAsync(user, "P@ssword1");
            if (userManager.Options.SignIn.RequireConfirmedAccount)
            {
                // Regisztrációt meg kell erősíteni.
                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var result = await userManager.ConfirmEmailAsync(user, code);
            }
            var addToRoleResult = await userManager.AddToRoleAsync(user, "Users");

            if (!createResult.Succeeded || !addToRoleResult.Succeeded)
            {
                throw new ApplicationException("Nem sikerült létrehozni a user felhasználót: " +
                  string.Join(", ", createResult.Errors.Concat(addToRoleResult.Errors).Select(x => x.Description)));
            }
        }
    }
}
