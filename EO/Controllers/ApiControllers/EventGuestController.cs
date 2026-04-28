using EO.Services.EventGuests;
using Microsoft.AspNetCore.Mvc;

namespace EO.Controllers.ApiControllers
{
    [Route("api/event-guests")]
    [ApiController]
    public class EventGuestController : ControllerBase
    {
        private readonly IEventGuestService _service;

        public EventGuestController(IEventGuestService service)
        {
            _service = service;
        }

        // ADD
        [HttpPost]
        public async Task<IActionResult> Add(int eventId, string userId)
        {
            await _service.AddGuestToEventAsync(eventId, userId);
            return Ok(new { message = "User added to event" });
        }

        // REMOVE
        [HttpDelete]
        public async Task<IActionResult> Remove(int eventId, string userId)
        {
            await _service.RemoveGuestFromEventAsync(eventId, userId);
            return Ok(new { message = "User removed from event" });
        }

        // GET
        [HttpGet("{eventId}")]
        public async Task<IActionResult> Get(int eventId)
        {
            var guests = await _service.GetGuestsByEventIdAsync(eventId);
            return Ok(guests);
        }
    }
}