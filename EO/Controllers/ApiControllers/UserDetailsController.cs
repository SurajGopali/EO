using EO.Models;
using EO.Services.Common;
using EO.Services.Profile;
using EO.WebContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

[Route("api/[controller]")]
[ApiController]
public class UserDetailsController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;
    private readonly IProfileService _profileService;
    private readonly IMemberService _memberService;
    private readonly ICommonService _commonService;

    public UserDetailsController(
        UserManager<ApplicationUser> userManager,
        AppDbContext context,
        IProfileService profileService,
        IMemberService memberService,ICommonService commonService)
    {
        _userManager = userManager;
        _context = context;
        _profileService = profileService;
        _memberService = memberService;
        _commonService = commonService;
    }

    [Authorize(AuthenticationSchemes = "Jwt")]
    [HttpPost("update-profile")]
    public async Task<IActionResult> UpsertProfile(
     [FromForm] string data,
     [FromForm] IFormFile? spouseProfileImageFile,
     [FromForm] List<IFormFile>? childrenFiles)
    {
        if (string.IsNullOrEmpty(data))
            return BadRequest("Invalid request");

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var request = JsonSerializer.Deserialize<UpdateProfileRequest>(
            data,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (request == null)
            return BadRequest("Invalid JSON payload");

        var targetUserId = string.IsNullOrEmpty(request.UserId)
            ? userId
            : request.UserId;

        if (spouseProfileImageFile != null && request.Spouse != null)
        {
            request.Spouse.ProfileImage =
                await _commonService.SaveFileAsync(
                    spouseProfileImageFile,
                    "spouse",
                    targetUserId);
        }

        if (request.Children != null && childrenFiles != null)
        {
            for (int i = 0; i < request.Children.Count; i++)
            {
                var file = childrenFiles.ElementAtOrDefault(i);

                if (file != null && file.Length > 0)
                {
                    request.Children[i].ProfileImage =
                        await _commonService.SaveFileAsync(
                            file,
                            "children",
                            targetUserId);
                }
            }
        }

        var result = await _profileService.UpsertProfileAsync(request, targetUserId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize(AuthenticationSchemes = "Jwt")]
    [HttpGet("members")]
    public async Task<IActionResult> GetMembers([FromQuery] bool New = false)
    {
        var members = await _memberService.GetMembersAsync(New);

        return Ok(new
        {
            success = true,
            message = New ? "New members" : "All members",
            members = members
        });
    }

    private static string GetFullName(ApplicationUser u)
    {
        return string.Join(" ", new[]
        {
            u.FirstName,
            u.MiddleName,
            u.LastName
        }.Where(x => !string.IsNullOrWhiteSpace(x)));
    }


    [Authorize(AuthenticationSchemes = "Jwt")]
    [HttpGet("member-details")]
    public async Task<IActionResult> GetMemberDetails([FromQuery] string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest(new
            {
                success = false,
                message = "Id is required"
            });
        }

        var member = await _memberService.GetMemberDetailAsync(id);

        if (member == null)
        {
            return NotFound(new
            {
                success = false,
                message = "User not found"
            });
        }

        return Ok(new
        {
            success = true,
            message = "Member Details",
            memberDetail = member
        });
    }

    [Authorize(AuthenticationSchemes = "Jwt")]
    [HttpGet("anniversaries")]
    public async Task<IActionResult> GetAnniversaries()
    {
        var anniversaries = await _memberService.GetUpcomingAnniversariesAsync();

        return Ok(new
        {
            success = true,
            message = "Anniversaries fetched successfully",
            data = anniversaries
        });
    }

    [Authorize(AuthenticationSchemes = "Jwt")]
    [HttpGet("birthdays")]
    public async Task<IActionResult> GetBirthdays()
    {
        var birthdays = await _memberService.GetUpcomingBirthdaysAsync();

        return Ok(new
        {
            success = true,
            message = "Upcoming birthdays",
            data = birthdays
        });
    }

    [Authorize(AuthenticationSchemes = "Jwt")]
    [HttpGet("new-members")]
    public async Task<IActionResult> GetNewMembers()
    {
        var members = await _memberService.GetNewJoineesThisMonthAsync();

        return Ok(new
        {
            success = true,
            message = "New members this month",
            data = members
        });
    }
}