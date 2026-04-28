using EO.Models;
using EO.Services.Event;
using EO.WebContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EO.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        private readonly AppDbContext _context;

        public EventController(IEventService eventService, AppDbContext context)
        {
            _eventService = eventService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _eventService.GetEventsAsync();
            return View(events);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            var data = await _eventService.GetEventDetailsAsync(id);

            if (data == null)
                return NotFound();

            return View(data);
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditDetails(int id)
        {
            var model = await _eventService.GetFullEventForEditAsync(id);

            if (model == null)
                return NotFound();

            var allGuests = await _context.Guests.ToListAsync();
            ViewBag.AllGuests = allGuests;

            var selectedGuestIds = await _context.EventGuests
                .Where(x => x.EventId == id)
                .Select(x => x.GuestId)
                .ToListAsync();

            ViewBag.SelectedGuestIds = selectedGuestIds;

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditDetails(int id, UpdateEventDetailFullDto dto)
        {
            await _eventService.UpdateFullEventAsync(id, dto);

            return RedirectToAction("Details", new { id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var eventTypes = await _context.EventTypes.ToListAsync();
            ViewBag.EventTypes = eventTypes;

       
            if (id == 0)
            {
                return View(new EventUpdateDto());
            }

         
            var model = await _eventService.GetEventForEditAsync(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, EventUpdateDto dto)
        {
            await _eventService.UpsertEventAsync(id, dto);
            return RedirectToAction("Index");
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _eventService.DeleteEventAsync(id);
            return RedirectToAction("Index");
        }

    }
}