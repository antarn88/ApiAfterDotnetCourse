using ApiAfterDotnetCourse.Data.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ApiAfterDotnetCourse.Data.SeedIdentityData;

public class RoleSeedService : IRoleSeedService
{
    private readonly RoleManager<IdentityRole<int>> roleManager;

    public RoleSeedService(RoleManager<IdentityRole<int>> roleManager)
    {
        this.roleManager = roleManager;
    }

    public async Task SeedRoleAsync()
    {
        if (!await roleManager.RoleExistsAsync("Admins"))
        {
            await roleManager.CreateAsync(new IdentityRole<int> { Name = "Admins" });
        }

        if (!await roleManager.RoleExistsAsync("Users"))
        {
            await roleManager.CreateAsync(new IdentityRole<int> { Name = "Users" });
        }
    }
}
