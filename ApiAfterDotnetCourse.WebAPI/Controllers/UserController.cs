using ApiAfterDotnetCourse.Bll.Dtos;
using ApiAfterDotnetCourse.Bll.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiAfterDotnetCourse.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public UserController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://localhost:5001";

        var result = await _userService.RegisterUserAsync(dto, (userId, token) =>
        {
            return $"{baseUrl}/api/users/confirm-email?userId={userId}&token={Uri.EscapeDataString(token)}";
        });

        if (result.Succeeded)
        {
            return Ok("User registered successfully. Please confirm your email.");
        }

        return BadRequest(result.Errors.Select(e => e.Description));
    }

    // [HttpGet("confirm-email")]
    // public async Task<IActionResult> ConfirmEmail(string userId, string token)
    // {
    //     var user = await _userService.GetUserByIdAsync(int.Parse(userId));

    //     if (user == null) return BadRequest("Invalid user ID.");

    //     var result = await _userService.ConfirmEmailAsync(user, token);

    //     if (result.Succeeded)
    //     {
    //         return Ok("Email confirmed successfully.");
    //     }

    //     return BadRequest("Invalid token.");
    // }
}


