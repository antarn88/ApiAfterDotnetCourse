using ApiAfterDotnetCourse.Bll.Dtos;
using ApiAfterDotnetCourse.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace ApiAfterDotnetCourse.Bll.Interfaces;

public interface IUserService
{
    Task<List<ApplicationUser>> GetAllUsersAsync();
    Task<IdentityResult> RegisterUserAsync(CreateUserDto dto, Func<string, string, string> generateConfirmationLink);
    Task<bool> IsTokenUsed(string token);
    Task AddUsedTokenAsync(string email, string token);
}
