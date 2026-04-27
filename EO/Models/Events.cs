namespace EO.Models
{
    public class Events
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? Location { get; set; }
        public DateTime Date { get; set; }
        public int EventTypeId { get; set; }  
        public EventType EventType { get; set; }
        public bool? IsRegistered { get; set; }
        public string? Image {  get; set; }
        public EventDetail EventDetail { get; set; }
        public List<EventSchedule> Schedules { get; set; }
        public List<EventGuest> Guests { get; set; }

    }

    public class EventType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class EventDetail
    {
        public int Id { get; set; }

        public int EventId { get; set; }   
        public Events Event { get; set; }

        public string Title { get; set; }         
        public string Description { get; set; }
        public string Venue { get; set; }
        public string CoverImage { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }

    public class EventSchedule
    {
        public int Id { get; set; }

        public int EventId { get; set; }   
        public Events Event { get; set; }

        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class Guests
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Designation { get; set; }
        public string Avatar { get; set; }

        public List<EventGuest> EventGuests { get; set; }
    }
    public class EventGuest
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public Events Event { get; set; }

        public int GuestId { get; set; }
        public Guests Guest { get; set; }
    }
    public class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? Location { get; set; }
        public DateTime Date { get; set; }
        public int TypeId { get; set; }
        public string? Type { get; set; }
        public bool? IsRegistered { get; set; }
        public string? Image { get; set; }
        public int RemainingDays { get; set; }
    }
    public class EventCreateDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? Location { get; set; }
        public DateTime Date { get; set; }
        public int EventTypeId { get; set; }
        public bool? IsRegistered { get; set; }
        public string? Image { get; set; }
    }

    public class EventDetailsDto
    {
        public string Id { get; set; }   
        public string Title { get; set; }
        public string Description { get; set; }

        public string Venue { get; set; }
        public string CoverImage { get; set; }

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public bool IsRegistered { get; set; }

        public List<ScheduleDto> Schedule { get; set; }
        public List<GuestDto> Guests { get; set; }
    }

    public class ScheduleDto
    {
        public int Id { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class GuestDto
    {
        public string Id { get; set; }   
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Avatar { get; set; }
    }

    public class CreateEventDetailsDto
    {
        public string Title { get; set; }       
        public string Description { get; set; }

        public string Venue { get; set; }
        public string CoverImage { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public bool IsRegistered { get; set; }
        public List<ScheduleDto> Schedule { get; set; }
        public List<GuestDto> Guests { get; set; }

    }

    public class UpdateEventDetailFullDto
    {
        public int EventId { get; set; }

        public CreateEventDetailsDto Event { get; set; }

        public List<ScheduleDto> Schedule { get; set; } = new();
        public List<GuestDto> Guests { get; set; } = new();
        public List<int> GuestIds { get; set; } = new();
    }


    public class EventUpdateDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? Location { get; set; }
        public DateTime Date { get; set; }
        public int TypeId { get; set; }
        public string? Type { get; set; }
        public bool? IsRegistered { get; set; }
        public string? Image { get; set; }
    }

    public class AddGuestToEventDto
    {
        public int EventId { get; set; }
        public int GuestId { get; set; }
    }

    public class UpdateGuestDto
    {
        public int GuestId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Avatar { get; set; }
    }
    public class CreateGuestDto
    {
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Avatar { get; set; }
    }
}
