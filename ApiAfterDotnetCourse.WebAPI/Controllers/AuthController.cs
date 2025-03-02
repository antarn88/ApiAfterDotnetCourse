using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiAfterDotnetCourse.Bll.Dtos;
using ApiAfterDotnetCourse.Bll.Interfaces;
using ApiAfterDotnetCourse.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiAfterDotnetCourse.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailSender _emailSender;
    private readonly IUserService _userService;

    public AuthController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        IEmailSender emailSender,
        IUserService userService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
        _emailSender = emailSender;
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return Unauthorized("Invalid credentials.");
        }

        var result = await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);
        if (!result.Succeeded)
        {
            return Unauthorized("Invalid credentials.");
        }

        // Token generálás
        var token = await GenerateJwtToken(user);

        return Ok(new
        {
            accessToken = token,
            loggedInUser = new
            {
                user.Id,
                user.Email,
                user.UserName,
                user.Name
            }
        });
    }

    // TODO esetleg kiszervezni servicebe!
    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Role-ok hozzáadása
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            expires: DateTime.UtcNow.AddHours(24),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // 1. Elfelejtett jelszó kérés (email kiküldése tokennel)
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Ok("If this email is registered, you will receive a password reset link.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // TODO kiszervezni a Base URL-t!
        var resetLink = $"https://localhost:7264/api/auth/reset-password?email={user.Email}&token={Uri.EscapeDataString(token)}";

        await _emailSender.SendEmailAsync(user.Email!, "Password Reset",
            $"Click <a href='{resetLink}'>here</a> to reset your password.");

        return Ok("If this email is registered, you will receive a password reset link.");
    }

    [HttpGet("reset-password")]
    public async Task<IActionResult> ResetPassword([FromQuery] string email, [FromQuery] string token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
        {
            return BadRequest("Invalid password reset request.");
        }

        // Token ellenőrzése a service-ben
        var tokenUsed = await _userService.IsTokenUsed(token);
        if (tokenUsed)
        {
            return BadRequest("This password reset link has already been used.");
        }

        return Ok(new { Email = email, Token = token });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return BadRequest("Invalid request.");
        }

        // Ellenőrizzük, hogy a token már fel lett-e használva
        var tokenUsed = await _userService.IsTokenUsed(model.Token);
        if (tokenUsed)
        {
            return BadRequest("This password reset link has already been used.");
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        if (result.Succeeded)
        {
            // Token mentése az adatbázisba
            await _userService.AddUsedTokenAsync(model.Email, model.Token);
            return Ok("Password has been reset successfully.");
        }

        return BadRequest(result.Errors);
    }
}