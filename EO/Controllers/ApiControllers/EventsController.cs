using EO.Models;
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

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("GetAllEvents")]
        public async Task<IActionResult> GetAllEvents()
        {
            var now = DateTime.UtcNow;

            var events = await _context.Events
                .OrderBy(x => x.Date)
                .ThenBy(x => x.StartTime)
                .ToListAsync();

            var result = events.Select(x =>
            {
                var eventDateTime = x.Date + (x.StartTime ?? TimeSpan.Zero);
                var remainingDays = (eventDateTime - now).TotalDays;

                return new EventDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Location = x.Location,
                    Date = x.Date,
                    Type = x.Type,
                    IsRegistered = x.IsRegistered,
                    Image = x.Image,
                    RemainingDays = remainingDays > 0
                                    ? (int)Math.Ceiling(remainingDays) : 0,
                };
            });

            return Ok(new
            {
                success = true,
                message = "Events fetched successfully",
                events = result
            });
        }

        [Authorize]
        [HttpGet("TodayEvents")]
        public async Task<IActionResult> GetTodayEvents()
        {
            var today = DateTime.UtcNow.Date;

            var events = await _context.Events
                .Where(x => x.Date.Date == today)
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
                Type = x.Type,
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

        [Authorize]
        [HttpGet("UpcomingEvents")]
        public async Task<IActionResult> GetUpcomingEvents()
        {
            var today = DateTime.Today;

            var events = await _context.Events
                .Where(x => x.Date.Date > today)
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
                Type = x.Type,
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
                Type = model.Type,
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
            existingEvent.Type = model.Type;
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

        [Authorize]
        [HttpGet("event-details/{id}")]
        public async Task<IActionResult> GetEventDetails(int id)
        {
            var eventData = await _context.Events
                .Include(e => e.EventDetail)
                .Include(e => e.Schedules)
                .Include(e => e.Guests)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventData == null)
                return NotFound();

            var orderedSchedules = eventData.Schedules
               .OrderBy(s => s.Id)
               .ToList();

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
                .OrderBy(g => g.Id)
                .Select(g => new GuestDto
                {
                    Id = g.Id.ToString(),
                    Name = g.Name,
                    Designation = g.Designation,
                    Avatar = g.Avatar
                })
                .ToList()
            };

            return Ok(response);
        }


        [Authorize]
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

            return Ok(new { message = "Event details saved" });
        }
    }
}
