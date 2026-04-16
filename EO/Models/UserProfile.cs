namespace EO.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string UserId { get; set; }   
        public DateTime? DateOfBirth { get; set; }
        public DateTime? AnniversaryDate { get; set; }
        public string? Address { get; set; }
        public bool IsMarried { get; set; }
        public ApplicationUser User { get; set; }
    }

    public class CompanyDetails
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string CompanyName { get; set; }
        public string Designation { get; set; }

        public ApplicationUser User { get; set; }
    }

    public class UserSocialLinks
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? LinkedIn { get; set; }
        public string? X { get; set; }
        public string? Website { get; set; }
        public ApplicationUser User { get; set; }
    }

    public class SpouseSocialLinks
    {
        public int Id { get; set; }
        public int SpouseId { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? LinkedIn { get; set; }
        public string? X { get; set; }
        public string? Website { get; set; }
        public Spouse Spouse { get; set; }
    }

    public class Spouse
    {
        public int Id { get; set; }
        public int UserProfileId { get; set; }
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public SpouseSocialLinks? SocialLinks { get; set; }
    }

    public class Child
    {
        public int Id { get; set; }
        public int UserProfileId { get; set; }
        public string Name { get; set; }
        public string? School { get; set; }
        public string? Grade { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

    public class ChildDto
    {
        public int? Id { get; set; }  

        public string Name { get; set; }
        public string School { get; set; }
        public string Grade { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

    public class CreateProfileRequest
    {
        public string UserId { get; set; }

        public PersonalDetailsDto PersonalDetails { get; set; }
        public CompanyDetailsDto CompanyDetails { get; set; }
        public SocialLinksDto SocialLinks { get; set; }
        public SpouseDto Spouse { get; set; }
        public List<ChildDto> Children { get; set; }
    }
    public class UpdateProfileRequest
    {
        public string UserId { get; set; }

        public PersonalDetailsDto? PersonalDetails { get; set; }
        public CompanyDetailsDto? CompanyDetails { get; set; }
        public SocialLinksDto? SocialLinks { get; set; }
        public SpouseDto? Spouse { get; set; }
        public List<ChildDto>? Children { get; set; }
    }
    public class PersonalDetailsDto
    {
        public DateTime? DateOfBirth { get; set; }
        public DateTime? AnniversaryDate { get; set; }
        public string Address { get; set; }
        public bool IsMarried { get; set; }
    }

    public class CompanyDetailsDto
    {
        public string CompanyName { get; set; }
        public string Designation { get; set; }
    }

    public class SocialLinksDto
    {
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string LinkedIn { get; set; }
        public string X { get; set; }
        public string Website { get; set; }
    }
    public class SpouseDto
    {
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public SocialLinksDto SocialLinks { get; set; }
    }

}
