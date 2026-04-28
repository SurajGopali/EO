using Azure;
using EO.Models;
using EO.Services.Event;
using EO.WebContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EO.Controllers.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEventService _eventService; 

        public EventsController(AppDbContext context, IEventService eventService)
        {
            _context = context;
            _eventService = eventService;
        }


        [Authorize(AuthenticationSchemes = "Jwt")]
        [HttpGet("GetAllEvents")]
        public async Task<IActionResult> GetAllEvents()
        {
            var data = await _eventService.GetEventsAsync();
            return Ok(new
            {
                success = true,
                message = "Event types fetched successfully",
                AllEvents = data
            });
        }

        [Authorize(AuthenticationSchemes = "Jwt")]
        [HttpGet("event-types")]
        public async Task<IActionResult> GetEventTypes()
        {
            var types = await _eventService.GetEventTypesAsync();

            return Ok(new
            {
                success = true,
                message = "Event types fetched successfully",
                eventTypes = types
            });
        }

        [Authorize(AuthenticationSchemes = "Jwt")]
        [HttpGet("filter-events")]
        public async Task<IActionResult> FilterEvents(
         [FromQuery] int? eventTypeId,
         [FromQuery] int? year,
         [FromQuery] string? range)
        {
            var result = await _eventService.FilterEventsAsync(eventTypeId, year, range);

            return Ok(new 
            {
                Success = true,
                Message = "Filtered events fetched successfully",
                Data = result
            });
        }


        [Authorize(AuthenticationSchemes = "Jwt")]
        [HttpGet("TodayEvents")]
        public async Task<IActionResult> GetTodayEvents()
        {
            var today = DateTime.UtcNow.Date;

            var events = await _context.Events
                .Where(x => x.Date.Date == today)
                .Include(x => x.EventType)
                .OrderBy(x => x.StartTime)
                .ToListAsync();

            var result = events.Select(x => new EventDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Location = x.Location,
                Date = x.Date,
                TypeId = x.EventTypeId,
                Type = x.EventType?.Name,
                IsRegistered = x.IsRegistered,
                Image = x.Image,
                RemainingDays = 0
            });

            return Ok(new
            {
                success = true,
                message = "Today Events fetched successfully",
                todayEvents = result
            });
        }

        [Authorize(AuthenticationSchemes = "Jwt")]
        [HttpGet("UpcomingEvents")]
        public async Task<IActionResult> GetUpcomingEvents()
        {
            var today = DateTime.Today;

            var events = await _context.Events
                .Where(x => x.Date.Date > today)
                .Include(x => x.EventType)
                .OrderBy(x => x.Date)
                .ThenBy(x => x.StartTime)
                .ToListAsync();

            var now = DateTime.Now;

            var result = events.Select(x => new EventDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Location = x.Location,
                Date = x.Date,
                TypeId = x.EventTypeId,
                Type = x.EventType?.Name,
                IsRegistered = x.IsRegistered,
                Image = x.Image,
                RemainingDays = (int)Math.Ceiling((x.Date + (x.StartTime ?? TimeSpan.Zero) - now).TotalDays)
            });

            return Ok(new
            {
                success = true,
                message = "Upcoming Events fetched successfully",
                upcomingEvents = result
            });
        }

        [HttpPost("AddEvent")]
        public async Task<IActionResult> AddEvent([FromBody] EventCreateDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newEvent = new Events
            {
                Title = model.Title,
                Description = model.Description,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Location = model.Location,
                Date = model.Date,
                EventTypeId = model.EventTypeId,
                IsRegistered = model.IsRegistered,
                Image = model.Image
            };

            await _context.Events.AddAsync(newEvent);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Event created successfully",
                AddedEvent = model
            });
        }

        [HttpPut("UpdateEvent/{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventCreateDto model)
        {
            var existingEvent = await _context.Events.FindAsync(id);

            if (existingEvent == null)
                return NotFound("Event Not Found");

            existingEvent.Title = model.Title;
            existingEvent.Description = model.Description;
            existingEvent.StartTime = model.StartTime;
            existingEvent.EndTime = model.EndTime;
            existingEvent.Location = model.Location;
            existingEvent.Date = model.Date;
            existingEvent.EventTypeId = model.EventTypeId;
            existingEvent.IsRegistered = model.IsRegistered;
            existingEvent.Image = model.Image;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                messsage = "Event updated successfully",
                UpdatedEvent = existingEvent
            });

        }

        [HttpDelete("DeleteEvent/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var eve = await _context.Events.FindAsync(id);

            if (eve == null)
                return NotFound("Event not found");

            _context.Events.Remove(eve);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Event Deleted Successfully"
            });
        }

        [Authorize(AuthenticationSchemes = "Jwt")]
        [HttpGet("event-details")]
        public async Task<IActionResult> GetEventDetails([FromQuery] int id)
        {
            if (id <= 0)
                return BadRequest("Invalid event id");

            var eventData = await _context.Events
                .Include(e => e.EventDetail)
                .Include(e => e.Schedules)
                .Include(e => e.Guests)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventData == null)
                return NotFound();

            var response = new EventDetailsDto
            {
                Id = eventData.Id.ToString(),
                Title = eventData.EventDetail?.Title,
                Description = eventData.EventDetail?.Description,
                Venue = eventData.EventDetail?.Venue,
                CoverImage = eventData.EventDetail?.CoverImage,
                StartDateTime = eventData.EventDetail?.StartDateTime ?? default,
                EndDateTime = eventData.EventDetail?.EndDateTime ?? default,
                IsRegistered = eventData.IsRegistered ?? false,

                Schedule = eventData.Schedules
                    .OrderBy(s => s.Id)
                    .Select(s => new ScheduleDto
                    {
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        Title = s.Title,
                        Description = s.Description
                    }).ToList(),

                Guests = eventData.Guests
    .Where(x => x.User != null)
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
    .ToList()
            };

            return Ok(new
            {
                success = true,
                message = "Event details fetched successfully",
                EventDetails = response
            });
        }


        [Authorize(AuthenticationSchemes = "Jwt")]
        [HttpPost("events/{eventId}/details")]
        public async Task<IActionResult> UpsertEventDetails(int eventId, [FromBody] CreateEventDetailsDto dto)
        {
            var eventEntity = await _context.Events
                .Include(e => e.EventDetail)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventEntity == null)
                return NotFound("Event not found");

            if (eventEntity.EventDetail != null)
            {
                eventEntity.EventDetail.Title = dto.Title;
                eventEntity.EventDetail.Description = dto.Description;
                eventEntity.EventDetail.Venue = dto.Venue;
                eventEntity.EventDetail.CoverImage = dto.CoverImage;
                eventEntity.EventDetail.StartDateTime = dto.StartDateTime;
                eventEntity.EventDetail.EndDateTime = dto.EndDateTime;
            }
            else
            {
                var detail = new EventDetail
                {
                    EventId = eventId,
                    Title = dto.Title,
                    Description = dto.Description,
                    Venue = dto.Venue,
                    CoverImage = dto.CoverImage,
                    StartDateTime = dto.StartDateTime,
                    EndDateTime = dto.EndDateTime
                };

                _context.Set<EventDetail>().Add(detail);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Event details saved successfully",
            });
        }
    }
}
