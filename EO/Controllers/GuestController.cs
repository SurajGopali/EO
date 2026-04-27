using EO.Models;
using EO.Services.Guest;
using Microsoft.AspNetCore.Mvc;

namespace EO.Controllers
{
    public class GuestController : Controller
    {
        private readonly IGuestService _guestService;

        public GuestController(IGuestService guestService)
        {
            _guestService = guestService;
        }

        // =====================
        // LIST ALL GUESTS
        // =====================
        public async Task<IActionResult> Index()
        {
            var guests = await _guestService.GetAllAsync();
            return View(guests);
        }

        // =====================
        // CREATE - GET
        // =====================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // =====================
        // CREATE - POST
        // =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateGuestDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _guestService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        // =====================
        // EDIT - GET
        // =====================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var guest = await _guestService.GetByIdAsync(id);

            if (guest == null)
                return NotFound();

            var dto = new CreateGuestDto
            {
                Name = guest.Name,
                Designation = guest.Designation,
                Avatar = guest.Avatar
            };

            ViewBag.GuestId = id;

            return View(dto);
        }

        // =====================
        // EDIT - POST
        // =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateGuestDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.GuestId = id;
                return View(dto);
            }

            var result = await _guestService.UpdateAsync(id, dto);

            if (!result)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // =====================
        // DELETE (IMPORTANT: anti-forgery recommended)
        // =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _guestService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}