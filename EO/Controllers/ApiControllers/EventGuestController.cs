using EO.Services.EventGuest;
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
        public async Task<IActionResult> Add(int eventId, int guestId)
        {
            await _service.AddGuestToEventAsync(eventId, guestId);
            return Ok(new { message = "Guest added to event" });
        }

        // REMOVE
        [HttpDelete]
        public async Task<IActionResult> Remove(int eventId, int guestId)
        {
            await _service.RemoveGuestFromEventAsync(eventId, guestId);
            return Ok(new { message = "Guest removed from event" });
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