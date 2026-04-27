using Azure;
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

        [Authorize(AuthenticationSchemes = "Jwt")]
        [HttpGet("GetAllEvents")]
        public async Task<IActionResult> GetAllEvents()
        {
            var now = DateTime.UtcNow;

            var events = await _context.Events
                .Include(x => x.EventType)
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
                    TypeId = x.EventTypeId,
                    Type = x.EventType?.Name,
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


        [Authorize(AuthenticationSchemes = "Jwt")]
        [HttpGet("event-types")]
        public async Task<IActionResult> GetEventTypes()
        {
            var types = await _context.EventTypes
                .OrderBy(x => x.Name)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                message = "Event types fetched successfully",
                EventTypes = types
            });
        }

        [Authorize(AuthenticationSchemes = "Jwt")]
        [HttpGet("filter-events")]
        public async Task<IActionResult> FilterEvents(
        [FromQuery] int? eventTypeId,
        [FromQuery] int? year,
        [FromQuery] string? range)
        {
            var today = DateTime.Now.Date;

            var query = _context.Events
                .Include(x => x.EventType)
                .AsQueryable();

           
            if (eventTypeId.HasValue)
            {
                query = query.Where(x => x.EventTypeId == eventTypeId.Value);
            }

            if (year.HasValue)
            {
                query = query.Where(x => x.Date.Year == year.Value);
            }

            if (!string.IsNullOrEmpty(range))
            {
                range = range.ToLower();

                if (range == "today")
                {
                    query = query.Where(x => x.Date.Date == today);
                }
                else if (range == "week")
                {
                    var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                    var endOfWeek = startOfWeek.AddDays(7);

                    query = query.Where(x =>
                        x.Date >= startOfWeek &&
                        x.Date < endOfWeek);
                }
                else if (range == "month")
                {
                    query = query.Where(x =>
                        x.Date.Month == today.Month &&
                        x.Date.Year == today.Year);
                }
            }

       
            var result = await query
                .OrderBy(x => x.Date)
                .ThenBy(x => x.StartTime)
                .Select(x => new EventDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Location = x.Location,
                    Date = x.Date,

                    TypeId = x.EventTypeId,
                    Type = x.EventType != null ? x.EventType.Name : null,

                    IsRegistered = x.IsRegistered,
                    Image = x.Image
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                count = result.Count,
                filteredData = result
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
