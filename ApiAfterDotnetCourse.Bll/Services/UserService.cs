using ApiAfterDotnetCourse.Bll.Dtos;
using ApiAfterDotnetCourse.Bll.Interfaces;
using ApiAfterDotnetCourse.Data;
using ApiAfterDotnetCourse.Data.Entities;
using ApiAfterDotnetCourse.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace ApiAfterDotnetCourse.Bll.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IEmailSender _emailSender;
    private readonly ApiAfterDotnetCourseDBContext _dbContext;

    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IEmailSender emailSender,
        ApiAfterDotnetCourseDBContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _emailSender = emailSender;
        _dbContext = dbContext;
    }

    public async Task<List<ApplicationUser>> GetAllUsersAsync()
    {
        return await _userManager.Users.ToListAsync();
    }

    public async Task<IdentityResult> RegisterUserAsync(CreateUserDto dto, Func<string, string, string> generateConfirmationLink)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            Name = dto.Name,
            UserType = UserType.User, // TODO Később ezt rugalmasabbá tenni!
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded) return result;

        // Role hozzárendelés
        var roleName = user.UserType switch
        {
            UserType.Admin => "Admins",
            UserType.User => "Users",
            _ => "Users"
        };

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole<int> { Name = roleName });
        }

        await _userManager.AddToRoleAsync(user, roleName);

        // E-mail megerősítés link generálás és küldés
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = generateConfirmationLink(user.Id.ToString(), Uri.EscapeDataString(token));

        await _emailSender.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

        return result;
    }

    public async Task<bool> IsTokenUsed(string token)
    {
        return await _dbContext.UsedTokens.AnyAsync(t => t.Token == token);
    }

    public async Task AddUsedTokenAsync(string email, string token)
    {
        _dbContext.UsedTokens.Add(new UsedToken
        {
            Email = email,
            Token = token,
            UsedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
    }
}
