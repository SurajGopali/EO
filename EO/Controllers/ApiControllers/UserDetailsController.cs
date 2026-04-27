using EO.Models;
using EO.Services.Profile;
using EO.WebContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

[Route("api/[controller]")]
[ApiController]
public class UserDetailsController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;
    private readonly IProfileService _profileService;

    public UserDetailsController(
        UserManager<ApplicationUser> userManager,
        AppDbContext context,
        IProfileService profileService)
    {
        _userManager = userManager;
        _context = context;
        _profileService = profileService;
    }

    [Authorize(AuthenticationSchemes = "Jwt")]
    [HttpPost("update-profile")]
    public async Task<IActionResult> UpsertProfile(UpdateProfileRequest request)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var result = await _profileService.UpsertProfileAsync(request, userId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [Authorize(AuthenticationSchemes = "Jwt")]
    [HttpGet("members")]
    public async Task<IActionResult> GetMembers([FromQuery] bool New = false)
    {
        var users = await _userManager.Users.ToListAsync();

        if (New)
        {
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);

            users = users
                .Where(u => u.JoinedDate != null && u.JoinedDate >= oneMonthAgo)
                .ToList();
        }

        var userIds = users.Select(x => x.Id).ToList();

        var profiles = await _context.UserProfiles
            .Where(x => userIds.Contains(x.UserId))
            .ToListAsync();

        var companies = await _context.CompanyDetails
            .Where(x => userIds.Contains(x.UserId))
            .ToListAsync();

        var result = users.Select(u =>
        {
            var profile = profiles.FirstOrDefault(p => p.UserId == u.Id);
            var company = companies.FirstOrDefault(c => c.UserId == u.Id);

            return new MemberDto
            {
                Id = u.Id,
                Name = GetFullName(u),
                EoRole = u.EoRole,
                JoinedDate = u.JoinedDate,

                ProfileImage = u.ProfileImage,

                CompanyName = company?.CompanyName,
                CompanyRole = company?.CompanyRole,
                Designation = company?.Designation,

                Birthday = profile?.DateOfBirth,
                Anniversary = profile?.AnniversaryDate,
            };
        }).ToList();

        return Ok(new
        {
            success = true,
            message = New ? "New members" : "All members",
            members = result
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
        var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
        {
            return NotFound(new
            {
                success = false,
                message = "User not found"
            });
        }

        var profile = await _context.UserProfiles
            .FirstOrDefaultAsync(x => x.UserId == id);

        var company = await _context.CompanyDetails
            .FirstOrDefaultAsync(x => x.UserId == id);

        var social = await _context.UserSocialLinks
            .FirstOrDefaultAsync(x => x.UserId == id);

        // -----------------------------
        // SAFE: profile-dependent data
        // -----------------------------
        Spouse spouse = null;
        List<Child> children = new();

        if (profile != null)
        {
            spouse = await _context.Spouses
                .Include(x => x.SocialLinks)
                .Include(x => x.ProfessionalDetails)
                .FirstOrDefaultAsync(x => x.UserProfileId == profile.Id);

            children = await _context.Children
                .Where(x => x.UserProfileId == profile.Id)
                .ToListAsync();
        }

        // -----------------------------
        // SAFE DTO BUILD
        // -----------------------------
        var data = new MemberDetailDto
        {
            Id = user.Id,
            Name = GetFullName(user),
            EoRole = user.EoRole,

            Email = user.Email,
            Phone = user.PhoneNumber,
            ProfileImage = user.ProfileImage,

            CompanyName = company?.CompanyName,
            CompanyRole = company?.CompanyRole,
            Designation = company?.Designation,

            Location = profile?.Address,
            Bio = profile?.Bio,
            JoinedDate = user.JoinedDate,
            Dob = profile?.DateOfBirth,

            // ---------------- SOCIAL ----------------
            SocialLinks = social == null ? null : new SocialDto
            {
                Linkedin = social.LinkedIn,
                Twitter = social.X,
                Facebook = social.Facebook,
                Instagram = social.Instagram,
                Website = social.Website
            },

            // ---------------- SPOUSE ----------------
            Spouse = spouse == null ? null : new SpouseDto
            {
                Name = spouse.Name,
                DateOfBirth = spouse.DateOfBirth,
                Phone = spouse.Phone,
                Email = spouse.Email,
                ProfileImage = spouse.ProfileImage,

                SocialLinks = spouse.SocialLinks == null ? null : new SocialLinksDto
                {
                    Facebook = spouse.SocialLinks.Facebook,
                    Instagram = spouse.SocialLinks.Instagram,
                    LinkedIn = spouse.SocialLinks.LinkedIn,
                    X = spouse.SocialLinks.X,
                    Website = spouse.SocialLinks.Website
                },

                Professional = spouse.ProfessionalDetails == null ? null : new SpouseProfessionalDto
                {
                    CurrentCompany = spouse.ProfessionalDetails.CurrentCompany,
                    Position = spouse.ProfessionalDetails.Position,
                    ExperienceYears = spouse.ProfessionalDetails.ExperienceYears,

                    PreviousRoles = string.IsNullOrEmpty(spouse.ProfessionalDetails.PreviousRoles)
                        ? new List<string>()
                        : spouse.ProfessionalDetails.PreviousRoles
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .ToList(),

                    Expertise = string.IsNullOrEmpty(spouse.ProfessionalDetails.Expertise)
                        ? new List<string>()
                        : spouse.ProfessionalDetails.Expertise
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .ToList()
                }
            },

            // ---------------- CHILDREN ----------------
            Children = children?.Select(c => new ChildDto
            {
                Id = c.Id,
                Name = c.Name,
                School = c.School,
                Grade = c.Grade,
                DateOfBirth = c.DateOfBirth,
                ProfileImage = c.ProfileImage
            }).ToList() ?? new List<ChildDto>()
        };

        return Ok(new
        {
            success = true,
            message = "Member Details",
            memberDetail = data
        });
    }

    [Authorize(AuthenticationSchemes = "Jwt")]
    [HttpGet("anniversaries")]
    public async Task<IActionResult> GetAnniversaries()
    {
        var today = DateTime.UtcNow;

        var data = await (
            from profile in _context.UserProfiles
            join user in _userManager.Users on profile.UserId equals user.Id
            join spouse in _context.Spouses on profile.Id equals spouse.UserProfileId
            where profile.IsMarried == true && profile.AnniversaryDate != null
            select new
            {
                name1 = user.FirstName + " " + user.LastName,
                name2 = spouse.Name,
                anniversaryDate = profile.AnniversaryDate,
                phoneNo = user.PhoneNumber,
                profileImage1 = user.ProfileImage,
                profileImage2 = spouse.ProfileImage
            }
        ).ToListAsync();

        var result = data.Select(x =>
        {
            var years = today.Year - x.anniversaryDate.Value.Year;

            if (today.Date < x.anniversaryDate.Value.AddYears(years).Date)
                years--;

            return new
            {
                name1 = x.name1,
                name2 = x.name2,
                years = years,
                date = x.anniversaryDate.Value.ToString("MMM dd"),
                phoneNo = x.phoneNo,
                profileImage1 = x.profileImage1,
                profileImage2 = x.profileImage2
            };
        }).ToList();

        return Ok(new
        {
            success = true,
            message = "Anniversaries fetched successfully",
            data = result
        });
    }

    [Authorize(AuthenticationSchemes = "Jwt")]
    [HttpGet("birthdays")]
    public async Task<IActionResult> GetBirthdays()
    {
        var today = DateTime.UtcNow;

        var data = await (
            from profile in _context.UserProfiles
            join user in _userManager.Users on profile.UserId equals user.Id
            where profile.DateOfBirth != null
            select new
            {
                name = user.FirstName + " " + user.LastName,
                dob = profile.DateOfBirth,
                phoneNo = user.PhoneNumber,
                profileImage = user.ProfileImage
            }
        ).ToListAsync();

        var result = data.Select(x => new
        {
            name = x.name,
            date = x.dob.Value.ToString("MMM dd"),
            phoneNo = x.phoneNo,
            profileImage = x.profileImage
        }).ToList();

        return Ok(new
        {
            success = true,
            message = "Birthdays fetched successfully",
            data = result
        });
    }
}