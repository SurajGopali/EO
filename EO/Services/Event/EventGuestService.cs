using EO.Models;
using EO.WebContext;
using Microsoft.EntityFrameworkCore;

namespace EO.Services.EventGuests
{
    public class EventGuestService : IEventGuestService
    {
        private readonly AppDbContext _context;

        public EventGuestService(AppDbContext context)
        {
            _context = context;
        }

        // =====================
        // ADD USER TO EVENT
        // =====================
        public async Task AddGuestToEventAsync(int eventId, string userId)
        {
            var exists = await _context.EventGuests
                .AnyAsync(x => x.EventId == eventId && x.UserId == userId);

            if (exists) return;

            var mapping = new EO.Models.EventGuest
            {
                EventId = eventId,
                UserId = userId
            };

            _context.EventGuests.Add(mapping);
            await _context.SaveChangesAsync();
        }

        // =====================
        // REMOVE USER FROM EVENT
        // =====================
        public async Task RemoveGuestFromEventAsync(int eventId, string userId)
        {
            var record = await _context.EventGuests
                .FirstOrDefaultAsync(x => x.EventId == eventId && x.UserId == userId);

            if (record == null) return;

            _context.EventGuests.Remove(record);
            await _context.SaveChangesAsync();
        }

        // =====================
        // GET USERS OF EVENT
        // =====================
        public async Task<List<EventGuestDto>> GetGuestsByEventIdAsync(int eventId)
        {
            return await _context.EventGuests
                .Where(x => x.EventId == eventId)
                .Include(x => x.User)
                    .ThenInclude(u => u.CompanyDetails)
                .Select(x => new EventGuestDto
                {
                    Id = x.User.Id,
                    Name = x.User.FullName,
                    Avatar = x.User.ProfileImage,

                    Designation = x.User.CompanyDetails != null
                        ? x.User.CompanyDetails.Designation
                        : "",

                    CompanyName = x.User.CompanyDetails != null
                        ? x.User.CompanyDetails.CompanyName
                        : ""
                })
                .ToListAsync();
        }
    }
}