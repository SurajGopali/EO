using EO.Models;

namespace EO.Services.EventGuests
{
    public interface IEventGuestService
    {
        Task AddGuestToEventAsync(int eventId, string userId);
        Task RemoveGuestFromEventAsync(int eventId, string userId);
        Task<List<EventGuestDto>> GetGuestsByEventIdAsync(int eventId);
    }
}