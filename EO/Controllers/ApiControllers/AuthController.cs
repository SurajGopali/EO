using EO.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager,
                          TokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            PhoneNumber = request.MobileNumber,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Registration failed",
                Data = result.Errors
            });
        }

        return Ok(new AuthResponse
        {
            Success = true,
            Message = "Registration successful",
            Data = null
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new AuthResponse
            {
                Success = false,
                Message = "Invalid credentials"
            });
        }

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _userManager.UpdateAsync(user);

        return Ok(new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            Data = new
            {
                user = new
                {
                    id = user.Id,
                    firstName = user.FirstName,
                    middleName = user.MiddleName,
                    lastName = user.LastName,
                    fullName = $"{user.FirstName} {user.LastName}",
                    email = user.Email,
                    mobile = user.PhoneNumber,
                    membershipType = user.MembershipType,
                    profileImage = user.ProfileImage
                },
                token = new
                {
                    accessToken,
                    refreshToken,
                    expiresIn = 3600
                }
            }
        });
    }
}