using ApiAfterDotnetCourse.Data.Enums;

namespace ApiAfterDotnetCourse.Bll.Dtos;

public class CreateUserDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserType UserType { get; set; } = UserType.User;
}
