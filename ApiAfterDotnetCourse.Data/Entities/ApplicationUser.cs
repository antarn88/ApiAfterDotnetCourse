using ApiAfterDotnetCourse.Data.Enums;
using Microsoft.AspNetCore.Identity;

namespace ApiAfterDotnetCourse.Data.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public string? Name { get; set; }
    public UserType UserType { get; set; } = UserType.User;
}
