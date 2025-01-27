using ApiAfterDotnetCourse.Bll.Dtos;
using ApiAfterDotnetCourse.Bll.Interfaces;
using ApiAfterDotnetCourse.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ApiAfterDotnetCourse.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(IUserService userService, IConfiguration configuration, UserManager<ApplicationUser> userManager)
    {
        _userService = userService;
        _configuration = configuration;
        _userManager = userManager;
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
        var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://localhost:7264";

        var result = await _userService.RegisterUserAsync(dto, (userId, token) =>
        {
            return $"{baseUrl}/api/User/confirm-email?userId={userId}&token={Uri.EscapeDataString(token)}";
        });

        if (result.Succeeded)
        {
            return Ok("User registered successfully. Please confirm your email.");
        }

        return BadRequest(result.Errors.Select(e => e.Description));
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) return BadRequest("UserId or token is missing.");

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return NotFound("User not found.");

        token = Uri.UnescapeDataString(token); // Dekódolás szükséges lehet

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded) return Content("Email confirmed successfully.");

        return BadRequest("Invalid token or email confirmation failed.");
    }
}
