using EO.Models;
using EO.Services.Common;
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
        private readonly ICommonService _commonService;

        public EventController(IEventService eventService, AppDbContext context,ICommonService commonService)
        {
            _eventService = eventService;
            _context = context;
            _commonService = commonService;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _eventService.GetEventsAsync();
            return View(events);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            var data = await _eventService.GetEventDetailsAsync(id);

            if (data == null) return NotFound();

            return View(data);
        }

       
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditDetails(int id)
        {
            var model = await _eventService.GetFullEventForEditAsync(id);

            if (model == null) return NotFound();

            var allUsers = await _context.Users.ToListAsync();
            ViewBag.AllUsers = allUsers;

            var selectedUserIds = await _context.EventGuests
                .Where(x => x.EventId == id)
                .Select(x => x.UserId)
                .ToListAsync();

            ViewBag.SelectedUserIds = selectedUserIds;

            return PartialView("_EditEventDetailsPartial", model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditDetails(UpdateEventDetailFullDto dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (dto.Event?.CoverImageFile != null)
            {
                dto.Event.CoverImage = await _commonService.SaveFileAsync(
                    dto.Event.CoverImageFile,
                    "events",
                    userId ?? "default"
                );
             }

            await _eventService.UpdateFullEventAsync(dto.EventId, dto);

            return Json(new { success = true });
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var eventTypes = await _context.EventTypes.ToListAsync();
            ViewBag.EventTypes = eventTypes;

            if (id == 0)
                return PartialView("_EditEventPartial", new EventUpdateDto());

            var model = await _eventService.GetEventForEditAsync(id);

            if (model == null) return NotFound();

            return PartialView("_EditEventPartial", model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(EventUpdateDto dto)
        {
            var id = dto.Id;
            if (dto.ImageFile != null)
            {
                var userId = User.FindFirst("sub")?.Value
                             ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var imagePath = await _commonService.SaveFileAsync(
                    dto.ImageFile,
                    "events",
                    userId ?? "system"
                );

                dto.Image = imagePath;
            }

            await _eventService.UpsertEventAsync(id, dto);

            return Json(new { success = true });
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _eventService.DeleteEventAsync(id);

            return Json(new
            {
                success = true,
                message = "Event deleted successfully"
            });
        }
    }
}