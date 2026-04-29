using EO.Models;

namespace EO.Services.Profile
{
    public interface IProfileService
    {
        Task<ServiceResponse> UpsertProfileAsync(UpdateProfileRequest request, string userId);
        Task<UpdateProfileRequest> GetProfileAsync(string userId);
        Task<ApplicationUser?> GetAsync(string userId);

        Task<bool> UpdateAsync(string userId, ProfileUpdateDto dto);

    }
}
