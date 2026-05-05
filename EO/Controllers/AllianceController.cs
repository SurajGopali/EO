using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using EO.Models;
using EO.WebContext;
using EO.Services.Common;

public class AllianceController : Controller
{
    private readonly IAllianceService _allianceService;
    private readonly AppDbContext _context;
    private readonly ICommonService _commonService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AllianceController(
        IAllianceService service,
        AppDbContext context,
        ICommonService commonService,
        UserManager<ApplicationUser> userManager)
    {
        _allianceService = service;
        _context = context;
        _commonService = commonService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _allianceService.GetAllAsync();
        return View(data);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Types = await _context.AllianceTypes.ToListAsync();
        ViewBag.Perks = await _context.Perks.ToListAsync();

        return PartialView("_EditAlliance", new AllianceDto());
    }

    public async Task<IActionResult> Edit(int id)
    {
        var data = await _allianceService.GetByIdAsync(id);

        ViewBag.Types = await _context.AllianceTypes.ToListAsync();
        ViewBag.Perks = await _context.Perks.ToListAsync();

        return PartialView("_EditAlliance", data);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert(AllianceDto dto)
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

        if (dto.Id > 0)
        {
            var existing = await _allianceService.GetByIdAsync(dto.Id);

            if (string.IsNullOrEmpty(dto.Logo))
                dto.Logo = existing.Logo;

            await _allianceService.UpdateAsync(dto);
        }
        else
        {
            await _allianceService.CreateAsync(dto);
        }

        return Json(new { success = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _allianceService.DeleteAsync(id);
        return Json(new { success = true, message = "Alliance deleted successfully" });
    }
}