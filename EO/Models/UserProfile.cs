namespace EO.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string UserId { get; set; } 
        public ApplicationUser User { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }

        public CompanyDetails? CompanyDetails { get; set; }
        public SocialLinks? SocialLinks { get; set; }
        public Spouse? Spouse { get; set; }

        public ICollection<Child>? Children { get; set; }
    }
    
    public class CompanyDetails
    {
        public int Id { get; set; }
        public int UserProfileId { get; set; }

        public string CompanyName { get; set; }
        public string Designation { get; set; }
    }

    public class SocialLinks
    {
        public int Id { get; set; }
        public int UserProfileId { get; set; } 
        public int SpouseId { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? LinkedIn { get; set; }
        public string X { get; set; }
        public string? Website { get; set; }
    }

    public class Spouse
    {
        public int Id { get; set; }
        public int UserProfileId { get; set; }
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public SocialLinks? SocialLinks { get; set; }
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
}
