using EO.Models;

namespace EO.Services.EventGuest
{
    public interface IEventGuestService
    {
        Task AddGuestToEventAsync(int eventId, int guestId);
        Task RemoveGuestFromEventAsync(int eventId, int guestId);
        Task<List<Guests>> GetGuestsByEventIdAsync(int eventId);
    }
}