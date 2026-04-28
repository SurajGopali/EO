using EO.Models;

namespace EO.Services.Guest
{
    public interface IGuestService
    {
        Task<List<Guests>> GetAllAsync();
        Task<Guests> GetByIdAsync(int id);   
        Task<Guests> CreateAsync(CreateGuestDto dto);
        Task<bool> UpdateAsync(int id, CreateGuestDto dto);
        Task<bool> DeleteAsync(int id);
    }
}