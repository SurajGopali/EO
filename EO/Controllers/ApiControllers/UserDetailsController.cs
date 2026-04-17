using EO.Models;
using EO.WebContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IActionResult> UpsertProfile(CreateProfileRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return NotFound("User not found");

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

            profile.DateOfBirth = request.PersonalDetails?.DateOfBirth;
            profile.AnniversaryDate = request.PersonalDetails?.AnniversaryDate;
            profile.Address = request.PersonalDetails?.Address;
            profile.IsMarried = request.PersonalDetails?.IsMarried ?? false;

            await _context.SaveChangesAsync();

            if (request.CompanyDetails != null)
            {
                var company = await _context.CompanyDetails
                    .FirstOrDefaultAsync(x => x.UserId == request.UserId);

                if (company == null)
                {
                    company = new CompanyDetails { UserId = request.UserId };
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
                    social = new UserSocialLinks { UserId = request.UserId };
                    await _context.UserSocialLinks.AddAsync(social);
                }

                social.Facebook = request.SocialLinks.Facebook;
                social.Instagram = request.SocialLinks.Instagram;
                social.LinkedIn = request.SocialLinks.LinkedIn;
                social.X = request.SocialLinks.X;
                social.Website = request.SocialLinks.Website;
            }

         
            var spouse = await _context.Spouses
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

                spouse = await _context.Spouses
                    .FirstOrDefaultAsync(x => x.UserProfileId == profile.Id);

                if (spouse == null)
                {
                    spouse = new Spouse
                    {
                        UserProfileId = profile.Id,
                        Name = request.Spouse.Name,  
                        Phone = request.Spouse.Phone,
                        Email = request.Spouse.Email,
                        DateOfBirth = request.Spouse.DateOfBirth
                    };

                    await _context.Spouses.AddAsync(spouse);
                }
                else
                {
                    spouse.Name = request.Spouse.Name;
                    spouse.Phone = request.Spouse.Phone;
                    spouse.Email = request.Spouse.Email;
                    spouse.DateOfBirth = request.Spouse.DateOfBirth;
                }
                await _context.Spouses.AddAsync(spouse);
                await _context.SaveChangesAsync();

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
            }
            else if (spouse != null)
            {
                _context.Spouses.Remove(spouse);
            }

           
            if (request.Children != null)
            {
                foreach (var childDto in request.Children)
                {
                    if (childDto.Id.HasValue)
                    {
                        var existingChild = await _context.Children
                            .FirstOrDefaultAsync(x =>
                                x.Id == childDto.Id.Value &&
                                x.UserProfileId == profile.Id);

                        if (existingChild != null)
                        {
                            existingChild.Name = childDto.Name;
                            existingChild.School = childDto.School;
                            existingChild.Grade = childDto.Grade;
                            existingChild.DateOfBirth = childDto.DateOfBirth;
                        }
                    }
                    else
                    {
                        var newChild = new Child
                        {
                            UserProfileId = profile.Id,
                            Name = childDto.Name,
                            School = childDto.School,
                            Grade = childDto.Grade,
                            DateOfBirth = childDto.DateOfBirth
                        };

                        await _context.Children.AddAsync(newChild);
                    }
                }
            }

           
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new
            {
                success = true,
                message = "Profile Updated successful",
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
    public async Task<IActionResult> GetMembers()
    {
        var users = await _userManager.Users.ToListAsync();

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

                ProfileImage = u.ProfileImage,

                CompanyName = company?.CompanyName,
                CompanyRole = company?.Designation,
                Designation = company?.Designation,

                Birthday = profile?.DateOfBirth,
                Anniversary = profile?.AnniversaryDate
            };
        }).ToList();

        return Ok(new
        {
            success = true,
            message = "Members fetched successfully",
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
    public IActionResult GetMemberDetails()
    {
        var data = new
        {
            id = "mem_001",
            name = "Neal Caffery",
            eoRole = "Executive Member",
            companyName = "Himalayan Tech Solutions",
            companyRole = "Board Member",
            designation = "CEO",
            profileImage =
                "https://thumbs.dreamstime.com/b/happy-middle-aged-business-man-ceo-executive-sitting-office-portrait-desk-smiling-mature-confident-professional-manager-311125360.jpg",
            email = "neal@example.com",
            phone = "+977-9800000001",
            location = "Kathmandu, Nepal",
            bio = "Experienced board member with a strong background in leadership, strategy, and corporate governance.",

            joinedDate = "2026-03-10",
            dob = "1978-04-12",

            socialLinks = new
            {
                linkedin = "https://www.linkedin.com/in/neal-caffery",
                twitter = "https://twitter.com/nealcaffery",
                facebook = "https://www.facebook.com/neal.caffery",
                instagram = "https://www.instagram.com/nealcaffery",
                website = "https://nealcaffery.com"
            },

            family = new
            {
                spouse = new
                {
                    id = "mem_002",
                    name = "Diana Barrigan",
                    companyRole = "Legal Consultant",
                    profileImage = "https://randomuser.me/api/portraits/women/65.jpg",
                    email = "diana@example.com",
                    phone = "+977-9800000002",
                    location = "Kathmandu, Nepal",
                    bio = "Corporate legal consultant specializing in mergers and compliance frameworks.",

                    marriageAnniversary = "2000-03-10",
                    dob = "1980-09-21",

                    socialLinks = new
                    {
                        linkedin = "https://www.linkedin.com/in/Diana-caffery",
                        twitter = "https://twitter.com/Diana",
                        facebook = "https://www.facebook.com/Diana.caffery",
                        instagram = "https://www.instagram.com/Diana",
                        website = "https://Dianacaffery.com"
                    },

                    professional = new
                    {
                        currentCompany = "Independent Consultant",
                        position = "Legal Consultant",
                        experienceYears = 14,
                        previousRoles = new[]
                        {
                        "Senior Legal Advisor - Law Firm XYZ"
                    },
                        expertise = new[]
                        {
                        "Corporate Law",
                        "Mergers & Acquisitions",
                        "Compliance"
                    }
                    }
                },

                children = new[]
                {
                new
                {
                    name = "Kate Caffery",
                    dob = "2008-06-14",
                    education = "Grade 12 Student",
                    interests = new[] { "Debate", "Music", "Volleyball" },
                    profileImage = "https://randomuser.me/api/portraits/women/12.jpg"
                },
                new
                {
                    name = "June Caffery",
                    dob = "2011-11-03",
                    education = "Grade 9 Student",
                    interests = new[] { "Painting", "Chess", "Reading" },
                    profileImage = "https://randomuser.me/api/portraits/men/15.jpg"
                }
            }
            }
        };

        return Ok(new
        {
            success = true,
            message = "Member Details",
            data
        });
    }
}