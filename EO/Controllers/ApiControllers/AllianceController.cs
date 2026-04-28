using EO.Models;
using EO.WebContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EO.Controllers.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllianceController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public AllianceController(
            UserManager<ApplicationUser> userManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize(AuthenticationSchemes = "Jwt")]
        [HttpGet]
        public async Task<IActionResult> GetAllianceData()
        {
            var types = await _context.AllianceTypes
                .Select(t => t.Name)
                .ToListAsync();

            var alliances = await _context.Alliances
                .Include(a => a.AlliancePerks)
                    .ThenInclude(ap => ap.Perk)
                .ToListAsync();

            var result = new
            {
                types,
                alliances = alliances.Select(a => new
                {
                    name = a.Name,
                    category = a.AllianceTypeId, 
                    logo = a.Logo,
                    description = a.Description,

                    perks = a.AlliancePerks
                        .Select(ap => ap.Perk.Name)
                })
            };

            return Ok(new
            {
                success = true,
                message = "Alliance Data Fetched Successfully",
                alliance = result
            });
        }
    }
}

