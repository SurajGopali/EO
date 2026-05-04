using EO.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace EO.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string? MembershipType { get; set; }
        public string? ProfileImage { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string? EoRole { get; set; }
        public DateTime JoinedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public CompanyDetails? CompanyDetails { get; set; }
        public UserProfile? UserProfiles { get; set; }
        public Gender? Gender { get; set; }
        public string FullName
        {
            get
            {
                return $"{FirstName} {MiddleName} {LastName}"
                    .Replace("  ", " ")
                    .Trim();
            }
        }
    }

    public class RegisterRequest
    {
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? MobileNumber { get; set; }
        public string Password { get; set; }
        public DateTime JoinedDate { get; set; }
    }
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string? MembershipType { get; set; }
        public string? ProfileImage { get; set; }
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    public class TokenRequest
    {
        public string RefreshToken { get; set; }
    }


    public class ProfileUpdateDto
    {
        public string UserName { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }

        public string? ProfileImage { get; set; }
    }

}
