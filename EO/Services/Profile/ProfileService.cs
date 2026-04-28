using EO.Models;
using EO.Services.Common;
using EO.Services.Profile;
using EO.WebContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class ProfileService : IProfileService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICommonService _commonService;

    public ProfileService(AppDbContext context, UserManager<ApplicationUser> userManager,ICommonService commonService)
    {
        _context = context;
        _userManager = userManager;
        _commonService = commonService;
    }

    public async Task<ServiceResponse> UpsertProfileAsync(UpdateProfileRequest request, string userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ServiceResponse { Success = false, Message = "User not found" };

            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (profile == null)
            {
                profile = new UserProfile { UserId = userId };
                _context.UserProfiles.Add(profile);
            }

            if (request.PersonalDetails != null)
            {
                profile.DateOfBirth = request.PersonalDetails.DateOfBirth;
                profile.AnniversaryDate = request.PersonalDetails.IsMarried
                    ? request.PersonalDetails.AnniversaryDate
                    : null;

                profile.Address = request.PersonalDetails.Address;
                profile.IsMarried = request.PersonalDetails.IsMarried;
                profile.Bio = request.PersonalDetails.Bio;
            }

            bool isMarried = profile.IsMarried;

            var company = await _context.CompanyDetails
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (company == null)
            {
                company = new CompanyDetails { UserId = userId };
                _context.CompanyDetails.Add(company);
            }

            if (request.CompanyDetails != null)
            {
                company.CompanyName = request.CompanyDetails.CompanyName;
                company.Designation = request.CompanyDetails.Designation;
                company.CompanyRole = request.CompanyDetails.Role;
            }

            var social = await _context.UserSocialLinks
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (social == null)
            {
                social = new UserSocialLinks { UserId = userId };
                _context.UserSocialLinks.Add(social);
            }

            if (request.SocialLinks != null)
            {
                social.Facebook = request.SocialLinks.Facebook;
                social.Instagram = request.SocialLinks.Instagram;
                social.LinkedIn = request.SocialLinks.LinkedIn;
                social.X = request.SocialLinks.X;
                social.Website = request.SocialLinks.Website;
            }

            await _context.SaveChangesAsync();

            Spouse spouse = null;

            if (isMarried && request.Spouse != null)
            {
                spouse = await _context.Spouses
                    .FirstOrDefaultAsync(x => x.UserProfileId == profile.Id);

                if (spouse == null)
                {
                    spouse = new Spouse { UserProfileId = profile.Id };
                    _context.Spouses.Add(spouse);
                }

                spouse.Name = request.Spouse.Name;
                spouse.Phone = request.Spouse.Phone;
                spouse.Email = request.Spouse.Email;
                spouse.DateOfBirth = request.Spouse.DateOfBirth;
                spouse.ProfileImage = request.Spouse.ProfileImage;

                var spouseSocial = await _context.SpouseSocialLinks
                    .FirstOrDefaultAsync(x => x.SpouseId == spouse.Id);

                if (spouseSocial == null)
                {
                    spouseSocial = new SpouseSocialLinks { SpouseId = spouse.Id };
                    _context.SpouseSocialLinks.Add(spouseSocial);
                }

                spouseSocial.Facebook = request.Spouse.SocialLinks?.Facebook;
                spouseSocial.Instagram = request.Spouse.SocialLinks?.Instagram;
                spouseSocial.LinkedIn = request.Spouse.SocialLinks?.LinkedIn;
                spouseSocial.X = request.Spouse.SocialLinks?.X;
                spouseSocial.Website = request.Spouse.SocialLinks?.Website;

                var prof = await _context.SpouseProfessionalDetails
                    .FirstOrDefaultAsync(x => x.SpouseId == spouse.Id);

                if (prof == null)
                {
                    prof = new SpouseProfessionalDetails { SpouseId = spouse.Id };
                    _context.SpouseProfessionalDetails.Add(prof);
                }

                prof.CurrentCompany = request.Spouse.Professional?.CurrentCompany;
                prof.Position = request.Spouse.Professional?.Position;
                prof.ExperienceYears = request.Spouse.Professional?.ExperienceYears;

                prof.PreviousRoles = request.Spouse.Professional?.PreviousRoles != null
                    ? string.Join(",", request.Spouse.Professional.PreviousRoles)
                    : null;

                prof.Expertise = request.Spouse.Professional?.Expertise != null
                    ? string.Join(",", request.Spouse.Professional.Expertise)
                    : null;
            }

            var existingChildren = await _context.Children
                .Where(x => x.UserProfileId == profile.Id)
                .ToListAsync();

            if (isMarried && request.Children != null)
            {
                var requestIds = request.Children
                    .Where(c => c.Id.HasValue)
                    .Select(c => c.Id.Value)
                    .ToList();

                _context.Children.RemoveRange(
                    existingChildren.Where(x => !requestIds.Contains(x.Id))
                );

                foreach (var child in request.Children)
                {
                    if (child.Id.HasValue)
                    {
                        var existing = existingChildren.FirstOrDefault(x => x.Id == child.Id);
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
                        _context.Children.Add(new Child
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
            else
            {
                _context.Children.RemoveRange(existingChildren);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ServiceResponse
            {
                Success = true,
                Message = "Profile updated successfully"
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new ServiceResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<UpdateProfileRequest> GetProfileAsync(string userId)
    {
        var profile = await _context.UserProfiles
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.UserId == userId);

        var company = await _context.CompanyDetails
            .FirstOrDefaultAsync(x => x.UserId == userId);

        var social = await _context.UserSocialLinks
            .FirstOrDefaultAsync(x => x.UserId == userId);

        // ---------------- SAFE PROFILE CHECK ----------------
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

        return new UpdateProfileRequest
        {
            UserId = userId,

            PersonalDetails = profile == null ? null : new PersonalDetailsDto
            {
                DateOfBirth = profile.DateOfBirth,
                AnniversaryDate = profile.AnniversaryDate,
                Address = profile.Address,
                Bio = profile.Bio,
                IsMarried = profile.IsMarried
            },

            CompanyDetails = company == null ? null : new CompanyDetailsDto
            {
                CompanyName = company.CompanyName,
                Designation = company.Designation,
                Role = company.CompanyRole
            },

            SocialLinks = social == null ? null : new SocialLinksDto
            {
                Facebook = social.Facebook,
                Instagram = social.Instagram,
                LinkedIn = social.LinkedIn,
                X = social.X,
                Website = social.Website
            },

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

                    PreviousRoles = string.IsNullOrWhiteSpace(spouse.ProfessionalDetails.PreviousRoles)
                        ? new List<string>()
                        : spouse.ProfessionalDetails.PreviousRoles
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .ToList(),

                    Expertise = string.IsNullOrWhiteSpace(spouse.ProfessionalDetails.Expertise)
                        ? new List<string>()
                        : spouse.ProfessionalDetails.Expertise
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .ToList()
                }
            },

            Children = children.Select(c => new ChildDto
            {
                Id = c.Id,
                Name = c.Name,
                School = c.School,
                Grade = c.Grade,
                DateOfBirth = c.DateOfBirth,
                ProfileImage = c.ProfileImage
            }).ToList()
        };
    }

}