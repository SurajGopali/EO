using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using EO.Models;
using EO.WebContext;
using EO.Services.Common;

public class AllianceController : Controller
{
    private readonly IAllianceService _service;
    private readonly AppDbContext _context;
    private readonly ICommonService _commonService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AllianceController(
        IAllianceService service,
        AppDbContext context,
        ICommonService commonService,
        UserManager<ApplicationUser> userManager)
    {
        _service = service;
        _context = context;
        _commonService = commonService;
        _userManager = userManager;
    }

    // ---------------- LIST ----------------
    public async Task<IActionResult> Index()
    {
        var data = await _service.GetAllAsync();
        return View(data);
    }

    // ---------------- CREATE (GET) ----------------
    public async Task<IActionResult> Create()
    {
        ViewBag.Types = await _context.AllianceTypes.ToListAsync();
        ViewBag.Perks = await _context.Perks.ToListAsync();
        return View();
    }

    // ---------------- CREATE (POST) ----------------
    [HttpPost]
    public async Task<IActionResult> Create(AllianceDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        if (dto.LogoFile != null)
        {
            dto.Logo = await _commonService.SaveFileAsync(
                dto.LogoFile,
                "alliances",
                user.Id);
        }

        await _service.CreateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    // ---------------- EDIT (GET) ----------------
    public async Task<IActionResult> Edit(int id)
    {
        var data = await _service.GetByIdAsync(id);

        ViewBag.Types = await _context.AllianceTypes.ToListAsync();
        ViewBag.Perks = await _context.Perks.ToListAsync();

        return View(data);
    }

    // ---------------- EDIT (POST) ----------------
    [HttpPost]
    public async Task<IActionResult> Edit(AllianceDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var existing = await _service.GetByIdAsync(dto.Id);

        // keep old logo if no new file uploaded
        dto.Logo = existing.Logo;

        if (dto.LogoFile != null)
        {
            dto.Logo = await _commonService.SaveFileAsync(
                dto.LogoFile,
                "alliances",
                user.Id);
        }

        await _service.UpdateAsync(dto);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}