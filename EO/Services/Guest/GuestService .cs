using EO.Models;
using EO.WebContext;
using Microsoft.EntityFrameworkCore;

namespace EO.Services.Guest
{
    public class GuestService : IGuestService
    {
        private readonly AppDbContext _context;

        public GuestService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Guests>> GetAllAsync()
        {
            return await _context.Guests.ToListAsync();
        }

        public async Task<Guests> CreateAsync(CreateGuestDto dto)
        {
            var guest = new Guests
            {
                Name = dto.Name,
                Designation = dto.Designation,
                Avatar = dto.Avatar
            };

            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();

            return guest;
        }

        public async Task<Guests> GetByIdAsync(int id)
        {
            return await _context.Guests
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> UpdateAsync(int id, CreateGuestDto dto)
        {
            var guest = await _context.Guests.FindAsync(id);

            if (guest == null)
                return false;

            guest.Name = dto.Name;
            guest.Designation = dto.Designation;
            guest.Avatar = dto.Avatar;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var guest = await _context.Guests.FindAsync(id);

            if (guest == null)
                return false;

            _context.Guests.Remove(guest);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}