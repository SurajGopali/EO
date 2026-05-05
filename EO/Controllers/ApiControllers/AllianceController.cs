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
        private readonly IAllianceService _allianceService;

        public AllianceController(
            UserManager<ApplicationUser> userManager,
            AppDbContext context,
            IAllianceService allianceService)
        {
            _userManager = userManager;
            _context = context;
            _allianceService = allianceService;
        }

        [Authorize(AuthenticationSchemes = "Jwt")]
        [HttpGet]
        public async Task<IActionResult> GetAllianceData()
        {
            var alliances = await _allianceService.GetAllAsync();
            return Ok(new
            {
                success = true,
                message = "Alliance Data Fetched Successfully",
                alliance = new
                {
                    alliances
                }
            });
        }
    }
}

