using EO.Models;
using EO.WebContext;
using Microsoft.EntityFrameworkCore;

namespace EO.Services.EventGuest
{
    public class EventGuestService : IEventGuestService
    {
        private readonly AppDbContext _context;

        public EventGuestService(AppDbContext context)
        {
            _context = context;
        }

        // =====================
        // ADD GUEST TO EVENT
        // =====================
        public async Task AddGuestToEventAsync(int eventId, int guestId)
        {
            var exists = await _context.EventGuests
                .AnyAsync(x => x.EventId == eventId && x.GuestId == guestId);

            if (exists)
                return;

            var mapping = new Models.EventGuest
            {
                EventId = eventId,
                GuestId = guestId
            };

            _context.EventGuests.Add(mapping);
            await _context.SaveChangesAsync();
        }

        // =====================
        // REMOVE GUEST FROM EVENT
        // =====================
        public async Task RemoveGuestFromEventAsync(int eventId, int guestId)
        {
            var record = await _context.EventGuests
                .FirstOrDefaultAsync(x => x.EventId == eventId && x.GuestId == guestId);

            if (record == null)
                return;

            _context.EventGuests.Remove(record);
            await _context.SaveChangesAsync();
        }

        // =====================
        // GET GUESTS OF EVENT
        // =====================
        public async Task<List<Guests>> GetGuestsByEventIdAsync(int eventId)
        {
            return await _context.EventGuests
                .Where(x => x.EventId == eventId)
                .Include(x => x.Guest)
                .Select(x => x.Guest)
                .ToListAsync();
        }
    }
}