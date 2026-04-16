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
}