using EO.Models;
using EO.WebContext;
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

        [HttpGet("GetAllEvents")]
        public async Task<IActionResult> GetAllEvents()
        {
            var now =  DateTime.Now;
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
            return Ok(result);
        }

        [HttpGet("TodayEvents")]
        public async Task<IActionResult> GetTodayEvents()
        {
            var today = DateTime.Today;

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

            return Ok(result);
        }

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

            return Ok(result);
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
                message = "Event created successfully",
                data = model
            });
        }

        [HttpPut("UpdateEvent/{id}")]
        public async Task<IActionResult> UpdateEvent (int id, [FromBody] EventCreateDto model)
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
                messsage = "Event updated successfully",
                data = existingEvent
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
    }
}
