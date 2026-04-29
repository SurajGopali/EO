using EO.Models;

namespace EO.Services.Event
{
    public interface IEventService
    {
        Task<List<EventDto>> GetEventsAsync();
        Task<EventDetailsDto> GetEventDetailsAsync(int id);

        Task<CreateEventDetailsDto> GetEventDetailsForEditAsync(int eventId);
        Task UpsertEventDetailsAsync(int eventId, CreateEventDetailsDto dto);

        Task<UpdateEventDetailFullDto> GetFullEventForEditAsync(int id);

        Task UpdateFullEventAsync(int id, UpdateEventDetailFullDto dto);

        Task<EventUpdateDto> GetEventForEditAsync(int id);

        Task UpsertEventAsync(int id, EventUpdateDto dto);
        Task DeleteEventAsync(int id);
        Task<List<EventType>> GetEventTypesAsync();
        Task<List<EventDto>> FilterEventsAsync(int? eventTypeId, int? year, string? range);
        Task<List<EventDto>> GetUpcomingEventsAsync();
        Task<List<EventDto>> GetTodayEventsAsync();

    }
}