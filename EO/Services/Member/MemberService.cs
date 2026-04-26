using EO.Models;
using EO.Services;
using EO.WebContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class MemberService : IMemberService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public MemberService(
        UserManager<ApplicationUser> userManager,
        AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<List<MemberDto>> GetMembersAsync(bool isNew)
    {
        var users = await _userManager.Users.ToListAsync();

        if (isNew)
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

        return users.Select(u =>
        {
            var profile = profiles.FirstOrDefault(p => p.UserId == u.Id);
            var company = companies.FirstOrDefault(c => c.UserId == u.Id);

            return new MemberDto
            {
                Id = u.Id,
                Name = $"{u.FirstName} {u.MiddleName} {u.LastName}".Trim(),
                EoRole = u.EoRole,
                JoinedDate = u.JoinedDate,
                ProfileImage = u.ProfileImage,

                CompanyName = company?.CompanyName,
                CompanyRole = company?.CompanyRole,
                Designation = company?.Designation,

                Birthday = profile?.DateOfBirth,
                Anniversary = profile?.AnniversaryDate,

                IsActive = u.IsActive   
            };
        }).ToList();
    }

    public async Task<MemberDetailDto?> GetMemberDetailAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
            return null;

        var profile = await _context.UserProfiles
            .FirstOrDefaultAsync(x => x.UserId == id);

        var company = await _context.CompanyDetails
            .FirstOrDefaultAsync(x => x.UserId == id);

        var social = await _context.UserSocialLinks
            .FirstOrDefaultAsync(x => x.UserId == id);

        Spouse? spouse = null;
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

        return new MemberDetailDto
        {
            Id = user.Id,
            Name = user.UserName ?? "",
            EoRole = user.EoRole ?? "",
            Email = user.Email ?? "",
            Phone = user.PhoneNumber ?? "",
            ProfileImage = user.ProfileImage ?? "",
            JoinedDate = user.JoinedDate,

            CompanyName = company?.CompanyName ?? "",
            CompanyRole = company?.CompanyRole ?? "",
            Designation = company?.Designation ?? "",

            Location = profile?.Address ?? "",
            Bio = profile?.Bio ?? "",
            Dob = profile?.DateOfBirth,

            SocialLinks = social == null ? null : new SocialDto
            {
                Linkedin = social.LinkedIn ?? "",
                Twitter = social.X ?? "",
                Facebook = social.Facebook ?? "",
                Instagram = social.Instagram ?? "",
                Website = social.Website ?? ""
            },

            Spouse = spouse == null ? null : new SpouseDto
            {
                Name = spouse.Name ?? "",
                Email = spouse.Email ?? "",
                Phone = spouse.Phone ?? "",
                ProfileImage = spouse.ProfileImage ?? "",

                SocialLinks = spouse.SocialLinks == null ? null : new SocialLinksDto
                {
                    Facebook = spouse.SocialLinks.Facebook ?? "",
                    Instagram = spouse.SocialLinks.Instagram ?? "",
                    LinkedIn = spouse.SocialLinks.LinkedIn ?? "",
                    X = spouse.SocialLinks.X ?? "",
                    Website = spouse.SocialLinks.Website ?? ""
                },

                Professional = spouse.ProfessionalDetails == null ? null : new SpouseProfessionalDto
                {
                    CurrentCompany = spouse.ProfessionalDetails.CurrentCompany ?? "",
                    Position = spouse.ProfessionalDetails.Position ?? "",
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

            Children = children?.Select(c => new ChildDto
            {
                Name = c.Name ?? "",
                DateOfBirth = c.DateOfBirth,
                Grade = c.Grade ?? "",
                School = c.School ?? "",
                ProfileImage = c.ProfileImage ?? ""
            }).ToList() ?? new List<ChildDto>()
        };
    }
}