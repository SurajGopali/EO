using EO.Models;
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

    public UserDetailsController(
        UserManager<ApplicationUser> userManager,
        AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [Authorize]
    [HttpPost("update-profile")]
    public async Task<IActionResult> UpsertProfile(UpdateProfileRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
         
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return NotFound(new { success = false, message = "User not found" });

            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(x => x.UserId == request.UserId);

            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = request.UserId
                };
                await _context.UserProfiles.AddAsync(profile);
            }

            if (request.PersonalDetails != null)
            {
                profile.DateOfBirth = request.PersonalDetails.DateOfBirth;
                profile.AnniversaryDate = request.PersonalDetails.AnniversaryDate;
                profile.Address = request.PersonalDetails.Address;
                profile.IsMarried = request.PersonalDetails.IsMarried;
                profile.Bio = request.PersonalDetails.Bio;
            }

            await _context.SaveChangesAsync();

       
            if (request.CompanyDetails != null)
            {
                var company = await _context.CompanyDetails
                    .FirstOrDefaultAsync(x => x.UserId == request.UserId);

                if (company == null)
                {
                    company = new CompanyDetails
                    {
                        UserId = request.UserId
                    };
                    await _context.CompanyDetails.AddAsync(company);
                }

                company.CompanyName = request.CompanyDetails.CompanyName;
                company.Designation = request.CompanyDetails.Designation;
                company.CompanyRole = request.CompanyDetails.Role;
            }

         
            if (request.SocialLinks != null)
            {
                var social = await _context.UserSocialLinks
                    .FirstOrDefaultAsync(x => x.UserId == request.UserId);

                if (social == null)
                {
                    social = new UserSocialLinks
                    {
                        UserId = request.UserId
                    };
                    await _context.UserSocialLinks.AddAsync(social);
                }

                social.Facebook = request.SocialLinks.Facebook;
                social.Instagram = request.SocialLinks.Instagram;
                social.LinkedIn = request.SocialLinks.LinkedIn;
                social.X = request.SocialLinks.X;
                social.Website = request.SocialLinks.Website;
            }

            Spouse? spouse = await _context.Spouses
                .FirstOrDefaultAsync(x => x.UserProfileId == profile.Id);

            if (profile.IsMarried && request.Spouse != null)
            {
                if (string.IsNullOrWhiteSpace(request.Spouse.Name))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Spouse name is required"
                    });
                }

                if (spouse == null)
                {
                    spouse = new Spouse
                    {
                        UserProfileId = profile.Id,
                        Name = request.Spouse.Name,
                        Phone = request.Spouse.Phone,
                        Email = request.Spouse.Email,
                        DateOfBirth = request.Spouse.DateOfBirth,
                        ProfileImage = request.Spouse.ProfileImage
                    };

                    await _context.Spouses.AddAsync(spouse);
                    await _context.SaveChangesAsync(); 
                }
                else
                {
                    spouse.Name = request.Spouse.Name;
                    spouse.Phone = request.Spouse.Phone;
                    spouse.Email = request.Spouse.Email;
                    spouse.DateOfBirth = request.Spouse.DateOfBirth;
                    spouse.ProfileImage = request.Spouse.ProfileImage;
                }

                if (request.Spouse.SocialLinks != null)
                {
                    var spouseSocial = await _context.SpouseSocialLinks
                        .FirstOrDefaultAsync(x => x.SpouseId == spouse.Id);

                    if (spouseSocial == null)
                    {
                        spouseSocial = new SpouseSocialLinks
                        {
                            SpouseId = spouse.Id
                        };
                        await _context.SpouseSocialLinks.AddAsync(spouseSocial);
                    }

                    spouseSocial.Facebook = request.Spouse.SocialLinks.Facebook;
                    spouseSocial.Instagram = request.Spouse.SocialLinks.Instagram;
                    spouseSocial.LinkedIn = request.Spouse.SocialLinks.LinkedIn;
                    spouseSocial.X = request.Spouse.SocialLinks.X;
                    spouseSocial.Website = request.Spouse.SocialLinks.Website;
                }

                if (request.Spouse.Professional != null)
                {
                    var prof = await _context.Set<SpouseProfessionalDetails>()
                        .FirstOrDefaultAsync(x => x.SpouseId == spouse.Id);

                    if (prof == null)
                    {
                        prof = new SpouseProfessionalDetails
                        {
                            SpouseId = spouse.Id
                        };
                        await _context.AddAsync(prof);
                    }

                    prof.CurrentCompany = request.Spouse.Professional.CurrentCompany;
                    prof.Position = request.Spouse.Professional.Position;
                    prof.ExperienceYears = request.Spouse.Professional.ExperienceYears;
                    prof.PreviousRoles = request.Spouse.Professional.PreviousRoles != null
                        ? string.Join(",", request.Spouse.Professional.PreviousRoles)
                        : null;

                    prof.Expertise = request.Spouse.Professional.Expertise != null
                        ? string.Join(",", request.Spouse.Professional.Expertise)
                        : null;
                }
            }
            else if (spouse != null)
            {
                _context.Spouses.Remove(spouse);
            }

            if (request.Children != null && request.Children.Any())
            {
                foreach (var child in request.Children)
                {
                    if (child.Id.HasValue)
                    {
                        var existing = await _context.Children
                            .FirstOrDefaultAsync(x =>
                                x.Id == child.Id &&
                                x.UserProfileId == profile.Id);

                        if (existing != null)
                        {
                            existing.Name = child.Name;
                            existing.School = child.School;
                            existing.Grade = child.Grade;
                            existing.DateOfBirth = child.DateOfBirth;
                            existing.ProfileImage = child.ProfileImage;
                        }
                    }
                    else
                    {
                        await _context.Children.AddAsync(new Child
                        {
                            UserProfileId = profile.Id,
                            Name = child.Name,
                            School = child.School,
                            Grade = child.Grade,
                            DateOfBirth = child.DateOfBirth,
                            ProfileImage = child.ProfileImage
                        });
                    }
                }
            }

        
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new
            {
                success = true,
                message = "Profile updated successfully",
                userId = request.UserId,
                profileId = profile.Id
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            return BadRequest(new
            {
                success = false,
                message = "Update failed",
                error = ex.Message
            });
        }
    }


    [Authorize]
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


    [Authorize]
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

        if (profile == null)
        {
            return Ok(new
            {
                success = true,
                message = "Member found (no profile yet)",
                data = new
                {
                    id = user.Id,
                    name = GetFullName(user),
                    eoRole = user.EoRole
                }
            });
        }

        var company = await _context.CompanyDetails
            .FirstOrDefaultAsync(x => x.UserId == id);

        var social = await _context.UserSocialLinks
            .FirstOrDefaultAsync(x => x.UserId == id);

        var spouse = await _context.Spouses
            .Include(x => x.SocialLinks)
            .Include(x => x.ProfessionalDetails)
            .FirstOrDefaultAsync(x => x.UserProfileId == profile.Id);

        var children = await _context.Children
            .Where(x => x.UserProfileId == profile.Id)
            .ToListAsync();

        var data = new
        {
            id = user.Id,
            name = GetFullName(user),
            eoRole = user.EoRole,
            companyName = company.CompanyName,
            companyRole = company.CompanyRole,
            designation = company.Designation,
            profileImage = user.ProfileImage,
            email = user.Email,
            phone = user.PhoneNumber,
            location = profile.Address,
            bio = profile?.Bio,
            joinedDate = user.JoinedDate,
            dob = profile?.DateOfBirth,

            socialLinks = social == null ? null : new
            {
                linkedin = social.LinkedIn,
                twitter = social.X,
                facebook = social.Facebook,
                instagram = social.Instagram,
                website = social.Website
            },

            family = new
            {
                spouse = spouse == null ? null : new
                {
                    id = spouse.Id,
                    name = spouse.Name,
                    companyRole = spouse.ProfessionalDetails?.Position,
                    profileImage = spouse?.ProfileImage,
                    email = spouse?.Email,
                    phone = spouse.Phone,

                    marriageAnniverssary = profile?.AnniversaryDate,
                    dob = spouse?.DateOfBirth,

                    socialLinks = spouse?.SocialLinks == null ? null : new
                    {
                        linkedin = spouse.SocialLinks.LinkedIn,
                        twitter = spouse.SocialLinks.X,
                        facebook = spouse.SocialLinks.Facebook,
                        instagram = spouse.SocialLinks.Instagram,
                        website = spouse.SocialLinks.Website
                    },


                    professional = spouse?.ProfessionalDetails == null ? null  : new
                    {
                        currentCompany = spouse.ProfessionalDetails.CurrentCompany,
                        position = spouse.ProfessionalDetails.Position,
                        experienceYear = spouse.ProfessionalDetails.ExperienceYears,

                        previousRoles = string.IsNullOrEmpty(spouse?.ProfessionalDetails?.PreviousRoles)
                    ? new List<string>()
                    : spouse.ProfessionalDetails.PreviousRoles
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToList(),

                        expertise = string.IsNullOrEmpty(spouse?.ProfessionalDetails?.Expertise)
                    ? new List<string>()
                    : spouse.ProfessionalDetails.Expertise
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToList()
                    }
                  
                }
            },

            children = children.Select(c => new
            {
                name = c.Name,
                dob = c.DateOfBirth,
                education = c.Grade,
                interests = new List<string>(),
                profileImage = ""
            })
        };

        return Ok(new
        {
            success = true,
            message = "Member Details",
            memberDetail = data
        });
    }

    [Authorize]
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

    [Authorize]
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