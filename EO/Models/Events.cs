using System;
using System.Collections.Generic;

namespace EO.Models
{
    // =========================
    // MAIN EVENT ENTITY
    // =========================
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
        public string? Image { get; set; }

        public EventDetail? EventDetail { get; set; }
        public List<EventSchedule>? Schedules { get; set; }
        public List<EventGuest>? Guests { get; set; }
    }

    // =========================
    // EVENT TYPE
    // =========================
    public class EventType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    // =========================
    // EVENT DETAIL (1-1)
    // =========================
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

    // =========================
    // EVENT SCHEDULE
    // =========================
    public class EventSchedule
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public Events Event { get; set; }

        public string? StartTime { get; set; }
        public string? EndTime { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
    }

    // =========================
    // EVENT GUEST (IDENTITY USER)
    // =========================
    public class EventGuest
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public Events? Event { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }

    // =========================
    // EVENT LIST DTO
    // =========================
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

    // =========================
    // CREATE EVENT
    // =========================
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

    // =========================
    // EVENT DETAILS RESPONSE
    // =========================
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

        public List<ScheduleDto> Schedule { get; set; } = new();
        public List<EventGuestDto> Guests { get; set; } = new();
    }

    // =========================
    // SCHEDULE DTO
    // =========================
    public class ScheduleDto
    {
        public int Id { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    // =========================
    // EVENT GUEST DTO (USER-BASED)
    // =========================
    public class EventGuestDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? CompanyName { get; set; }
        public string? Designation { get; set; }
        public string? Avatar { get; set; }
    }

    // =========================
    // EVENT DETAIL CREATE/UPDATE
    // =========================
    public class CreateEventDetailsDto
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public string Venue { get; set; }
        public string CoverImage { get; set; }

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public bool IsRegistered { get; set; }

        public List<ScheduleDto> Schedule { get; set; } = new();
    }

    // =========================
    // FULL UPDATE DTO
    // =========================
    public class UpdateEventDetailFullDto
    {
        public int EventId { get; set; }

        public CreateEventDetailsDto Event { get; set; }

        public List<ScheduleDto> Schedule { get; set; } = new();

        public List<string> GuestIds { get; set; } = new();
    }

    // =========================
    // SIMPLE EVENT UPDATE
    // =========================
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

    // =========================
    // ADD USER TO EVENT
    // =========================
    public class AddGuestToEventDto
    {
        public int EventId { get; set; }
        public string UserId { get; set; }
    }
}